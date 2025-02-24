import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './UsersList.css'; // Import file CSS

const UsersList = () => {
    const [users, setUsers] = useState([]);
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        fetch('https://localhost:7259/api/User/List')
            .then(response => response.json())
            .then(data => setUsers(data))
            .catch(error => setErrorMessage('Lỗi khi tải danh sách người dùng!'));
    }, []);

    const handleDeleteUser = async (userId) => {
        if (!window.confirm('Bạn có chắc chắn muốn xóa người dùng này?')) return;

        try {
            await fetch(`https://localhost:7259/api/User/${userId}`, { method: 'DELETE' });
            setUsers(users.filter(user => user.id !== userId));
        } catch (error) {
            setErrorMessage('Lỗi khi xóa người dùng!');
        }
    };

    return (
        <div>
            <h2>Danh sách Người Dùng</h2>
            {errorMessage && <div className="error">{errorMessage}</div>}
            <button onClick={() => navigate('/admin/user/new')} className="add-button">➕ Thêm Người Dùng</button>
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Tên</th>
                        <th>Email</th>
                        <th>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map(user => (
                        <tr key={user.id}>
                            <td>{user.id}</td>
                            <td>{user.name}</td>
                            <td>{user.email}</td>
                            <td>
                                <button className="edit-btn" onClick={() => navigate(`/admin/user/${user.id}`)}>✏️ Chỉnh sửa</button>
                                <button className="delete-btn" onClick={() => handleDeleteUser(user.id)}>🗑️ Xóa</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default UsersList;
