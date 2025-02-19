import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import './ListLesson.css';
const ListLesson = () => {
  const { chapterId } = useParams();
  const [lessons, setLessons] = useState([]);
  const [showAddLesson, setShowAddLesson] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
   const [newLessonTitle, setNewLessonTitle] = useState('');

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
                <button>Chỉnh sửa</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default ListLesson;
