import React, { useState, useEffect } from "react";
import { useParams, Link } from "react-router-dom";
import BackLink from "../../components/BackLink";
import { fetchLessons } from "../../services/LessonService";
import { authFetch } from "../../utils/authFetch";
import { normalizeName } from "../../utils/validator";
import './ListLesson.css';
const ListLesson = () => {
  const { gradeId, subjectId, chapterId } = useParams();
  const [lessons, setLessons] = useState([]);
  const [showAddLesson, setShowAddLesson] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [errorMessageModal, setErrorMessageModal] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [newLessonTitle, setNewLessonTitle] = useState('');
  const [isEditing, setIsEditing] = useState(false);
  const [selectedLesson, setSelectedLesson] = useState(null);
  const [chapterName, setChapterName] = useState('');

  const roleId = localStorage.getItem('roleId');
  const apiUrl = process.env.REACT_APP_API_URL;

  const loadLessons = async () => {
    const data = await fetchLessons(chapterId);
    setLessons(data.lessons);
    setChapterName(data.chapterName);
  };

  useEffect(() => {
    loadLessons();
  }, [chapterId]);

  useEffect(() => {
    setTimeout(() => {
      setErrorMessage('');
      setSuccessMessage('');
    }, 3000);
  }, [errorMessage, successMessage]);

  const handleAddLesson = async () => {
    if (!newLessonTitle.trim()) {
      setErrorMessage("Tên bài không được bỏ trống");
      return;
    }

    try {
      const response = await authFetch(`${apiUrl}/api/Curriculum/add-lessons`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          chapterId: chapterId,
          input: normalizeName(newLessonTitle)
        })
      });

      if (!response.ok) {
        const errorResponse = await response.text();
        const errorMessage = errorResponse ? errorResponse : 'Không thể thêm bài mới';
        throw new Error(errorMessage);
      }

      loadLessons();
      setNewLessonTitle('');
      setShowAddLesson(false);
      setErrorMessage('');
      setSuccessMessage('Bài đã được thêm thành công');


      setTimeout(() => {
        setSuccessMessage('');
        setErrorMessage('');
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
    if (!selectedLesson.lessonName.trim()) {
      setErrorMessageModal("Tên bài và thứ tự bài không được bỏ trống");
      return;
    }

    if (!selectedLesson?.order) {
      setErrorMessageModal("Tên bài và thứ tự bài không được bỏ trống");
      return;
    }

    if (!selectedLesson?.order || isNaN(selectedLesson.order) || selectedLesson.order <= 0) {
      setErrorMessageModal("Thứ tự bài phải là số và lớn hơn 0");
      return;
    }
    try {
      const response = await authFetch(`${apiUrl}/api/Curriculum/edit-lesson/${selectedLesson.lessonId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          lessonId: selectedLesson.lessonId,
          order: selectedLesson.order,
          lessonName: normalizeName(selectedLesson.lessonName),
          isVisible: selectedLesson.isVisible,
        }),
      });
      const message = await response.text();

      if (!response.ok) {
        throw new Error(message);
      }

      alert("Cập nhật thành công");
      setIsEditing(false);
      setErrorMessageModal("");
      loadLessons();
    } catch (error) {
      setErrorMessageModal(error.message);
    }
  };

  return (
    <div className="lesson-container">
      <h2>Danh sách bài học {chapterName}</h2>
      <div className="group-header">
        <div> <BackLink></BackLink></div>
        <button className="add-chapter" onClick={() => setShowAddLesson(true)}>Thêm bài mới</button>
      </div>
      {showAddLesson && (
        <div>
          <div className="add-lesson-form">
            <input
              type="text"
              placeholder="Nhập tên bài mới"
              value={newLessonTitle}
              onChange={(e) => setNewLessonTitle(e.target.value)}
              required
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
          {lessons.length > 0 ? (
            lessons.map(lesson => (
              <tr key={lesson.lessonId}>
                <td>{lesson.order}</td>
                <td>{lesson.lessonName}</td>
                <td>
                  {lesson.isVisible ? (
                    <span style={{ color: "green" }}>Hoạt động</span>
                  ) : (
                    <span style={{ color: "red" }}>Không hoạt động</span>
                  )}
                </td>
                <td>
                  <button>
                      <Link
                        to={`/content_manage/grades/${gradeId}/subject/${subjectId}/chapters/${chapterId}/lessons/${lesson.lessonId}/questions`}
                      >
                        Quản lý câu hỏi
                      </Link>
                  </button>
                  <button onClick={() => handleEdit(lesson)}>Chỉnh sửa</button>
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="4" style={{ textAlign: "center", fontStyle: "italic", color: "gray" }}>
                Chưa có bài học nào.
              </td>
            </tr>
          )}
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
                onChange={(e) =>
                  setSelectedLesson({ ...selectedLesson, order: e.target.value })
                }
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
                required
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
            {errorMessageModal && <p className="error-message">{errorMessageModal}</p>}
          </div>
        </div>
      )}
    </div>
  );
};

export default ListLesson;
