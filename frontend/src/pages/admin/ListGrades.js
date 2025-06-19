import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import BackLink from "../../components/BackLink";
import { authFetch } from "../../utils/authFetch";
import '../content_manager/ListChapter.css';


const ListGrades = () => {
  const [grades, setGrades] = useState([]);
  const [newGrades, setNewGrades] = useState('');
  const [showAddGrade, setShowAddGrade] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [errorMessageModal, setErrorMessageModal] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [isEditing, setIsEditing] = useState(false);
  const [selectedGrade, setSelectedGrade] = useState(null);
  const [contentManagers, setContentManagers] = useState([]);

  const apiUrl = process.env.REACT_APP_API_URL;

  const navigate = useNavigate();
  //get du lieu

  const fetchGrades = async () => {
    try {
      const response = await authFetch(`${apiUrl}/api/Grade/get-all-grade`);
      const data = await response.json();

      setGrades(data);
    } catch (error) {
      console.error("Có lỗi khi lấy dữ liệu lớp", error);
    }
  };


  const fetchContentManagers = async () => {
    try {
      const response = await authFetch(`${apiUrl}/api/User/get-user-by-roleId/4`);
      if (!response.ok) {
        throw new Error("Không thể tải danh sách người quản lý nội dung.");
      }
      const data = await response.json();
      setContentManagers(data);
    } catch (error) {
      console.error("Lỗi khi lấy danh sách người quản lý nội dung:", error);
    }
  };

  useEffect(() => {
    fetchContentManagers()
  }, []);

  useEffect(() => {
    fetchGrades()
  }, [selectedGrade]);

  useEffect(() => {
    setTimeout(() => {
      setErrorMessage('');
    }, 3000);
  }, [errorMessage]);

  const isValidGrade = (newGrades) => {
    const regex = /^Khối (1[0-2]|[1-9])$/;
    return regex.test(newGrades);
  };

  //add new grade
  const handleAddGrade = async () => {
    if (!newGrades.trim()) {
      setErrorMessage("Tên khối không được bỏ trống");
      return;
    }

    if (!isValidGrade(newGrades)) {
      setErrorMessage("Tên khối không hợp lệ");
      return;
    }

    try {
      const response = await authFetch(`${apiUrl}/api/Grade/add-grade`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(newGrades)
      });

      if (!response.ok) {
        const errorResponse = await response.json();
        const errorMessage = errorResponse.message ? errorResponse.message : 'Không thể thêm lớp mới';
        throw new Error(errorMessage);
      }

      fetchGrades()
      setNewGrades('');
      setShowAddGrade(false);
      setErrorMessage('');
      setSuccessMessage('Khối đã được thêm thành công');


      setTimeout(() => {
        setSuccessMessage('');
      }, 3000);
    } catch (error) {
      setErrorMessage(error.message);
      console.error("Có lỗi khi thêm chương mới", error);
    }
  };

  //edit chapter
  const handleEdit = (grade) => {
    setSelectedGrade(grade);
    setIsEditing(true);
  };

  const handleClose = () => {
    setIsEditing(false);
    setSelectedGrade(null);
  };

  const handleToCategoryTest = () => {
    navigate('/admin/grades/test_category')
  }

  const handleToSubjects = () => {
    navigate('/admin/grades/subject')
  }

  const handleSave = async () => {
    if (!selectedGrade.gradeName.trim()) {
      setErrorMessageModal("Tên khối không được bỏ trống");
      return;
    }

    if (!isValidGrade(selectedGrade.gradeName)) {
      setErrorMessageModal("Tên khối không hợp lệ");
      return;
    }

    try {
      const response = await authFetch(`${apiUrl}/api/Grade/update-grade/${selectedGrade.gradeId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          gradeName: selectedGrade.gradeName,
          description: selectedGrade.gradeDescription,
          isVisible: selectedGrade.gradeIsVisible,
          userId: selectedGrade.userId
        }),
      });
      const message = await response.json();

      if (!response.ok) {
        throw new Error(message.message);
      }


      alert("Cập nhật thành công");
      setIsEditing(false);
      setErrorMessageModal("");
      fetchGrades();
    } catch (error) {
      setErrorMessageModal(error.message);
    }
  };

  return (
    <div className="chapter-list-container">
      <h2>Danh Sách Khối Lớp</h2>
      <div className="group-header">
        <div>
          <BackLink />
        </div>
        <div>
          <button className="add-chapter" onClick={() => setShowAddGrade(true)}>Thêm khối mới</button>
          <button className="add-chapter" onClick={() => handleToCategoryTest()}>Quản lý bài kiểm tra</button>
          <button className="add-chapter" onClick={() => handleToSubjects()}>Quản lý môn học</button>
        </div>

      </div>
      {showAddGrade && (
        <div>
          <div className="add-chapter-form">
            <input
              type="text"
              placeholder="Nhập tên khối mới"
              value={newGrades}
              onChange={(e) => setNewGrades(e.target.value)}
            />
            <div className="action-buttons">
              <button onClick={handleAddGrade}>Thêm</button>
              <button onClick={() => setShowAddGrade(false)}>Hủy</button>
            </div>
          </div>
          <div>
            <p className="instruction-text">* Vui lòng nhập tên khối theo định dạng: "Khối số"</p>
          </div>
        </div>

      )}
      {errorMessage && <div className="error-message">{errorMessage}</div>}
      {successMessage && <div className="success-message">{successMessage}</div>}
      <table className="chapter-table">
        <thead>
          <tr>
            <th style={{ width: "5%" }}>Id</th>
            <th style={{ width: "15%" }}>Tên khối lớp</th>
            <th style={{ width: "20%" }}>Mô tả</th>
            <th style={{ width: "15%" }}>Người quản lý</th>
            <th style={{ width: "15%" }}>Trạng thái</th>
            <th style={{ width: "30%" }}>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {grades.length > 0 ? (grades.map((grade) => (
            <tr key={grade.gradeId}>
              <td>{grade.gradeId}</td>
              <td>{grade.gradeName}</td>
              <td>{grade.gradeDescription}</td>
              <td>{grade.userName}</td>
              <td>{grade.gradeIsVisible ? <span style={{ color: "green" }}>Hoạt động</span> : <span style={{ color: "red" }}>Không hoạt động</span>}</td>
              <td>
                <button onClick={() => handleEdit(grade)}>Chỉnh sửa</button>
              </td>
            </tr>
          ))) : (<tr>
            <td colSpan="6" style={{ textAlign: "center", fontStyle: "italic", color: "gray" }}>
              Chưa có lớp nào.
            </td>
          </tr>)}
        </tbody>
      </table>
      {/* Form chỉnh sửa */}
      {isEditing && (
        <div className="modal">
          <div className="modal-content">
            <h3>Chỉnh sửa khối</h3>
            <label>
              Tên khối:
              <input
                type="text"
                value={selectedGrade?.gradeName}
                onChange={(e) =>
                  setSelectedGrade({ ...selectedGrade, gradeName: e.target.value })
                }
              />
            </label>
            <label>
              Mô tả:
              <input
                type="text"
                value={selectedGrade?.gradeDescription}
                onChange={(e) =>
                  setSelectedGrade({ ...selectedGrade, gradeDescription: e.target.value })
                }
              />
            </label>
            <label>
              Trạng thái:
              <select
                value={selectedGrade?.gradeIsVisible}
                onChange={(e) =>
                  setSelectedGrade({
                    ...selectedGrade,
                    gradeIsVisible: e.target.value === "true"
                  })
                }
              >
                <option value="true">Hoạt động</option>
                <option value="false">Không hoạt động</option>
              </select>
            </label>
            <label>
              Người quản lý
              <select
                value={selectedGrade?.userId?.toString() || "0"} 
                onChange={(e) =>
                  setSelectedGrade({
                    ...selectedGrade,
                    userId: e.target.value === "0" ? null : parseInt(e.target.value)
                  })
                }
              >
                <option value="0">Không có</option>
                {contentManagers.map((user) => (
                  <option key={user.userId} value={user.userId}>
                    {user.userName}
                  </option>
                ))}
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

export default ListGrades;
