import React, { useState, useEffect } from "react";
import { useParams, Link } from "react-router-dom";
import "./AssignChapter.css";

const AssignChapter = () => {
  const { gradeId } = useParams();
  const [chapters, setChapters] = useState([]);
  const [selectedChapter, setSelectedChapter] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [contentManagers, setContentManagers] = useState([]);

  useEffect(() => {
    const fetchChapters = async () => {
      try {
        const response = await fetch(`https://localhost:7259/api/Curriculum/get-all-chapter`);
        if (!response.ok) {
          throw new Error("Lỗi khi tải dữ liệu chương");
        }
        const data = await response.json();
        setChapters(data);
      } catch (error) {
        console.error("Có lỗi khi lấy dữ liệu chương", error);
        setErrorMessage("Không thể tải danh sách chương.");
      }
    };

    fetchChapters();
  }, []);
  useEffect(() => {
    const fetchContentManagers = async () => {
      try {
        const response = await fetch("https://localhost:7259/api/User/get-user-by-roleId/4");
        if (!response.ok) {
          throw new Error("Không thể tải danh sách người quản lý nội dung.");
        }
        const data = await response.json();
        setContentManagers(data);
      } catch (error) {
        console.error("Lỗi khi lấy danh sách người quản lý nội dung:", error);
      }
    };
  
    fetchContentManagers();
  }, []);
  const openModal = (chapter) => {
    setSelectedChapter({
      chapterId: chapter.chapterId, 
      chapterName: chapter.chapterName,
      userId: chapter.userId || "", 
      userName: chapter.userName || ""
    });
    setShowModal(true);
  };

  const closeModal = () => {
    setShowModal(false);
    setSelectedChapter(null);
  };

  const handleSave = async () => {
    if (!selectedChapter) return;
  
    try {
      const response = await fetch(`https://localhost:7259/api/Curriculum/assign-content-managers`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json-patch+json",
        },
        body: JSON.stringify({
          assignments: [
            {
              chapterId: selectedChapter.chapterId,
              userId: selectedChapter.userId,
            }
          ]
        }),
      });
  
      if (!response.ok) {
        throw new Error("Lỗi khi cập nhật chương học");
      }
  
      // ✅ Cập nhật danh sách chương ngay sau khi lưu
      setChapters((prevChapters) =>
        prevChapters.map((ch) =>
          ch.chapterId === selectedChapter.chapterId
            ? { 
                ...ch, 
                userId: selectedChapter.userId, 
                userName: contentManagers.find(user => user.userId === selectedChapter.userId)?.userName || "Chưa có"
              }
            : ch
        )
      );
  
      setSuccessMessage("Cập nhật thành công!");
      setTimeout(() => {
        setSuccessMessage("");
        closeModal();
      }, 1500);
    } catch (error) {
      console.error("Có lỗi khi cập nhật chương học", error);
      setErrorMessage("Không thể cập nhật chương học.");
    }
  };
  
  

  return (
    <div className="chapter-list-container">
      <h2>Danh sách Chương Học</h2>
      <div className="group-header">
        <div>
          <Link className="backlink" to="/admin">Trang chủ</Link> / Chương
        </div>
      </div>

      {errorMessage && <div className="error-message">{errorMessage}</div>}
      {successMessage && <div className="success-message">{successMessage}</div>}

      <table className="chapter-table">
        <thead>
          <tr>
            <th>Chương</th>
            <th>Tên Chương</th>
            <th>Học kỳ</th>
            <th>Trạng thái</th>
            <th>Người quản lý</th>
            <th>Khối lớp</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {chapters.map((chapter) => (
            <tr key={chapter.chapterId}>
              <td>{chapter.order}</td>
              <td>{chapter.chapterName}</td>
              <td>Học kỳ {chapter.semester}</td>
              <td>
                {chapter.isVisible ? 
                  <span style={{ color: "green" }}>Hoạt động</span> : 
                  <span style={{ color: "red" }}>Không hoạt động</span>}
              </td>
              <td>{chapter.userName ? chapter.userName : "Chưa có"}</td>
              <td>{chapter.gradeName ? chapter.gradeName : "Chưa có"}</td>
              <td>
                <button onClick={() => openModal(chapter)}>Phân công</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Modal */}
      {showModal && selectedChapter && (
        <div className="modal">
          <div className="modal-content">
            <h2>Phân công Chương</h2>
            <label>Tên Chương:</label>
            <input
              type="text"
              value={selectedChapter.chapterName}
              onChange={(e) => setSelectedChapter({ ...selectedChapter, chapterName: e.target.value })}
              readOnly
            />
            
<label>Người Quản Lý:</label>
<select
  value={selectedChapter.userId || ""}
  onChange={(e) =>
    setSelectedChapter({ ...selectedChapter, userId: parseInt(e.target.value, 10) })
  }
>

  {contentManagers.map((user) => (
    <option key={user.userId} value={user.userId}>
      {user.userName}
    </option>
  ))}
</select>




            <div className="button-group">
              <button onClick={handleSave}>Lưu</button>
              <button onClick={closeModal}>Hủy</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AssignChapter;
