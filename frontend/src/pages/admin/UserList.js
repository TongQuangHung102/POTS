import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './UsersList.css';

const UsersList = () => {
    const [users, setUsers] = useState([]);
    const [errorMessage, setErrorMessage] = useState('');
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [roleId, setRoleId] = useState('');
    const [email, setEmail] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const params = new URLSearchParams();
                if (roleId) params.append("roleId", roleId);
                if (email) params.append("email", email);
                params.append("page", page);
                params.append("pageSize", pageSize);

                const response = await fetch(`https://localhost:7259/api/User/get-all-user?${params.toString()}`);

                if (!response.ok) {
                    throw new Error('Lỗi từ server!');
                }

                const data = await response.json();
                setUsers(Array.isArray(data.data) ? data.data : []);
            } catch (error) {
                setErrorMessage('Lỗi khi tải danh sách người dùng!');
                setUsers([]);
            }
        };

        fetchUsers();
    }, [page, pageSize, roleId, email]);

    const handleDeleteUser = async (userId) => {
        if (!window.confirm('Bạn có chắc chắn muốn xóa người dùng này?')) return;

        try {
            const response = await fetch(`https://localhost:7259/api/User/delete-user/${userId}`, { method: 'DELETE' });

            if (!response.ok) {
                throw new Error('Lỗi khi xóa người dùng!');
            }

            setUsers(users.filter(user => user.id !== userId));
        } catch (error) {
            setErrorMessage('Lỗi khi xóa người dùng!');
        }
    };

    return (
        <div className="users-container adjusted-margin">
            <h2 className="title">Danh sách Người Dùng</h2>
            {errorMessage && <div className="error">{errorMessage}</div>}

            <div className="search-filter">
                <input
                    type="text"
                    placeholder="Tìm kiếm theo email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />
                <select
                    value={roleId}
                    onChange={(e) => setRoleId(e.target.value)}
                >
                    <option value="">Chọn vai trò</option>
                    <option value="1">Quản trị viên</option>
                    <option value="2">Người dùng</option>
                </select>
            </div>

            <button onClick={() => navigate('/admin/user/new')} className="add-button">➕ Thêm Người Dùng</button>

            <div className="pagination">
                <button onClick={() => setPage(page - 1)} disabled={page === 1}>Trước</button>
                <span>Trang {page}</span>
                <button onClick={() => setPage(page + 1)}>Sau</button>
            </div>

            <table className="users-table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Tên</th>
                        <th>Email</th>
                        <th>Vai trò</th>
                        <th>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {Array.isArray(users) && users.length > 0 ? (
                        users.map(user => (
                            <tr key={user.id}>
                                <td>{user.id}</td>
                                <td>{user.userName}</td>
                                <td>{user.email}</td>
                                <td>{user.role}</td>
                                <td>
                                    <button className="edit-btn" onClick={() => navigate(`/admin/user/edit/${user.id}`)}>✏️ Chỉnh sửa</button>
                                    <button className="delete-btn" onClick={() => handleDeleteUser(user.id)}>🗑️ Xóa</button>
                                </td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="5">Không có dữ liệu</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default UsersList;
