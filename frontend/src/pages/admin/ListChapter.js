import React, { useState, useEffect } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";
import BackLink from "../../components/BackLink";
import { fetchChapters, addChapter } from "../../services/ChapterService";
import './ListChapter.css';


const ListChapter = () => {
  const { gradeId, subjectId } = useParams();
  const [chapters, setChapters] = useState([]);
  const [newChapterTitle, setNewChapterTitle] = useState('');
  const [showAddChapter, setShowAddChapter] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [isEditing, setIsEditing] = useState(false);
  const [selectedChapter, setSelectedChapter] = useState(null);
  const [selectedSemester, setSelectedSemester] = useState(1);
  const [subjectgradeName, setSubjectGradeName] = useState("");
  const [subjectGradeId, setSubjectGradeId] = useState();
  const [chapterNameError, setChapterNameError] = useState("");

  const navigate = useNavigate();
  const roleId = sessionStorage.getItem('roleId');
  //get du lieu
  const loadChapters = async () => {
    const data = await fetchChapters(gradeId, subjectId);
    setChapters(data.chapters);
    setSubjectGradeName(data.gradeName + ' môn ' + data.subjectName);
    setSubjectGradeId(data.subjectGradeId);
  };

  useEffect(() => {
    loadChapters();
  }, [gradeId, subjectId]);

  //add new chapter
  const handleAddChapter = async () => {
    try {
      const result = await addChapter(subjectGradeId, newChapterTitle, selectedSemester);

      if (result.success) {
        setSuccessMessage(result.message);
        setNewChapterTitle('');
        setShowAddChapter(false);
        loadChapters();
      } else {
        setErrorMessage(result.message);
      }

      setTimeout(() => {
        setSuccessMessage('');
        setErrorMessage('');
      }, 3000);
    } catch (error) {
      console.error("Có lỗi khi thêm chương mới", error);
      setErrorMessage(error.message);
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
    if (!selectedChapter?.chapterName.trim()) {
      setChapterNameError("Tên chương không được để trống.");
      return;
    }

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
          semester: selectedChapter.semester
        }),
      });
      const message = await response.text();

      if (!response.ok) {
        throw new Error(message);
      }


      alert("Cập nhật thành công!");
      setIsEditing(false);
      setErrorMessage("");
      loadChapters();
    } catch (error) {
      setErrorMessage(error.message);
    }
  };


  return (
    <div className="chapter-list-container">
      <h2>Danh Sách Chương của {subjectgradeName}</h2>
      <div className="group-header">
        <div>
          <BackLink />
        </div>
        <div>
          <button className="add-chapter" onClick={() => setShowAddChapter(true)}>Thêm Chương Mới</button>
        </div>

      </div>
      {showAddChapter && (
        <div>
          <div className="add-chapter-form">
            <input
              type="text"
              placeholder="Nhập tên chương mới"
              value={newChapterTitle}
              onChange={(e) => setNewChapterTitle(e.target.value)}
            />
            <div className="semester-selection">
              <label>
                <input
                  type="radio"
                  value={1}
                  checked={selectedSemester === 1}
                  onChange={() => setSelectedSemester(1)}
                />
                Học kỳ 1
              </label>
              <label>
                <input
                  type="radio"
                  value={2}
                  checked={selectedSemester === 2}
                  onChange={() => setSelectedSemester(2)}
                />
                Học kỳ 2
              </label>
            </div>
            <div className="action-buttons">
              <button onClick={handleAddChapter}>Thêm</button>
              <button onClick={() => setShowAddChapter(false)}>Hủy</button>
            </div>
          </div>
          <div>
            <p className="instruction-text">* Vui lòng nhập tên chương theo định dạng: "Chương số: tên chương"</p>
            <p className="instruction-text">* Có thể nhập nhiều chương cùng một lúc, ví dụ: "Chương 1: ABC Chương 2: XYZ..."</p>
          </div>
        </div>

      )}
      {errorMessage && <div className="error-message">{errorMessage}</div>}
      {successMessage && <div className="success-message">{successMessage}</div>}
      <table className="chapter-table">
        <thead>
          <tr>
            <th style={{ width: "5%" }}>Chương</th>
            <th style={{ width: "40%" }}>Tên Chương</th>
            <th style={{ width: "15%" }}>Học kỳ</th>
            <th style={{ width: "15%" }}>Trạng thái</th>
            <th style={{ width: "25%" }}>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {chapters.length > 0 ? (chapters.map((chapter) => (
            <tr key={chapter.chapterId}>
              <td>{chapter.order}</td>
              <td>{chapter.chapterName}</td>
              <td>Học kỳ {chapter.semester}</td>
              <td>{chapter.isVisible ? <span style={{ color: "green" }}>Hoạt động</span> : <span style={{ color: "red" }}>Không hoạt động</span>}</td>
              <td>
                <button>
                  {roleId === "3" ? (
                    <Link to={`/admin/grades/${gradeId}/subject/${subjectId}/chapters/${chapter.chapterId}`}>
                      Xem bài học
                    </Link>
                  ) : (
                    <Link to={`/content_manage/grades/${gradeId}/subject/${subjectId}/chapters/${chapter.chapterId}`}>
                      Xem bài học
                    </Link>
                  )}
                </button>
                <button onClick={() => handleEdit(chapter)}>Chỉnh sửa</button>
              </td>
            </tr>
          ))
        ) : (
            <tr>
              <td colSpan="5" style={{ textAlign: "center", fontStyle: "italic", color: "gray" }}>
                Chưa có chương nào.
              </td>
            </tr>
          )}

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
                onChange={(e) =>
                  setSelectedChapter({ ...selectedChapter, order: e.target.value })
                }
              />
            </label>
            <label>
              Tên chương:
              <input
                type="text"
                value={selectedChapter?.chapterName}
                onChange={(e) => {
                  const value = e.target.value;
                  setSelectedChapter({ ...selectedChapter, chapterName: value });
                  
                  if (value.trim() === "") {
                    setChapterNameError("Tên chương không được để trống.");
                  } else {
                    setChapterNameError("");
                  }
                }}
              />
            </label>
            <label>Học kỳ:</label>
            <div className="semester-selection">
              <label>
                <input
                  type="radio"
                  value={1}
                  checked={selectedChapter?.semester === 1}
                  onChange={() => setSelectedChapter({ ...selectedChapter, semester: 1 })}
                />
                Học kỳ 1
              </label>
              <label>
                <input
                  type="radio"
                  value={2}
                  checked={selectedChapter?.semester === 2}
                  onChange={() => setSelectedChapter({ ...selectedChapter, semester: 2 })}
                />
                Học kỳ 2
              </label>
            </div>
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
            {chapterNameError && <p className="error-message">{chapterNameError}</p>}
          </div>
        </div>

      )}

    </div>
  );
};

export default ListChapter;
