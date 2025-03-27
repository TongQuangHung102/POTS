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
    const [selectedUser, setSelectedUser] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const params = { page, pageSize };
                if (role) {
                    const selectedRole = roles.find(r => r.roleName === role);
                    if (selectedRole) {
                        params.roleId = selectedRole.roleId;
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
                userName: updatedFields.userName || user.userName,
                email: updatedFields.email || user.email,
                role: updatedFields.roleId || user.roleId,
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
            setIsModalOpen(false); // Đóng modal sau khi lưu
        } catch (error) {
            setErrorMessage(`Lỗi khi cập nhật người dùng: ${error.response?.data?.message || error.message}`);
        }
    };

    const openModal = (user) => {
        setSelectedUser(user);  // Lưu thông tin người dùng đang được chỉnh sửa
        setIsModalOpen(true);    // Mở modal
    };

    const closeModal = () => {
        setIsModalOpen(false);  // Đóng modal
        setSelectedUser(null);   // Xóa thông tin người dùng đang chỉnh sửa
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
                        <th>Hành động</th>
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
                                </td>
                                <td>{user.roleName}</td>
                                <td>
                                    <button onClick={() => openModal(user)}>
                                        Chỉnh sửa
                                    </button>
                                </td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="6">Không có dữ liệu</td>
                        </tr>
                    )}
                </tbody>
            </table>

            {isModalOpen && selectedUser && (
                <div className="modal">
                    <div className="modal-content">
                        <span className="close-btn" onClick={closeModal}>×</span>
                        <h3>Chỉnh sửa thông tin người dùng</h3>
                        <form onSubmit={(e) => { e.preventDefault(); handleUpdateUser(selectedUser.userId, selectedUser); }}>
                            <div>
                                <label>Tên người dùng</label>
                                <input
                                    type="text"
                                    value={selectedUser.userName}
                                    onChange={(e) => setSelectedUser({ ...selectedUser, userName: e.target.value })}
                                />
                            </div>
                            <div>
                                <label>Email</label>
                                <input
                                    type="email"
                                    value={selectedUser.email}
                                    onChange={(e) => setSelectedUser({ ...selectedUser, email: e.target.value })}
                                />
                            </div>
                            <div>
                                <label>Vai trò</label>
                                <select
                                    value={selectedUser.roleName}
                                    onChange={(e) => setSelectedUser({ ...selectedUser, roleName: e.target.value })}
                                >
                                    {roles.map(role => (
                                        <option key={role.roleId} value={role.roleName}>{role.roleName}</option>
                                    ))}
                                </select>
                            </div>
                            <div>
                                <label>Trạng thái</label>
                                <select
                                    value={selectedUser.isActive}
                                    onChange={(e) => setSelectedUser({ ...selectedUser, isActive: e.target.value === 'true' })}
                                >
                                    <option value={true}>Đang hoạt động</option>
                                    <option value={false}>Đã bị khóa</option>
                                </select>
                            </div>
                            <div>
                                <button type="submit">Lưu thay đổi</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
};

export default UsersList;
