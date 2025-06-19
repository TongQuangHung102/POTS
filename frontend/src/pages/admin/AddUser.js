import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { authFetch } from '../../utils/authFetch';
import { validateEmail, cleanAndValidateString, isValidString } from '../../utils/validator';
import './AddUser.css'
const AddUser = () => {
    const [userName, setUserName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [role, setRole] = useState('');
    const [isActive, setIsActive] = useState(true);
    const [roles, setRoles] = useState([]);
    const [errorMessage, setErrorMessage] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const navigate = useNavigate();
    const apiUrl = process.env.REACT_APP_API_URL;

    useEffect(() => {
        const fetchRoles = async () => {
            try {
                const response = await authFetch(`${apiUrl}/api/Role/get-all-role`);
                const data = await response.json();

                if (data) {
                    setRoles(data);
                } else {
                    setErrorMessage('Không nhận được dữ liệu vai trò từ server.');
                }
            } catch (error) {
                console.error('Lỗi khi fetch roles:', error);
                const message =
                    error?.response?.data?.message ||
                    error?.message ||
                    'Đã xảy ra lỗi không xác định khi tải vai trò.';
                setErrorMessage(`Lỗi khi tải danh sách vai trò: ${message}`);
            }
        };

        fetchRoles();
    }, []);


    const handleSubmit = async (e) => {
        e.preventDefault();
        e.preventDefault();
        setErrorMessage('');

        const { valid, cleaned } = cleanAndValidateString(userName);
        if (!valid || cleaned.length > 30) {
            setErrorMessage('Vui lòng nhập tên hợp lệ và không vượt quá 30 ký tự');
            return;
        }

        if (!email) {
            setErrorMessage('Vui lòng nhập email');
            return;
        }

        if (!validateEmail(email)) {
            setErrorMessage('Sai định dạng email');
            return;
        }

        if (!password) {
            setErrorMessage('Vui lòng nhập mật khẩu');
            return;
        }

        if (password.length < 6) {
            setErrorMessage('Mật khẩu phải có ít nhất 6 ký tự');
            return;
        }

        if (password.length >= 24) {
            setErrorMessage('Mật khẩu phải ít hơn 24 ký tự');
            return;
        }

        if (!isValidString(password)) {
            setErrorMessage('Mật khẩu không được chứa khoảng trắng hoặc ký tự đặc biệt');
            return;
        }

        if (password !== confirmPassword) {
            setErrorMessage('Mật khẩu xác nhận không khớp');
            return;
        }

        if (!role) {
            setErrorMessage('Vui lòng chọn vai trò');
            return;
        }
        try {
            const newUser = {
                userName,
                email,
                password,
                role: role,
                isActive
            };

            const response = await authFetch(
                `${apiUrl}/api/User/add-user`,
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(newUser)
                }
            );

            const result = await response.json();

            if (response.status === 200) {
                navigate('/admin/users');  // Điều hướng về danh sách người dùng sau khi thêm thành công
            }
            else {
                setErrorMessage(result?.message || 'Có lỗi xảy ra khi tạo người dùng.');
            }
        } catch (error) {
            setErrorMessage(error?.message || 'Lỗi không xác định khi gọi API.');
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
                    <label>Xác nhận mật khẩu</label>
                    <input
                        type="password"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
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
                        <option value={true}>Hoạt động</option>
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
