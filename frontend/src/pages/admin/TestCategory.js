import React, { useState, useEffect } from "react";
import BackLink from "../../components/BackLink";
import { authFetch } from "../../utils/authFetch";
import { normalizeName } from "../../utils/validator";
import '../content_manager/ListChapter.css';


const TestCategory = () => {
  const [testCategory, setTestCategory] = useState([]);
  const [newTestCategory, setNewTestCategory] = useState('');
  const [showAdd, setShowAdd] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [errorMessageModal, setErrorMessageModal] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [isEditing, setIsEditing] = useState(false);
  const [selectedTestCategory, setSelectedTestCategory] = useState(null);
  const apiUrl = process.env.REACT_APP_API_URL;
  //get du lieu
  useEffect(() => {
    const fetchTestCategory = async () => {
      try {
        const response = await authFetch(`${apiUrl}/api/TestCategory/get-all-test-category`);
        const data = await response.json();
        setTestCategory(data);
      } catch (error) {
        console.error("Có lỗi khi lấy dữ liệu lớp", error);
      }
    };

    fetchTestCategory();
  }, []);

  useEffect(() => {
    setTimeout(() => {
      setErrorMessage('');
      setSuccessMessage('');
    }, 4000);
  }, [errorMessage, successMessage]);

  //add new grade
  const handleAddTestCategory = async () => {
    if (!newTestCategory.trim()){
      setErrorMessage('Tên loại bài kiểm tra không được bỏ trống');
      return;
    };

    if (newTestCategory.trim().length > 50) {
      setErrorMessage('Tên loại bài kiểm tra không được vượt quá 50 ký tự');
      return;
    }

    try {
      const response = await authFetch(`${apiUrl}/api/TestCategory/add-new-test-category`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          categoryName: normalizeName(newTestCategory),
          isVisible: true,
        }),
      });

      if (!response.ok) {
        const errorResponse = await response.json();
        const errorMessage = errorResponse ? errorResponse.message : 'Không thể thêm loại bài mới';
        throw new Error(errorMessage);
      }

      const tCategoryResponse = await authFetch(`${apiUrl}/api/TestCategory/get-all-test-category`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      const tCategoryData = await tCategoryResponse.json();
      setTestCategory(tCategoryData);
      setNewTestCategory('');
      setShowAdd(false);
      setErrorMessage('');
      setSuccessMessage('Loại bài đã được thêm thành công');

    } catch (error) {
      setErrorMessage(error.message);
      console.error("Có lỗi khi thêm chương mới", error);
    }
  };

  //edit chapter
  const handleEdit = (t) => {
    setSelectedTestCategory(t);
    setIsEditing(true);
  };

  const handleClose = () => {
    setIsEditing(false);
    setSelectedTestCategory(null);
  };

  const handleSave = async () => {
    if (!selectedTestCategory.categoryName.trim()){
      setErrorMessageModal('Tên loại bài kiểm tra không được bỏ trống');
      return;
    };

    if (selectedTestCategory.categoryName.trim().length > 50) {
      setErrorMessageModal('Tên loại bài kiểm tra không được vượt quá 50 ký tự');
      return;
    }
    try {
      const response = await authFetch(`${apiUrl}/api/TestCategory/update-test-category/${selectedTestCategory.testCategoryId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          testCategoryId: selectedTestCategory.testCategoryId,
          categoryName: normalizeName(selectedTestCategory.categoryName),
          isVisible: selectedTestCategory.isVisible,
        }),
      });
      const message = await response.json();

      if (!response.ok) {
        throw new Error(message.message);
      }

      alert("Cập nhật thành công");
      setIsEditing(false);
      setErrorMessageModal("");
      const tCategoryResponse = await authFetch(`${apiUrl}/api/TestCategory/get-all-test-category`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      const tCategoryData = await tCategoryResponse.json();
      setTestCategory(tCategoryData);
    } catch (error) {
      setErrorMessageModal(error.message);
    }
  };




  return (
    <div className="chapter-list-container">
      <h2>Danh Sách Loại Bài Kiểm Tra</h2>
      <div className="group-header">
        <div>
          <BackLink />
        </div>
        <button className="add-chapter" onClick={() => setShowAdd(true)}>Thêm loại bài kiểm tra mới</button>
      </div>
      {showAdd && (
        <div>
          <div className="add-chapter-form">
            <input
              type="text"
              placeholder="Nhập tên loại bài mới"
              value={newTestCategory}
              onChange={(e) => setNewTestCategory(e.target.value)}
            />
            <div className="action-buttons">
              <button onClick={handleAddTestCategory}>Thêm</button>
              <button onClick={() => setShowAdd(false)}>Hủy</button>
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
            <th style={{ width: "30%" }}>Tên loại bài</th>
            <th style={{ width: "20%" }}>Trạng thái</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {testCategory && testCategory.length > 0 ? (
            testCategory.map((t) => (
              <tr key={t.testCategoryId}>
                <td>{t.testCategoryId}</td>
                <td>{t.categoryName}</td>
                <td>{t.isVisible ? <span style={{ color: "green" }}>Hoạt động</span> : <span style={{ color: "red" }}>Không hoạt động</span>}</td>
                <td>
                  <button onClick={() => handleEdit(t)}>Chỉnh sửa</button>
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="4" style={{ textAlign: "center", fontStyle: "italic", color: "gray" }}>
                Chưa có loại bài kiểm tra nào.
              </td>
            </tr>
          )}

        </tbody>
      </table>
      {/* Form chỉnh sửa */}
      {isEditing && (
        <div className="modal">
          <div className="modal-content">
            <h3>Chỉnh sửa loại bài</h3>
            <label>
              Tên:
              <input
                type="text"
                value={selectedTestCategory?.categoryName}
                onChange={(e) =>
                  setSelectedTestCategory({ ...selectedTestCategory, categoryName: e.target.value })
                }
              />
            </label>
            <label>
              Trạng thái:
              <select
                value={selectedTestCategory?.isVisible}
                onChange={(e) =>
                  setSelectedTestCategory({
                    ...selectedTestCategory,
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

export default TestCategory;
