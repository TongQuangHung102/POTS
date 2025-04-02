import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './AddUser.css'
const AddUser = () => {
    const [userName, setUserName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [role, setRole] = useState('');
    const [isActive, setIsActive] = useState(true);
    const [roles, setRoles] = useState([]);
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        const fetchRoles = async () => {
            try {
                const response = await axios.get('https://localhost:7259/api/Role/get-all-role');
                setRoles(response.data);
            } catch (error) {
                setErrorMessage(`Lỗi khi tải danh sách vai trò: ${error.response?.data?.message || error.message}`);
            }
        };

        fetchRoles();
    }, []);

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const newUser = {
                userName,
                email,
                password,
                role: role,
                isActive
            };

            const response = await axios.post('https://localhost:7259/api/User/add-user', newUser, {
                headers: { 'Content-Type': 'application/json' }
            });

            if (response.status === 200) {
                navigate('/admin/users');  // Điều hướng về danh sách người dùng sau khi thêm thành công
            }
        } catch (error) {
            setErrorMessage(`Lỗi khi thêm người dùng: ${error.response?.data?.message || error.message}`);
        }
    };

    return (
        <div className="add-user-container">
            <h2>Thêm Người Dùng</h2>
            {errorMessage && <div className="error">{errorMessage}</div>}

            <form onSubmit={handleSubmit}>
                <div>
                    <label>Tên người dùng</label>
                    <input
                        type="text"
                        value={userName}
                        onChange={(e) => setUserName(e.target.value)}
                        required
                    />
                </div>

                <div>
                    <label>Email</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>

                <div>
                    <label>Mật khẩu</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>

                <div>
                    <label>Vai trò</label>
                    <select value={role} onChange={(e) => setRole(e.target.value)} required>
                        <option value="">Chọn vai trò</option>
                        {roles.map((role) => (
                            <option key={role.roleId} value={role.roleId}>
                                {role.roleName}
                            </option>
                        ))}
                    </select>
                </div>

                <div>
                    <label>Trạng thái</label>
                    <select value={isActive} onChange={(e) => setIsActive(e.target.value === 'true')} required>
                        <option value={true}>Đang hoạt động</option>
                        <option value={false}>Đã bị khóa</option>
                    </select>
                </div>

                <div>
                    <button type="submit">Thêm người dùng</button>
                </div>
            </form>
        </div>
    );
};

export default AddUser;
