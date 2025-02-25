import React, { useState } from 'react';
import { GoogleOAuthProvider, GoogleLogin } from "@react-oauth/google";
import { Link } from "react-router-dom";
import './Auth.css';

const clientId = "486727453623-gisg4i179stste58r793ru3j30iasd4k.apps.googleusercontent.com";

const LoginForm = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');

  const handleSuccess = async (response) => {
    try {
      const googleToken = response.credential;
      const res = await fetch("https://localhost:7259/api/Auth/google-login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({ token: googleToken })
      });

      if (!res.ok) {
        throw new Error("Failed to authenticate");
      }

      const data = await res.json();
      handleLoginSuccess(data);
    } catch (error) {
      console.error("Google Login Failed:", error);
    }
  };


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

      handleLoginSuccess(data);


    } catch (error) {
      console.error("Lỗi khi đăng nhập:", error);
      setError("Có lỗi xảy ra, vui lòng thử lại!");
    }

    console.log('Đăng nhập với:', formData);
  };

  const handleLoginSuccess = (data) => {
    if (data.token) {
      sessionStorage.setItem("token", data.token);
      sessionStorage.setItem("roleId", data.role);
      sessionStorage.setItem("userId", data.userId);

      if (data.role === 1) window.location.href = "/student";
      else if (data.role === 2) window.location.href = "/parent";
      else if (data.role === 3) window.location.href = "/admin";
      else window.location.href = "/choose-role";
    }
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
          <Link to='/forgot-password'>Quên mật khẩu?</Link>
            <Link to='/register'>Đăng ký tài khoản mới</Link>
          </div>

          <div className="divider">
            <span className="divider-text">Hoặc</span>
          </div>

          <GoogleOAuthProvider clientId={clientId}>
            <div>
              <GoogleLogin
                onSuccess={handleSuccess}
                onError={() => console.log("Login Failed")}
              />
            </div>
          </GoogleOAuthProvider>
        </form>
      </div>
    </div>
  );
};

export default LoginForm;