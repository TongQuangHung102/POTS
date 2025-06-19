import React, { useEffect, useState } from 'react';
import { Link } from "react-router-dom";
import { validateEmail, cleanAndValidateString, isValidString } from '../utils/validator';
import './Auth.css';
import Loading from '../components/Loading';


const RegisterForm = () => {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    password: '',
    confirmPassword: ''
  });
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [loading, setLoading] = useState(false);

  const apiUrl = process.env.REACT_APP_API_URL;
  

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
    const { valid, cleaned } = cleanAndValidateString(formData.name);
    if(!valid || cleaned.length > 30){
      setError('Vui lòng nhập tên hợp lệ');
      return;
    }

    if (!formData.email) {
      setError('Vui lòng nhập email');
      return;
    }

    if (!validateEmail(formData.email)) {
      setError("Sai định dạng email");
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

    if (formData.password.length >= 24) {
      setError('Mật khẩu phải ít hơn 24 ký tự');
      return;
    }

    if (!isValidString(formData.password)) {
      setError('Mật khẩu không được chứa khoảng trắng, dấu và ký tự đặc biệt');
      return;
    }

    if (formData.password !== formData.confirmPassword) {
      setError('Mật khẩu xác nhận không khớp');
      return;
    }

    if (formData.role == '') {
      setError('Vui lòng chọn vai trò');
      return;
    }

    const registerData = {
      userName: cleaned,
      email: formData.email,
      password: formData.password,
      role: formData.role,
    };

    try {
      setLoading(true);
      const response = await fetch(`${apiUrl}/api/Auth/Register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(registerData),
      });

      const result = await response.json();

      if (response.ok) {
      
        setMessage(result.message || 'Đăng ký thành công, Vui lòng mở email để xác nhận!')
      } else {
        setError(result.message || 'Đăng ký không thành công');
      }
    } catch (error) {
      console.error('Lỗi khi đăng ký:', error);
      setError('Đã có lỗi xảy ra. Vui lòng thử lại.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
      setTimeout(() => {
        setError('');
        setMessage('');
      }, 3000);
    }, [error, message]);


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
            {message && (
            <div className="success-message">
              {message}
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
      {loading && (
      <div className="overlay-auth">
        <Loading message="Đang tiến hành đăng ký..." />
      </div>
    )}
    </div>
  );
};

export default RegisterForm;