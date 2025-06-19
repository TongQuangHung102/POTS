import React, { useState, useEffect } from 'react';
import { GoogleOAuthProvider, GoogleLogin } from "@react-oauth/google";
import { useSearchParams } from "react-router-dom";
import { Link } from "react-router-dom";
import Loading from '../components/Loading';
import './Auth.css';

const clientId = "486727453623-gisg4i179stste58r793ru3j30iasd4k.apps.googleusercontent.com";

const LoginForm = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);
  const [loadingSendMail, setLoadingSendMail] = useState(false);
  const [isResendMail, setIsResendMail] = useState(false);
  const [googleError, setGoogleError] = useState('');

  const [searchParams] = useSearchParams();
  const status = searchParams.get("status");

  const apiUrl = process.env.REACT_APP_API_URL;

  useEffect(() => {
    if (status === "success") {
      setSuccess("Xác nhận email thành công! Bạn có thể đăng nhập.");
    } else if (status === "invalid_token") {
      setError("Token không hợp lệ! Vui lòng kiểm tra lại email.");
    } else if (status === "token_expired") {
      setError("Token đã hết hạn! Vui lòng yêu cầu gửi lại email xác nhận.");
    }
  }, [status]);

  const handleSuccess = async (response) => {
    setLoading(true);
    try {
      const googleToken = response.credential;
      const res = await fetch(`${apiUrl}/api/Auth/google-login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({ token: googleToken }),
        credentials: 'include'
      });

      if (!res.ok) {
        throw new Error("Có lỗi xảy ra, vui lòng thử lại sau!");
      }

      const data = await res.json();
      handleLoginSuccess(data);
    } catch (error) {
      console.error("Google Login Failed:", error);
      setError(error.message || "Đăng nhập thất bại");
    }
    finally {
      setLoading(false);
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
      const response = await fetch(`${apiUrl}/api/Auth/Login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          email: formData.email,
          password: formData.password
        }),
      });

      const data = await response.json();

      if (!response.ok) {
        setError(data.message || "Đăng nhập thất bại");
        if (data.message === "Tài khoản chưa được kích hoạt") {
          setIsResendMail(true);
        }
        setLoading(false);
        return;
      }
      handleLoginSuccess(data);
    } catch (error) {
      console.error("Lỗi khi đăng nhập:", error);
      setError("Có lỗi xảy ra, vui lòng thử lại!");
    }

  };

  const handleResendEmail = async () => {
    setLoadingSendMail(true);
    try {
      const response = await fetch(`${apiUrl}/api/Auth/resend-confirmation-email`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: formData.email }),
      });

      const data = await response.json();
      setIsResendMail(false);
      setError('');
      alert(data.message);

    } catch (error) {
      alert("Có lỗi xảy ra, vui lòng thử lại");
    } finally {
      setLoadingSendMail(false);
    }
  };

  const handleLoginSuccess = (data) => {
    const now = new Date();
    const vnTime = new Date(now.toLocaleString('en-US', { timeZone: 'Asia/Ho_Chi_Minh' }));
    const expiresAt = vnTime.getTime() + (24 * 60 * 60 * 1000);

    localStorage.setItem("roleId", data.role);
    localStorage.setItem("userId", data.userId);
    localStorage.setItem("token", data.token);
    localStorage.setItem("tokenExpiresAt", expiresAt.toString());

    if (data.role === 1 && data.gradeId !== null) {
      localStorage.setItem("gradeId", data.gradeId);
      window.location.href = "/student";
    }
    else if (data.role === 2) window.location.href = "/parent";
    else if (data.role === 3) window.location.href = "/admin";
    else if (data.role === 4) {
      localStorage.setItem("managedGrades", JSON.stringify(data.grades));
      window.location.href = "/content_manage";
    }
    else if (data.role === 1 && data.gradeId === null) window.location.href = "/choose-grade"
    else window.location.href = "/choose-role";

  };

  if (loading) {
    return <div className="loading-spinner">Đang tải dữ liệu...</div>;
  }


  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2 className="auth-title">Đăng nhập</h2>

        <form onSubmit={handleSubmit} className="auth-form">
          {error && (
            <div>
              <div className="error-message">
                {error}
              </div>
              {isResendMail && (<p className='resend-email' onClick={handleResendEmail}>Gửi lại email kích hoạt tài khoản</p>)}
            </div>

          )}
          {success && (
            <div style={{color : "green"}}>
              {success}
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
                autocomplete="current-password"
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
                onError={() => {
                  setGoogleError("Đăng nhập Google thất bại. Vui lòng thử lại.");
                }}
                useOneTap
              />
              {googleError && (
                <div className="error-message" style={{ marginTop: '10px' }}>
                  {googleError}
                </div>
              )}
            </div>
          </GoogleOAuthProvider>
        </form>
      </div>
      {loadingSendMail && (
        <div className="overlay-auth">
          <Loading message="Đang gửi liên kết về mail của bạn..." />
        </div>
      )}
    </div>
  );
};

export default LoginForm;