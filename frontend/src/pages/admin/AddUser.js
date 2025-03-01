import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import './AddUser.css';

const AddUser = () => {
    const [userName, setUserName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [role, setRole] = useState('');
    const [roles, setRoles] = useState([]);
    const [isActive, setIsActive] = useState(true);
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

    const handleAddUser = async (e) => {
        e.preventDefault();
        try {
            const selectedRole = roles.find(r => r.roleName === role);
            const payload = {
                userName,
                email,
                password,
                role: selectedRole ? selectedRole.roleId : null,
                isActive,
            };

            await axios.post('https://localhost:7259/api/User/add-user', payload, {
                headers: { 'Content-Type': 'application/json' }
            });

            alert('Người dùng đã được thêm thành công!');
            navigate('/admin/users'); // Quay lại danh sách user
        } catch (error) {
            setErrorMessage(`Lỗi khi thêm người dùng: ${error.response?.data?.message || error.message}`);
        }
    };

    return (
        <div className="add-user-container">
            <h2>Thêm Người Dùng</h2>
            {errorMessage && <div className="error">{errorMessage}</div>}

            <form onSubmit={handleAddUser}>
                <label>Tên:</label>
                <input type="text" value={userName} onChange={(e) => setUserName(e.target.value)} required />

                <label>Email:</label>
                <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />

                <label>Mật khẩu:</label>
                <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />

                <label>Vai trò:</label>
                <select value={role} onChange={(e) => setRole(e.target.value)} required>
                    <option value="">Chọn vai trò</option>
                    {roles.map(r => (
                        <option key={r.roleId} value={r.roleName}>{r.roleName}</option>
                    ))}
                </select>

                <label>Trạng thái:</label>
                <select value={isActive} onChange={(e) => setIsActive(e.target.value === 'true')}>
                    <option value="true">Hoạt động</option>
                    <option value="false">Bị khóa</option>
                </select>

                <button type="submit">Thêm Người Dùng</button>
            </form>

            <button className="back-btn" onClick={() => navigate('/admin/users')}>⬅ Quay lại</button>
        </div>
    );
};

export default AddUser;
