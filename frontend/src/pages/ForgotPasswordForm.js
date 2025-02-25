import React, { useState } from "react";
import { Link } from "react-router-dom";
import './Auth.css';

const ForgotPasswordForm = () => {
  const [email, setEmail] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setMessage('');

    if (!email) {
      setError('Vui lòng nhập email');
      return;
    }

    try {
      const response = await fetch("https://localhost:7259/api/Auth/Reset-password", {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({ email })
      });

      const text = await response.text();

      if (!response.ok) {
        setError(text || "Có lỗi xảy ra, vui lòng thử lại!");
        return;
      }

      setMessage(text || "Hướng dẫn đặt lại mật khẩu đã được gửi đến email của bạn.");
    } catch (error) {
      console.error("Lỗi khi gửi yêu cầu quên mật khẩu:", error);
      setError("Có lỗi xảy ra, vui lòng thử lại!");
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2 className="auth-title">Quên mật khẩu</h2>

        <form onSubmit={handleSubmit} className="auth-form">
          {error && <div className="error-message">{error}</div>}
          {message && <div className="success-message">{message}</div>}

          <div className="form-group">
            <label className="form-label">Email</label>
            <input
              type="email"
              name="email"
              placeholder="Nhập email của bạn"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="form-input"
            />
          </div>

          <button type="submit" className="submit-button">Gửi yêu cầu</button>

          <div className="login-links">
            <Link to='/login'>Quay lại đăng nhập</Link>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ForgotPasswordForm;