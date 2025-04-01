import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './UsersList.css';
import { Edit, Search, UserPlus, AlertCircle, ChevronLeft, ChevronRight } from 'lucide-react';

const UsersList = () => {
  const [users, setUsers] = useState([]);
  const [roles, setRoles] = useState([]);
  const [errorMessage, setErrorMessage] = useState('');
  const [page, setPage] = useState(1);
  const [pageSize] = useState(5);
  const [role, setRole] = useState('');
  const [email, setEmail] = useState('');
  const [selectedUser, setSelectedUser] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const navigate = useNavigate();

  useEffect(() => {
    const fetchUsers = async () => {
      setIsLoading(true);
      try {
        const params = { page, pageSize };
        if (role && role !== 'all') {
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
      } finally {
        setIsLoading(false);
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

      setUsers(users.map(user =>
        user.userId === userId
          ? { ...user, ...updatedFields, role: payload.role }
          : user
      ));
      setIsModalOpen(false);
    } catch (error) {
      setErrorMessage(`Lỗi khi cập nhật người dùng: ${error.response?.data?.message || error.message}`);
    }
  };

  const openModal = (user) => {
    setSelectedUser(user);
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setSelectedUser(null);
  };

  return (
    <div className="users-container">
      <div className="card">
        <div className="card-header">
          <h2 className="card-title">Danh sách Người Dùng</h2>
          <button
            className="add-user-btn"
            onClick={() => navigate('/admin/add-user')}
          >
            <UserPlus size={16} />
            Thêm User
          </button>
        </div>
        <div className="card-content">
          {errorMessage && (
            <div className="alert alert-error">
              <AlertCircle size={16} />
              <span>{errorMessage}</span>
            </div>
          )}

          {/* Filters - tìm kiếm và vai trò */}
          <div className="filters">
            <div className="search-container horizontal">
              <Search className="search-icon-outside" size={16} />
              <input
                type="text"
                placeholder="Tìm kiếm theo email"
                className="search-input"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </div>

            <select
              value={role}
              onChange={(e) => setRole(e.target.value)}
              className="select-input"
            >
              <option value="all">Tất cả vai trò</option>
              {roles.map((role) => (
                <option key={role.roleId} value={role.roleName}>
                  {role.roleName}
                </option>
              ))}
            </select>
          </div>

          {/* Table */}
          <div className="table-container">
            <table className="users-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Tên</th>
                  <th>Email</th>
                  <th>Trạng thái</th>
                  <th>Vai trò</th>
                  <th className="action-column">Hành động</th>
                </tr>
              </thead>
              <tbody>
                {isLoading ? (
                  <tr>
                    <td colSpan={6} className="loading-cell">
                      <div className="spinner"></div>
                    </td>
                  </tr>
                ) : users.length > 0 ? (
                  users.map(user => (
                    <tr key={user.userId}>
                      <td className="user-id">{user.userId}</td>
                      <td>{user.userName}</td>
                      <td>{user.email}</td>
                      <td>
                        <span className={`status-badge ${user.isActive ? "status-active" : "status-inactive"}`}>
                          {user.isActive ? '🟢 Đang hoạt động' : '🔴 Đã bị khóa'}
                        </span>
                      </td>
                      <td>
                        <span className="role-badge">{user.roleName}</span>
                      </td>
                      <td className="action-column">
                        <button
                          className="edit-btn"
                          onClick={() => openModal(user)}
                        >
                          <Edit size={14} />
                          Chỉnh sửa
                        </button>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan={6} className="empty-data">
                      Không có dữ liệu
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>

          <div className="pagination">
            <button
              className="pagination-btn"
              onClick={() => setPage(prev => Math.max(prev - 1, 1))}
              disabled={page === 1 || isLoading}
            >
              <ChevronLeft size={16} />
              Trang trước
            </button>
            <span className="page-info">Trang {page}</span>
            <button
              className="pagination-btn"
              onClick={() => setPage(prev => prev + 1)}
              disabled={users.length < pageSize || isLoading}
            >
              Trang sau
              <ChevronRight size={16} />
            </button>
          </div>
        </div>
      </div>

      {/* Modal chỉnh sửa */}
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
