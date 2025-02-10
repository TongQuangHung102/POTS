import React, { useState } from "react";
import { Form, Button, Alert } from "react-bootstrap";
//import { useNavigate } from "react-router-dom";

const LoginForm = ({ onRegister, onForgotPassword }) => {
  // const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError("");

    if (!email || !password) {
      setError("Vui lòng điền đầy đủ thông tin.");
      setIsLoading(false);
      return;
    }

    try {
      const response = await fetch("https://localhost:7259/api/Auth/Login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: email,
          password: password,
        }),
      });

      const responseData = await response.json();

      if (response.ok) {
        // Lưu token vào sessionStorage
        const token = responseData.token;
        sessionStorage.setItem("token", token);
        // Điều hướng đến dashboard
        //navigate("/dashboard");
        setSuccess(true);
      } else {
        setError(responseData.message || "Lỗi đăng nhập. Vui lòng thử lại.");
      }
    } catch (err) {
      setError("Lỗi kết nối đến server. Hãy thử lại sau.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div>
      <h2 style={{ color: "#AAB7B7", textAlign: "center", marginBottom: "30px" }}>Đăng nhập</h2>
      <Form onSubmit={handleSubmit}>
        {/* Email */}
        <Form.Group className="mb-3" controlId="formBasicEmail">
          <Form.Label style={{ color: "#C0C8CA" }}>Email</Form.Label>
          <Form.Control
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            style={{ backgroundColor: "#D4D8DD", color: "#1A2D42" }}
            placeholder="Nhập email của bạn"
          />
        </Form.Group>

        {/* Mật khẩu */}
        <Form.Group className="mb-3" controlId="formBasicPassword">
          <Form.Label style={{ color: "#C0C8CA" }}>Mật khẩu</Form.Label>
          <Form.Control
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            style={{ backgroundColor: "#D4D8DD", color: "#1A2D42" }}
            placeholder="Nhập mật khẩu của bạn"
          />
        </Form.Group>

        {/* Hiển thị lỗi hoặc thông báo thành công */}
        {error && <Alert variant="danger" style={{ marginBottom: "10px" }}>{error}</Alert>}
        {success && <Alert variant="success" style={{ marginBottom: "10px" }}>Đăng nhập thành công!</Alert>}

        {/* Nút đăng nhập */}
        <Button
          type="submit"
          style={{
            width: "100%",
            padding: "10px",
            backgroundColor: "#AAB7B7",
            color: "#1A2D42",
            border: "none",
            borderRadius: "5px",
          }}
          disabled={isLoading}
        >
          {isLoading ? "Đang xử lý..." : "Đăng nhập"}
        </Button>
      </Form>

      {/* Chuyển đến đăng ký */}
      <div style={{ textAlign: "center", marginTop: "20px" }}>
        <Button
          variant="link"
          style={{ color: "#AAB7B7", textDecoration: "none" }}
          onClick={onRegister}
        >
          Chưa có tài khoản? Đăng ký
        </Button>
      </div>

      {/* Quên mật khẩu */}
      <div style={{ textAlign: "center", marginTop: "10px" }}>
        <Button
          variant="link"
          style={{ color: "#AAB7B7", textDecoration: "none" }}
          onClick={onForgotPassword}
        >
          Quên mật khẩu?
        </Button>
      </div>
    </div>
  );
};

export default LoginForm;