import React, { useState } from "react";
import './ListChapter.css';


const ListChapter = () => {
    const [chapters, setChapters] = useState([
        { id: 1, title: 'Chương 1: Khởi đầu' },
        { id: 2, title: 'Chương 2: Phát triển' },
        { id: 3, title: 'Chương 3: Kết thúc' },
      ]);
      const [newChapterTitle, setNewChapterTitle] = useState('');
      const [showAddChapter, setShowAddChapter] = useState(false);
      const handleView = (id) => {
        console.log(`Xem chương với ID: ${id}`);
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
          <button onClick={() => setShowAddChapter(true)}>Thêm Chương Mới</button>
      {showAddChapter && (
        <div className="add-chapter-form">
          <input
            type="text"
            placeholder="Nhập tên chương mới"
          />
          <button>Thêm</button>
          <button onClick={() => setShowAddChapter(false)}>Hủy</button>
        </div>
      )}
          <ul>
            {chapters.map(chapter => (
              <li key={chapter.id} className="chapter-item">
                <span>{chapter.title}</span>
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
