import React, { useState, useEffect } from "react";
import './ListChapter.css';


const ListChapter = () => {
  const [chapters, setChapters] = useState([]);
  const [newChapterTitle, setNewChapterTitle] = useState('');
  const [showAddChapter, setShowAddChapter] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const handleView = (id) => {
    console.log(`Xem chương với ID: ${id}`);
  };

  useEffect(() => {
    const fetchChapters = async () => {
      try {
        const response = await fetch('https://localhost:7259/api/Curriculum/get-all-chapter');  // Thay thế 'API_URL' bằng URL API của bạn
        const data = await response.json();  // Chuyển đổi phản hồi thành JSON
        setChapters(data);  // Giả sử response trả về mảng các chương
      } catch (error) {
        console.error("Có lỗi khi lấy dữ liệu chương", error);
      }
    };

    fetchChapters();
  }, []);

  const handleAddChapter = async () => {
    if (!newChapterTitle.trim()) return;  // Kiểm tra nếu tên chương trống

    try {
      const response = await fetch('https://localhost:7259/api/Curriculum/add-chapters', {  // Thay thế 'API_URL_ADD' bằng URL API thêm chương
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(newChapterTitle),  // Gửi tên chương dưới dạng chuỗi
      });

      if (!response.ok) {
        const errorResponse = await response.text();  // Đọc nội dung lỗi từ phản hồi
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
      setChapters(chaptersData);  // Cập nhật lại danh sách chương trong state
      setNewChapterTitle('');
      setShowAddChapter(false);
      setErrorMessage('');
      setSuccessMessage('Chương đã được thêm thành công!');

    // Sau 3 giây, ẩn thông báo thành công
    setTimeout(() => {
      setSuccessMessage('');
    }, 3000);
    } catch (error) {
      setErrorMessage(error.message);  // Cập nhật thông báo lỗi
      console.error("Có lỗi khi thêm chương mới", error);
    }
  };

  const handleEdit = (id) => {
    console.log(`Chỉnh sửa chương với ID: ${id}`);
  };

  const handleDelete = (id) => {
    setChapters(chapters.filter(chapter => chapter.id !== id));
    console.log(`Xóa chương với ID: ${id}`);
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
      <ul>
        {chapters.map(chapter => (
          <li key={chapter.chapterId} className="chapter-item">
            <span>Chương {chapter.order}: {chapter.chapterName}</span>
            <div className="action-buttons">
              <button onClick={() => handleView(chapter.id)}>Xem</button>
              <button onClick={() => handleEdit(chapter.id)}>Chỉnh sửa</button>
              <button onClick={() => handleDelete(chapter.id)}>Xóa</button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ListChapter;
