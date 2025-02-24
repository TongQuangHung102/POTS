import React, { useState } from 'react';
import { Link } from "react-router-dom";
import './Auth.css';

const RegisterForm = () => {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    password: '',
    confirmPassword: ''
  });
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    if (!formData.name) {
      setError('Vui lòng nhập họ tên');
      return;
    }

    if (!formData.email) {
      setError('Vui lòng nhập email');
      return;
    }

    if (!formData.password) {
      setError('Vui lòng nhập mật khẩu');
      return;
    }

    if (formData.password.length < 6) {
      setError('Mật khẩu phải có ít nhất 6 ký tự');
      return;
    }

    if (formData.password !== formData.confirmPassword) {
      setError('Mật khẩu xác nhận không khớp');
      return;
    }

    const registerData = {
      userName: formData.name,
      email: formData.email,
      password: formData.password,
      role: formData.role,
    };

    try {
      // Gửi yêu cầu đăng ký đến API
      const response = await fetch('https://localhost:7259/api/Auth/Register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(registerData),
      });

      const result = await response.json();

      if (response.ok) {
        console.log('Đăng ký thành công:', result);
        window.location.href = '/login';
      } else {
        setError(result.message || 'Đăng ký không thành công');
      }
    } catch (error) {
      console.error('Lỗi khi đăng ký:', error);
      setError('Đã có lỗi xảy ra. Vui lòng thử lại.');
    } finally {
      setLoading(false);
    }

    console.log('Đăng ký với:', formData);
  };


  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2 className="auth-title">Đăng ký tài khoản</h2>

        <form onSubmit={handleSubmit} className="auth-form">
          {error && (
            <div className="error-message">
              {error}
            </div>
          )}

          <div className="form-group">
            <label className="form-label">Họ tên</label>
            <input
              type="text"
              name="name"
              placeholder="Nhập họ tên của bạn"
              value={formData.name}
              onChange={handleChange}
              className="form-input"
            />
          </div>

          <div className="form-group">
            <label className="form-label">Email</label>
            <input
              type="email"
              name="email"
              placeholder="Nhập email của bạn"
              value={formData.email}
              onChange={handleChange}
              className="form-input"
            />
          </div>

          <div className="form-group">
            <label className="form-label">Mật khẩu</label>
            <div className="password-input">
              <input
                type={showPassword ? "text" : "password"}
                name="password"
                placeholder="Nhập mật khẩu"
                value={formData.password}
                onChange={handleChange}
                className="form-input"
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="toggle-password"
              >
                {showPassword ? "Ẩn" : "Hiện"}
              </button>
            </div>
          </div>

          <div className="form-group">
            <label className="form-label">Xác nhận mật khẩu</label>
            <div className="password-input">
              <input
                type={showPassword ? "text" : "password"}
                name="confirmPassword"
                placeholder="Nhập lại mật khẩu"
                value={formData.confirmPassword}
                onChange={handleChange}
                className="form-input"
              />
            </div>
          </div>

          <div className="form-group">
            <label className="form-label">Vai trò</label>
            <select
              name="role"
              value={formData.role}
              onChange={handleChange}
              className="form-input"
            >
              <option value="">Chọn vai trò</option>
              <option value="1">Học sinh</option>
              <option value="2">Phụ huynh</option>
            </select>
          </div>

          <button type="submit" className="submit-button">
            Đăng ký
          </button>

          <div className="auth-links">
            <Link to='/login'>Bạn đã có tài khoản? Đăng nhập</Link>
          </div>
        </form>
      </div>
    </div>
  );
};

export default RegisterForm;