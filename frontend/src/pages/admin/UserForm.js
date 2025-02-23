import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import './UsersList.css'; // Import CSS

const UserForm = () => {
    const { userId } = useParams();
    const [user, setUser] = useState({ name: '', email: '' });
    const navigate = useNavigate();

    useEffect(() => {
        if (userId && userId !== 'new') {
            fetch(`https://your-api-endpoint.com/api/User/${userId}`)
                .then(response => response.json())
                .then(data => setUser(data))
                .catch(() => alert('Lỗi khi tải thông tin người dùng'));
        }
    }, [userId]);

    const handleSubmit = async (event) => {
        event.preventDefault();

        const method = userId === 'new' ? 'POST' : 'PUT';
        const url = userId === 'new' ? 'https://your-api-endpoint.com/api/User' : `https://your-api-endpoint.com/api/User/${userId}`;

        try {
            const response = await fetch(url, {
                method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(user),
            });

            if (!response.ok) throw new Error('Lỗi khi lưu thông tin');

            navigate('/admin/users');
        } catch (error) {
            alert(error.message);
        }
    };

    return (
        <div>
            <h2>{userId === 'new' ? 'Thêm Người Dùng' : 'Chỉnh Sửa Người Dùng'}</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Tên:</label>
                    <input type="text" value={user.name} onChange={(e) => setUser({ ...user, name: e.target.value })} required />
                </div>
                <div>
                    <label>Email:</label>
                    <input type="email" value={user.email} onChange={(e) => setUser({ ...user, email: e.target.value })} required />
                </div>
                <button type="submit" className="submit-btn">💾 Lưu</button>
                <button type="button" onClick={() => navigate('/admin/users')} className="delete-btn">❌ Hủy</button>
            </form>
        </div>
    );
};

export default UserForm;
