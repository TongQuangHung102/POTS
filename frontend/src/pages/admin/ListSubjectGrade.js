import React, { useState, useEffect } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import BackLink from "../../components/BackLink";
import { fetchSubjects } from "../../services/SubjectService";
import { getSubjectGradesByGrade } from "../../services/SubjectGradeService";
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

  const navigate = useNavigate();
  //get du lieu

  const fetchSubjectGrades = async () => {
    try {
      setIsLoading(true)
      const data = await getSubjectGradesByGrade(gradeId);
      setSubjectGrades(data);
      if(data.length > 0){
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

  useEffect(() => {
    fetchSubjectGrades();
  }, [selectedSubjectGrades]);

  //add new grade
  const handleAddSubject = async () => {

    try {
      const response = await fetch('https://localhost:7259/api/SubjectGrade/add-subject-grade', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          subjectId: selectSubject,
          gradeId: gradeId
        }),
      });

      if (!response.ok) {
        const errorResponse = await response.json();
        const errorMessage = errorResponse ? errorResponse.message : 'Không thể thêm lớp mới';
        setTimeout(() => {
          setErrorMessage('');
        }, 3000);
        throw new Error(errorMessage);
      }

      loadSubjects();
      setShowAddSubjectGrades(false);
      setErrorMessage('');
      setSuccessMessage('Lớp đã được thêm thành công!');

      setTimeout(() => {
        setSuccessMessage('');
      }, 3000);
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
          <button className="add-chapter" onClick={() => setShowAddSubjectGrades(true)}>Thêm Môn Học Mới</button>
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
            <div className="action-buttons">
              <button onClick={handleAddSubject}>Thêm</button>
              <button onClick={() => setShowAddSubjectGrades(false)} >Hủy</button>
            </div>
          </div>
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
               <Link to={`/admin/grades/${gradeId}/subject/${subject.id}`}>Chương trình</Link>
                </button>
                <button>
               <Link to={`/admin/grades/${gradeId}/subject/${subject.id}/list_tests`}>Bài kiểm tra</Link>
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
