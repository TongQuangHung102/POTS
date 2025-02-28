import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import './ListChapter.css';


const ListGrades = () => {
  const [grades, setGrades] = useState([]);
  const [newGrades, setNewGrades] = useState('');
  const [showAddGrade, setShowAddGrade] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [isEditing, setIsEditing] = useState(false);
  const [selectedGrade, setSelectedGrade] = useState(null);

  //get du lieu
  useEffect(() => {
    const fetchGrades = async () => {
      try {
        const response = await fetch('https://localhost:7259/api/Grade/get-all-grade');
        const data = await response.json();
        setGrades(data);
      } catch (error) {
        console.error("Có lỗi khi lấy dữ liệu lớp", error);
      }
    };

    fetchGrades();
  }, []);

  //add new grade
  const handleAddGrade = async () => {
    if (!newGrades.trim()) return;

    try {
      const response = await fetch('https://localhost:7259/api/Grade/add-grade', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(newGrades),
      });

      if (!response.ok) {
        const errorResponse = await response.text();
        const errorMessage = errorResponse ? errorResponse : 'Không thể thêm lớp mới';
        setTimeout(() => {
          setErrorMessage('');
        }, 3000);
        throw new Error(errorMessage);
      }

      const gradesResponse = await fetch('https://localhost:7259/api/Grade/get-all-grade', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      const gradesData = await gradesResponse.json();
      setGrades(gradesData);
      setNewGrades('');
      setShowAddGrade(false);
      setErrorMessage('');
      setSuccessMessage('Lớp đã được thêm thành công!');


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

  const handleSave = async () => {
    try {
      const response = await fetch(`https://localhost:7259/api/Grade/update-grade/${selectedGrade.gradeId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          gradeName: selectedGrade.gradeName,
          description: selectedGrade.gradeDescription,
          isVisible: selectedGrade.gradeIsVisible,
        }),
      });
      const message = await response.json();

      if (!response.ok) {
        throw new Error(message.message);
      }


      alert("Cập nhật thành công!");
      setIsEditing(false);
      setErrorMessage("");
      const chaptersResponse = await fetch('https://localhost:7259/api/Grade/get-all-grade', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      const gradesData = await chaptersResponse.json();
      setGrades(gradesData);
    } catch (error) {
      setErrorMessage(error.message);
    }
  };




  return (
    <div className="chapter-list-container">
      <h2>Danh Sách Khối Lớp</h2>
      <div className="group-header">
        <div>
           <Link className="backlink" to='/admin'>Trang chủ</Link>/ Khối lớp 
        </div>
        <button className="add-chapter" onClick={() => setShowAddGrade(true)}>Thêm lớp mới</button>
      </div>
      {showAddGrade && (
        <div>
          <div className="add-chapter-form">
            <input
              type="text"
              placeholder="Nhập tên chương mới"
              value={newGrades}
              onChange={(e) => setNewGrades(e.target.value)}
            />
            <div className="action-buttons">
              <button onClick={handleAddGrade}>Thêm</button>
              <button onClick={() => setShowAddGrade(false)}>Hủy</button>
            </div>
          </div>
          <div>
            <p className="instruction-text">* Vui lòng nhập tên lớp theo định dạng: "Lớp số"</p>
          </div>
        </div>

      )}
      {errorMessage && <div className="error-message">{errorMessage}</div>}
      {successMessage && <div className="success-message">{successMessage}</div>}
      <table className="chapter-table">
        <thead>
          <tr>
            <th style={{ width: "10%" }}>Id</th>
            <th style={{ width: "30%" }}>Tên khối lớp</th>
            <th style={{ width: "20%" }}>Mô tả</th>
            <th style={{ width: "20%" }}>Trạng thái</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {grades.map((grade) => (
            <tr key={grade.gradeId}>
              <td>{grade.gradeId}</td>
              <td>{grade.gradeName}</td>
              <td>{grade.gradeDescription}</td>
              <td>{grade.gradeIsVisible ? <span style={{ color: "green" }}>Hoạt động</span> : <span style={{ color: "red" }}>Không hoạt động</span>}</td>
              <td>
                <button>
                  <Link to={`/admin/${grade.gradeId}`}>Chương trình</Link>
                </button>
                <button>
                  <Link to={`/admin/grade/${grade.gradeId}`}>Chương trình</Link>
                </button>
                <button onClick={() => handleEdit(grade)}>Chỉnh sửa</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      {/* Form chỉnh sửa */}
      {isEditing && (
        <div className="modal">
          <div className="modal-content">
            <h3>Chỉnh sửa lớp</h3>
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
            <div className="button-group">
              <button onClick={handleSave}>Lưu</button>
              <button onClick={handleClose}>Đóng</button>
            </div>
            {errorMessage && <p className="error-message">{errorMessage}</p>}
          </div>
        </div>
      )}
    </div>
  );
};

export default ListGrades;
