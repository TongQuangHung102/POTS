import React, { useState } from 'react';
import { Link } from "react-router-dom";
import './Auth.css';

const LoginForm = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');

  const handleChange = async (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    if (!formData.email) {
      setError('Vui lòng nhập email');
      return;
    }

    if (!formData.password) {
      setError('Vui lòng nhập mật khẩu');
      return;
    }

    try {
      const response = await fetch("https://localhost:7259/api/Auth/Login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          email: formData.email,
          password: formData.password
        })
      });

      const data = await response.json();

    if (!response.ok) {
      setError(data.message || "Đăng nhập thất bại");
      return;
    }

      if (data.token) {
        localStorage.setItem("token", data.token);
      }
  
 
      window.location.href = "/admin";
  
    } catch (error) {
      console.error("Lỗi khi đăng nhập:", error);
      setError("Có lỗi xảy ra, vui lòng thử lại!");
    }

    console.log('Đăng nhập với:', formData);
  };

  const handleGoogleLogin = () => {
    window.location.href = "https://localhost:7259/api/auth/login-google";
  };
  

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2 className="auth-title">Đăng nhập</h2>
        
        <form onSubmit={handleSubmit} className="auth-form">
          {error && (
            <div className="error-message">
              {error}
            </div>
          )}

          <div className="form-group">
            <label className="form-label">Email</label>
            <input
              type="email"
              name="email"
              placeholder="Email của bạn"
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
                placeholder="Mật khẩu của bạn"
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

          <button type="submit" className="submit-button">
            Đăng nhập
          </button>

          <div className="login-links">
            <a href="#" className="auth-link">Quên mật khẩu?</a>
            <Link to='/register'>Đăng ký tài khoản mới</Link>
          </div>

          <div className="divider">
            <span className="divider-text">Hoặc</span>
          </div>

          <button 
            type="button"
            className="google-button"
            onClick={handleGoogleLogin}
          >
            <img 
              src="https://www.google.com/favicon.ico" 
              alt="Google"
              className="google-icon"
            />
            Đăng nhập với Google
          </button>
        </form>
      </div>
    </div>
  );
};

export default LoginForm;