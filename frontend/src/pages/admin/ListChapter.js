import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import './ListChapter.css';


const ListChapter = () => {
  const [chapters, setChapters] = useState([]);
  const [newChapterTitle, setNewChapterTitle] = useState('');
  const [showAddChapter, setShowAddChapter] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [isEditing, setIsEditing] = useState(false);
  const [selectedChapter, setSelectedChapter] = useState(null);

  //get du lieu
  useEffect(() => {
    const fetchChapters = async () => {
      try {
        const response = await fetch('https://localhost:7259/api/Curriculum/get-all-chapter');
        const data = await response.json();
        setChapters(data);
      } catch (error) {
        console.error("Có lỗi khi lấy dữ liệu chương", error);
      }
    };

    fetchChapters();
  }, []);

  //add new chapter
  const handleAddChapter = async () => {
    if (!newChapterTitle.trim()) return;

    try {
      const response = await fetch('https://localhost:7259/api/Curriculum/add-chapters', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(newChapterTitle),
      });

      if (!response.ok) {
        const errorResponse = await response.text();
        const errorMessage = errorResponse ? errorResponse : 'Không thể thêm chương mới';
        throw new Error(errorMessage);
      }

      const chaptersResponse = await fetch('https://localhost:7259/api/Curriculum/get-all-chapter', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      const chaptersData = await chaptersResponse.json();
      setChapters(chaptersData);
      setNewChapterTitle('');
      setShowAddChapter(false);
      setErrorMessage('');
      setSuccessMessage('Chương đã được thêm thành công!');


      setTimeout(() => {
        setSuccessMessage('');
      }, 3000);
    } catch (error) {
      setErrorMessage(error.message);
      console.error("Có lỗi khi thêm chương mới", error);
    }
  };

  //edit chapter
  const handleEdit = (chapter) => {
    setSelectedChapter(chapter);
    setIsEditing(true);
  };

  const handleClose = () => {
    setIsEditing(false);
    setSelectedChapter(null);
  };

  const handleSave = async () => {
    try {
      const response = await fetch(`https://localhost:7259/api/Curriculum/edit-chapter/${selectedChapter.chapterId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          chapterId: selectedChapter.chapterId,
          order: selectedChapter.order,
          chapterName: selectedChapter.chapterName,
          isVisible: selectedChapter.isVisible,
        }),
      });
      const message = await response.text();

      if (!response.ok) {
        throw new Error(message);
      }


      alert("Cập nhật thành công!");
      setIsEditing(false);
      setErrorMessage("");
      const chaptersResponse = await fetch('https://localhost:7259/api/Curriculum/get-all-chapter', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      const chaptersData = await chaptersResponse.json();
      setChapters(chaptersData);
    } catch (error) {
      setErrorMessage(error.message);
    }
  };




  return (
    <div className="chapter-list-container">
      <h2>Danh Sách Chương</h2>
      <button className="add-chapter" onClick={() => setShowAddChapter(true)}>Thêm Chương Mới</button>
      {showAddChapter && (
        <div>
          <div className="add-chapter-form">
            <input
              type="text"
              placeholder="Nhập tên chương mới"
              value={newChapterTitle}
              onChange={(e) => setNewChapterTitle(e.target.value)}
            />
            <div className="action-buttons">
              <button onClick={handleAddChapter}>Thêm</button>
              <button onClick={() => setShowAddChapter(false)}>Hủy</button>
            </div>
          </div>
          <div>
            <p className="instruction-text">* Vui lòng nhập tên chương theo định dạng: "Chương X: Tên chương"</p>
            <p className="instruction-text">* Có thể nhập nhiều chương cùng một lúc, ví dụ: "Chương 1: ABC Chương 2: XYZ..."</p>
          </div>
        </div>

      )}
      {errorMessage && <div className="error-message">{errorMessage}</div>}
      {successMessage && <div className="success-message">{successMessage}</div>}
      <table className="chapter-table">
        <thead>
          <tr>
            <th style={{ width: "10%" }}>Chương</th>
            <th style={{ width: "50%" }}>Tên Chương</th>
            <th style={{ width: "20%" }}>Trạng thái</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {chapters.map((chapter) => (
            <tr key={chapter.chapterId}>
              <td>{chapter.order}</td>
              <td>{chapter.chapterName}</td>
              <td>{chapter.isVisible ? <span style={{ color: "green" }}>Hoạt động</span> : <span style={{ color: "red" }}>Không hoạt động</span>}</td>
              <td>
                <button>
                  <Link to={`/admin/listchapter/${chapter.chapterId}`}>Xem bài học</Link>
                </button>
                <button onClick={() => handleEdit(chapter)}>Chỉnh sửa</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      {/* Form chỉnh sửa */}
      {isEditing && (
        <div className="modal">
          <div className="modal-content">
            <h3>Chỉnh sửa chương</h3>
            <label>
              Chương:
              <input
                type="text"
                value={selectedChapter?.order}
                disabled
              />
            </label>
            <label>
              Tên chương:
              <input
                type="text"
                value={selectedChapter?.chapterName}
                onChange={(e) =>
                  setSelectedChapter({ ...selectedChapter, chapterName: e.target.value })
                }
              />
            </label>
            <label>
              Trạng thái:
              <select
                value={selectedChapter?.isVisible}
                onChange={(e) =>
                  setSelectedChapter({
                    ...selectedChapter,
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
            {errorMessage && <p className="error-message">{errorMessage}</p>}
          </div>
        </div>
      )}
      <style>
        {`
          .modal {
            position: fixed;
            top: 0; left: 0;
            width: 100%; height: 100%;
            background: rgba(0, 0, 0, 0.5);
            display: flex; justify-content: center; align-items: center;
          }
          .modal-content {
            width:50%;
            background: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
          }
          input, select {
            display: block;
            width: 100%;
            margin: 10px 0;
            padding: 5px;
            border-radius: 5px;
            border: 1px solid gray;
          }
            
        `}
      </style>

    </div>
  );
};

export default ListChapter;
