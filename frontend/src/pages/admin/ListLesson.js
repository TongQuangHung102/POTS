import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import './ListLesson.css';
const ListLesson = () => {
  const {chapterId } = useParams();
  const [lessons, setLessons] = useState([]);
  const [showAddLesson, setShowAddLesson] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [newLessonTitle, setNewLessonTitle] = useState('');
  const [isEditing, setIsEditing] = useState(false);
  const [selectedLesson, setSelectedLesson] = useState(null);

  useEffect(() => {
    const fetchLessons = async () => {
      try {
        const response = await fetch(`https://localhost:7259/api/Curriculum/get-lesson-by-chapterId?chapterId=${chapterId}`);
        const data = await response.json();
        setLessons(data);
      } catch (error) {
        console.error("Có lỗi khi lấy bài học", error);
      }
    };

    fetchLessons();
  }, [chapterId]);

  const handleAddLesson = async () => {
    if (!newLessonTitle.trim()) return;

    try {
      const response = await fetch('https://localhost:7259/api/Curriculum/add-lessons', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          chapterId: chapterId, 
          input: newLessonTitle
        })
      });

      if (!response.ok) {
        const errorResponse = await response.text();
        const errorMessage = errorResponse ? errorResponse : 'Không thể thêm bài mới';
        throw new Error(errorMessage);
      }

      const lessonsResponse = await fetch(`https://localhost:7259/api/Curriculum/get-lesson-by-chapterId?chapterId=${chapterId}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      const lessons = await lessonsResponse.json();
      setLessons(lessons);
      setNewLessonTitle('');
      setShowAddLesson(false);
      setErrorMessage('');
      setSuccessMessage('Bài đã được thêm thành công!');


      setTimeout(() => {
        setSuccessMessage('');
      }, 3000);
    } catch (error) {
      setErrorMessage(error.message);
      console.error("Có lỗi khi thêm chương mới", error);
    }
  };

  const handleEdit = (lesson) => {
    setSelectedLesson(lesson);
    setIsEditing(true);
  };

  const handleClose = () => {
    setIsEditing(false);
    setSelectedLesson(null);
  };

  const handleSave = async () => {
    try {
      const response = await fetch(`https://localhost:7259/api/Curriculum/edit-lesson/${selectedLesson.lessonId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          lessonId: selectedLesson.lessonId,
          order: selectedLesson.order,
          lessonName: selectedLesson.lessonName,
          isVisible: selectedLesson.isVisible,
        }),
      });
      const message = await response.text();

      if (!response.ok) {
        throw new Error(message);
      }


      alert("Cập nhật thành công!");
      setIsEditing(false);
      setErrorMessage("");
      const lessonsResponse = await fetch(`https://localhost:7259/api/Curriculum/get-lesson-by-chapterId?chapterId=${chapterId}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      const lessonData = await lessonsResponse.json();
      setLessons(lessonData);
    } catch (error) {
      setErrorMessage(error.message);
    }
  };

  return (
    <div className="lesson-container">
      <h2>Danh sách bài học</h2>

      <button className="add-chapter" onClick={() => setShowAddLesson(true)}>Thêm Bài Mới</button>
      {showAddLesson && (
        <div>
          <div className="add-lesson-form">
            <input
              type="text"
              placeholder="Nhập tên bài mới"
              value={newLessonTitle}
              onChange={(e) => setNewLessonTitle(e.target.value)}
            />
            <div className="action-buttons">
              <button onClick={handleAddLesson}>Thêm</button>
              <button onClick={() => setShowAddLesson(false)}>Hủy</button>
            </div>
          </div>
          <div>
            <p className="instruction-text">* Vui lòng nhập tên bài theo định dạng: "Bài X: Tên bài"</p>
            <p className="instruction-text">* Có thể nhập nhiều bài cùng một lúc, ví dụ: "Bài 1: ABC Bài 2: XYZ..."</p>
          </div>
        </div>

      )}
      {errorMessage && <div className="error-message">{errorMessage}</div>}
      {successMessage && <div className="success-message">{successMessage}</div>}

      <table className="lesson-table">
        <thead>
          <tr>
            <th style={{ width: "10%" }}>Bài</th>
            <th style={{ width: "50%" }}>Tên bài học</th>
            <th style={{ width: "20%" }}>Trạng thái</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {lessons.map(lesson => (
            <tr key={lesson.lessonId}>
              <td>{lesson.order}</td>
              <td>{lesson.lessonName}</td>
              <td>{lesson.isVisible ? <span style={{ color: "green" }}>Hoạt động</span> : <span style={{ color: "red" }}>Không hoạt động</span>}</td>
              <td>
                <button>
                  Xem
                </button>
                <button onClick={() => handleEdit(lesson)}>Chỉnh sửa</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Form chỉnh sửa */}
      {isEditing && (
        <div className="modal">
          <div className="modal-content">
            <h3>Chỉnh sửa bài</h3>
            <label>
              Bài:
              <input
                type="text"
                value={selectedLesson?.order}
                disabled
              />
            </label>
            <label>
              Tên Bài:
              <input
                type="text"
                value={selectedLesson?.lessonName}
                onChange={(e) =>
                  setSelectedLesson({ ...selectedLesson, lessonName: e.target.value })
                }
              />
            </label>
            <label>
              Trạng thái:
              <select
                value={selectedLesson?.isVisible}
                onChange={(e) =>
                  setSelectedLesson({
                    ...selectedLesson,
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

export default ListLesson;
