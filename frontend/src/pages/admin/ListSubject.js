import React, { useState, useEffect } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import BackLink from "../../components/BackLink";
import { fetchSubjects } from "../../services/SubjectService";
import { authFetch } from "../../utils/authFetch";
import '../content_manager/ListChapter.css';
import { cleanAndValidateString } from "../../utils/validator";


const ListSubject = () => {
    const [subjects, setSubjects] = useState([]);
    const [showAddSubject, setShowAddSubject] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');
    const [errorMessageModal, setErrorMessageModal] = useState('');
    const [successMessage, setSuccessMessage] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [selectedSubject, setSelectedSubject] = useState(null);
    const [subjectName, setSubjectName] = useState('');
    const [isEditing, setIsEditing] = useState(false);

    const apiUrl = process.env.REACT_APP_API_URL; 

    const navigate = useNavigate();
    //get du lieu

    const loadSubjects = async () => {
        const data = await fetchSubjects();
        setSubjects(data);
    }

    useEffect(() => {
        setIsLoading(true);
        loadSubjects();
        setIsLoading(false);
    }, []);

    useEffect(() => {
        setTimeout(() => {
            setErrorMessage('');
            setSuccessMessage('');
        }, 4000);
    }, [errorMessage, successMessage]);

    //add new grade
    const handleAddSubject = async () => {
        if (!subjectName.trim()){
            setErrorMessage("Tên môn học không được bỏ trống");
            return;
        }
        if (subjectName.length > 40) {
            setErrorMessage('Tên môn học không được vượt quá 40 ký tự');
            return;
        }
        if(hasSpecialCharacters(subjectName)){
            setErrorMessage("Tên môn học không được chứa ký tự đặc biệt");
            return;
        }
        try {
            const response = await authFetch(`${apiUrl}/api/Subject/add-subject`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    subjectName: subjectName,
                    isVisible: true
                }),
            });

            if (!response.ok) {
                const errorResponse = await response.json();
                const errorMessage = errorResponse ? errorResponse.message : 'Không thể thêm môn mới';
                throw new Error(errorMessage);
            }

            loadSubjects();
            setShowAddSubject(false);
            setErrorMessage('');
            setSubjectName('');
            setSuccessMessage('Môn học đã được thành công');

        } catch (error) {
            setErrorMessage(error.message);
            console.error("Có lỗi khi thêm môn học mới", error);
        }
    };

    const handleEdit = (subject) => {
        setSelectedSubject(subject);
        setIsEditing(true);
    }

    const hasSpecialCharacters = (text) => {
        const regex = /[^a-zA-ZÀ-Ỷà-ỹ0-9 "'-]/;
        return regex.test(text);
      };
    const handleSave = async () => {
        if (!selectedSubject.name.trim()){
            setErrorMessageModal("Tên môn học không được bỏ trống");
            return;
        }
        if (selectedSubject.name.length > 40) {
            setErrorMessageModal('Tên môn học không được vượt quá 40 ký tự');
            return;
        }
        if(hasSpecialCharacters(selectedSubject.name)){
            setErrorMessageModal("Tên môn học không được chứa ký tự đặc biệt");
            return;
        }
        try {
          const response = await authFetch(`${apiUrl}/api/Subject/edit-subject`, {
            method: "PUT",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({
                subjectId: selectedSubject.id,
                subjectName: selectedSubject.name,
                isVisible: selectedSubject.isVisible
            }),
          });
          const message = await response.json();
    
          if (!response.ok) {
            throw new Error(message.message || "Có lỗi xảy ra!");
          }
    
          alert("Cập nhật thành công");
          setIsEditing(false);
          setErrorMessageModal("");
          loadSubjects();
        } catch (error) {
          setErrorMessageModal(error.message);
        }
      };

      const handleClose = () => {
        setIsEditing(false);
        setSelectedSubject(null);
        setErrorMessageModal('');
      };


    if (isLoading) {
        return <div className="loading-dashboard">Đang tải dữ liệu...</div>;
    }

    return (
        <div className="chapter-list-container">
            <h2>Danh Sách Môn Học </h2>
            <div className="group-header">
                <div>
                    <BackLink />
                </div>
                <div>
                    <button className="add-chapter" onClick={() => setShowAddSubject(true)}>Thêm môn học mới</button>
                </div>
            </div>
            {showAddSubject && (
                <div>
                    <div className="add-chapter-form">
                        <div className="action-buttons">
                            <input
                                type="text"
                                value={subjectName}
                                onChange={(e) => setSubjectName(e.target.value)}
                                placeholder="Nhập tên môn học"
                            />
                        </div>
                        <div className="action-buttons">
                            <button onClick={handleAddSubject}>Thêm</button>
                            <button onClick={() => setShowAddSubject(false)} >Hủy</button>
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
                        <th style={{ width: "15%" }}>Trạng thái</th>
                        <th style={{ width: "15%" }}>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {subjects.length > 0 ?(subjects.map((subject) => (
                        <tr key={subject.id}>
                            <td>{subject.id}</td>
                            <td>{subject.name}</td>
                            <td>{subject.isVisible ? <span style={{ color: "green" }}>Hoạt động</span> : <span style={{ color: "red" }}>Không hoạt động</span>}</td>
                            <td>
                                <button onClick={ () => handleEdit(subject)}>
                                    Chỉnh sửa
                                </button>
                            </td>
                        </tr>
                    ))):(<tr>
                        <td colSpan="4" style={{ textAlign: "center", fontStyle: "italic", color: "gray" }}>
                          Chưa có môn nào.
                        </td>
                      </tr>)}
                </tbody>
            </table>

            {isEditing && (
                <div className="modal">
                    <div className="modal-content">
                        <h3>Chỉnh sửa môn</h3>
                        <label>
                            Tên môn học
                            <input
                                type="text"
                                value={selectedSubject?.name}
                                onChange={(e) =>
                                    setSelectedSubject({
                                        ...selectedSubject,
                                        name: e.target.value
                                    })
                                }
                            />
                        </label>
                        <label>
                            Trạng thái:
                            <select
                                value={selectedSubject?.isVisible}
                                onChange={(e) =>
                                    setSelectedSubject({
                                        ...selectedSubject,
                                        isVisible: e.target.value === "true"
                                    })
                                }
                            >
                                <option value="true">Hoạt động</option>
                                <option value="false">Không hoạt động</option>
                            </select>
                        </label>
                        <div className="button-group">
                            <button onClick={handleSave}>Lưu</button>
                            <button onClick={handleClose}>Đóng</button>
                        </div>
                        {errorMessageModal && <p className="error-message">{errorMessageModal}</p>}
                    </div>
                </div>

            )}
        </div>
    );
};

export default ListSubject;
