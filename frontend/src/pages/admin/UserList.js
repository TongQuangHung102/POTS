import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';

import './UsersList.css';

const UsersList = () => {
    const [users, setUsers] = useState([]);
    const [roles, setRoles] = useState([]);
    const [errorMessage, setErrorMessage] = useState('');
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [role, setRole] = useState('');
    const [email, setEmail] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const params = { page, pageSize };
                if (role) {
                    const selectedRole = roles.find(r => r.roleName === role);
                    if (selectedRole) {
                        params.roleId = selectedRole.roleId; // Sử dụng roleId nếu API yêu cầu
                    }
                }
                if (email) params.email = email;

                const response = await axios.get('https://localhost:7259/api/User/get-all-user', { params });
                setUsers(Array.isArray(response.data.data) ? response.data.data : []);
            } catch (error) {
                setErrorMessage(`Lỗi khi tải danh sách người dùng: ${error.response?.data?.message || error.message}`);
                setUsers([]);
            }
        };

        const fetchRoles = async () => {
            try {
                const response = await axios.get('https://localhost:7259/api/Role/get-all-role');
                setRoles(Array.isArray(response.data) ? response.data : []);
            } catch (error) {
                setErrorMessage(`Lỗi khi tải danh sách vai trò: ${error.response?.data?.message || error.message}`);
            }
        };

        fetchUsers();
        fetchRoles();
    }, [page, pageSize, role, email]);

    const handleUpdateUser = async (userId, updatedFields) => {
        try {
            const user = users.find(user => user.userId === userId);
            if (!user) return;

            const payload = {
                userName: user.userName,
                email: user.email,
                role: user.role || 1, // Nếu role bị null, đặt giá trị mặc định
                isActive: updatedFields.isActive ?? user.isActive,
            };

            if ('roleName' in updatedFields) {
                const selectedRole = roles.find(r => r.roleName === updatedFields.roleName);
                if (selectedRole) payload.role = selectedRole.roleId;
            }

            await axios.put(`https://localhost:7259/api/User/edit-user/${userId}`, payload, {
                headers: { 'Content-Type': 'application/json' }
            });
            setUsers(users.map(user => user.userId === userId ? { ...user, ...updatedFields, role: payload.role } : user));
        } catch (error) {
            setErrorMessage(`Lỗi khi cập nhật người dùng: ${error.response?.data?.message || error.message}`);
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
                    value={role}
                    onChange={(e) => setRole(e.target.value)}
                >
                    <option value="">Chọn vai trò</option>
                    {roles.map(role => (
                        <option key={role.roleId} value={role.roleName}>{role.roleName}</option>
                    ))}
                </select>

            </div>
            <div>
                <button className="add-user-btn" onClick={() => navigate('/admin/add-user')}>
                    ➕ Thêm User
                </button>

            </div>
            <table className="users-table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Tên</th>
                        <th>Email</th>
                        <th>Trạng thái</th>
                        <th>Vai trò</th>
                    </tr>
                </thead>
                <tbody>
                    {Array.isArray(users) && users.length > 0 ? (
                        users.map(user => (
                            <tr key={user.userId}>
                                <td>{user.userId}</td>
                                <td>{user.userName}</td>
                                <td>{user.email}</td>
                                <td>
                                    <span className={user.isActive ? "active" : "inactive"}>
                                        {user.isActive ? '🟢 Đang hoạt động' : '🔴 Đã bị khóa'}
                                    </span>
                                    <button onClick={() => handleUpdateUser(user.userId, { isActive: !user.isActive })}>
                                        {user.isActive ? '🔒 Khóa người dùng' : '✅ Mở khóa người dùng'}
                                    </button>
                                </td>
                                <td>
                                    <select value={user.roleName || ''} onChange={(e) => handleUpdateUser(user.userId, { roleName: e.target.value })}>
                                        {roles.map(role => (
                                            <option key={role.roleId} value={role.roleName}>{role.roleName}</option>
                                        ))}
                                    </select>
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
