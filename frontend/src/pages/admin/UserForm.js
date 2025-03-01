import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import './UsersList.css'; // Import CSS

const UserForm = () => {
    const { userId } = useParams();
    const [user, setUser] = useState({ name: '', email: '', roleId: '' });  // Thêm roleId cho việc chọn vai trò
    const [roles, setRoles] = useState([]);  // Lưu các vai trò có sẵn
    const navigate = useNavigate();

    // Lấy danh sách vai trò từ API
    useEffect(() => {
        fetch('https://localhost:7259/api/roles')  // Điều chỉnh API lấy danh sách vai trò
            .then(response => response.json())
            .then(data => setRoles(data))
            .catch(() => alert('Lỗi khi tải danh sách vai trò'));
    }, []);

    useEffect(() => {
        if (userId && userId !== 'new') {
            fetch(`https://localhost:7259/api/User/get-user-by/${userId}`)  // Sửa lại API để lấy thông tin người dùng theo ID
                .then(response => response.json())
                .then(data => setUser(data))
                .catch(() => alert('Lỗi khi tải thông tin người dùng'));
        }
    }, [userId]);

    const handleSubmit = async (event) => {
        event.preventDefault();

        const method = userId === 'new' ? 'POST' : 'PUT';
        const url = userId === 'new' ? 'https://localhost:7259/api/User/add-user' : `https://localhost:7259/api/User/edit-user/${userId}`;  // Sửa API cho POST và PUT

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
                    <input
                        type="text"
                        value={user.name}
                        onChange={(e) => setUser({ ...user, name: e.target.value })}
                        required
                    />
                </div>
                <div>
                    <label>Email:</label>
                    <input
                        type="email"
                        value={user.email}
                        onChange={(e) => setUser({ ...user, email: e.target.value })}
                        required
                    />
                </div>
                <div>
                    <label>Vai trò:</label>
                    <select
                        value={user.roleId}
                        onChange={(e) => setUser({ ...user, roleId: e.target.value })}
                        required
                    >
                        <option value="">Chọn vai trò</option>
                        {roles.map(role => (
                            <option key={role.id} value={role.id}>
                                {role.name}
                            </option>
                        ))}
                    </select>
                </div>
                <button type="submit" className="submit-btn">💾 Lưu</button>
                <button type="button" onClick={() => navigate('/admin/users')} className="delete-btn">❌ Hủy</button>
            </form>
        </div>
    );
};

export default UserForm;
