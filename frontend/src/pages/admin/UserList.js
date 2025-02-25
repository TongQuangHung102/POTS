import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
<<<<<<< HEAD
=======

>>>>>>> origin/main

const UsersList = () => {
    const [users, setUsers] = useState([]);
    const [errorMessage, setErrorMessage] = useState('');
    const [page, setPage] = useState(1);  // Trang hiện tại
    const [pageSize, setPageSize] = useState(10);  // Số lượng người dùng mỗi trang
    const [roleId, setRoleId] = useState('');  // Lọc theo vai trò
    const [email, setEmail] = useState('');  // Lọc theo email
    const navigate = useNavigate();

    // Hàm gọi API khi có thay đổi các tham số tìm kiếm và phân trang
    useEffect(() => {
<<<<<<< HEAD
        const params = new URLSearchParams({
            roleId: roleId,
            email: email,
            page: page,
            pageSize: pageSize
        }).toString();

        fetch(`https://localhost:7259/api/User/get-all-user?${params}`)
=======
        fetch('https://localhost:7259/api/User/get-all-user')
>>>>>>> origin/main
            .then(response => response.json())
            .then(data => setUsers(data))
            .catch(error => setErrorMessage('Lỗi khi tải danh sách người dùng!'));
    }, [page, pageSize, roleId, email]);  // Thêm roleId và email vào dependencies

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

            {/* Thêm các trường tìm kiếm và lọc */}
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
                    {/* Thêm các vai trò khác nếu cần */}
                </select>
            </div>

            <button onClick={() => navigate('/admin/user/new')} className="add-button">➕ Thêm Người Dùng</button>

            {/* Thêm phân trang */}
            <div className="pagination">
                <button onClick={() => setPage(page - 1)} disabled={page === 1}>Trước</button>
                <span>Trang {page}</span>
                <button onClick={() => setPage(page + 1)}>Sau</button>
            </div>

            <table>
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
                    {users.map(user => (
                        <tr key={user.id}>
                            <td>{user.id}</td>
                            <td>{user.name}</td>
                            <td>{user.email}</td>
                            <td>{user.roleId}</td> {/* Hiển thị vai trò người dùng */}
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
