import React, { useState, useEffect } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import BackLink from "../../components/BackLink";
import { fetchSubjects } from "../../services/SubjectService";
import { getSubjectGradesByGrade } from "../../services/SubjectGradeService";
import { authFetch } from "../../utils/authFetch";

import './ListChapter.css';


const ListSubjectGrades = () => {
  const { gradeId } = useParams();
  const [subjectGrades, setSubjectGrades] = useState([]);
  const [grade, setGrade] = useState([]);
  const [showAddSubjectGrades, setShowAddSubjectGrades] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [selectedSubjectGrades, setSelectedSubjectGrades] = useState(null);
  const [subject, setSubject] = useState([]);
  const [selectSubject, setSelectSubject] = useState([]);
  const [file1, setFile1] = useState(null);
  const [file2, setFile2] = useState(null);

  const [loadingFile, setLoadingFile] = useState(false);

  const roleId = localStorage.getItem('roleId');
  const apiUrl = process.env.REACT_APP_API_URL;

  const navigate = useNavigate();
  //get du lieu

  const fetchSubjectGrades = async () => {
    try {
      setIsLoading(true)
      const data = await getSubjectGradesByGrade(gradeId);
      setSubjectGrades(data);
      if (data.length > 0) {
        setGrade(data[0].grade);
      }
      setIsLoading(false)
    } catch (error) {
      console.error("Có lỗi khi lấy dữ liệu lớp", error);
    }
  };

  const loadSubjects = async () => {
    const data = await fetchSubjects();
    setSubject(data);
  }

  useEffect(() => {
    loadSubjects();
  }, []);

  useEffect(() =>{
    setTimeout(() => {
      setSuccessMessage('');
      setErrorMessage('');
    }, 3000);
  },[successMessage, errorMessage])

  useEffect(() => {
    fetchSubjectGrades();
  }, [selectedSubjectGrades]);

  //add new grade
  const handleAddSubject = async () => {
    const formData = new FormData();
    if(selectSubject.toString() === ""){
      setErrorMessage('Vui lòng chọn môn học');
      return;
    }

    formData.append('SubjectId', selectSubject.toString()); // Ví dụ: số -> string
    formData.append('GradeId', gradeId.toString());

    if (!file1 || !file2) {
      setErrorMessage("Vui lòng chọn đủ cả 2 file Kỳ 1 và Kỳ 2.");
      setTimeout(() => setErrorMessage(''), 3000);
      return;
    }

    formData.append('FileKy1', file1);
    formData.append('FileKy2', file2);
  
    setLoadingFile(true);
    try {
      const response = await authFetch(`${apiUrl}/api/SubjectGrade/add-subject-grade`, {
        method: 'POST',
        body: formData,
      });

      setLoadingFile(false);

      if (!response.ok) {
        const errorResponse = await response.json();
        console.error("Lỗi từ server:", errorResponse); // <- thêm dòng này để xem lỗi
        throw new Error(errorResponse.message);
      }

      fetchSubjectGrades();
      setShowAddSubjectGrades(false);
      setErrorMessage('');
      setSuccessMessage('Môn học đã được thêm thành công!');

    } catch (error) {
      setErrorMessage(error.message);
      console.error("Có lỗi khi thêm môn học mới", error);
    }
  };


  const handleChange = (event) => {
    setSelectSubject(event.target.value);
  };

  if (isLoading) {
    return <div className="loading-dashboard">Đang tải dữ liệu...</div>;
  }

  return (
    <div className="chapter-list-container">
      <h2>Danh Sách Môn Học {grade}</h2>
      <div className="group-header">
        <div>
          <BackLink />
        </div>
        <div>
          <button className="add-chapter" onClick={() => setShowAddSubjectGrades(true)}>Thêm môn học mới</button>
        </div>
      </div>
      {showAddSubjectGrades && (
        <div>
          <div className="add-chapter-form">
            <div className="action-buttons">
              <select value={selectSubject} onChange={handleChange}>
                <option value="">-- Chọn môn học --</option>
                {subject.filter(s => s.isVisible).map((subject) => (
                  <option key={subject.id} value={subject.id}>
                    {subject.name}
                  </option>
                ))}
              </select>
            </div>
            <div className='action-buttons'>
              <label>Kỳ 1:</label>
              <input style={{ padding: '2px', border: 'none', marginLeft: '5px' }}
                type="file"
                accept=".docx, .pdf, .txt"
                onChange={(e) => setFile1(e.target.files[0])}
                 className="styled-file-input"
              />
            </div>
            <div className='action-buttons'>
              <label>Kỳ 2:</label>
              <input style={{ padding: '2px', border: 'none', marginLeft: '5px' }}
                type="file"
                accept=".docx, .pdf, .txt"
                onChange={(e) => setFile2(e.target.files[0])}
                 className="styled-file-input"
              />
            </div>
            <div className="action-buttons">
              <button onClick={handleAddSubject}>Thêm</button>
              <button onClick={() => setShowAddSubjectGrades(false)} >Hủy</button>
              
            </div>
            
          </div>
          {loadingFile && (<p className="loadingFile-message">
            Quá trình tải file có thể mất vài phút, vui lòng đợi...
          </p>)}
        </div>

      )}
      {errorMessage && <div className="error-message">{errorMessage}</div>}
      {successMessage && <div className="success-message">{successMessage}</div>}
      <table className="chapter-table">
        <thead>
          <tr>
            <th style={{ width: "10%" }}>Id</th>
            <th style={{ width: "15%" }}>Môn học</th>
            <th style={{ width: "15%" }}>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {subjectGrades.length > 0 ? (
            subjectGrades.map((subject) => (
              <tr key={subject.id}>
                <td>{subject.id}</td>
                <td>{subject.name}</td>
                <td>
                  <button>
                    <Link to={`/content_manage/grades/${gradeId}/subject/${subject.subjectId}`}>Chương trình</Link>
                  </button>
                  <button>
                    <Link to={`/content_manage/grades/${gradeId}/subject/${subject.subjectId}/list_tests`}>Bài kiểm tra</Link>
                  </button>
                </td>
              </tr>
            ))) :
            (<tr>
              <td colSpan="3" style={{ textAlign: "center", fontStyle: "italic", color: "gray" }}>
                Chưa có môn học nào.
              </td>
            </tr>
            )}
        </tbody>
      </table>
    </div>
  );
};

export default ListSubjectGrades;
