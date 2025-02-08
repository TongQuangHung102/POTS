import React, { useState } from "react";
import { Form, Button, Alert } from "react-bootstrap";

const RegisterForm = ({ onLogin }) => {
    const [email, setEmail] = useState("");
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [role, setRole] = useState(""); // Không đặt mặc định
    const [error, setError] = useState("");
    const [success, setSuccess] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!email || !username || !password || !confirmPassword || !role) {
            setError("Vui lòng điền đầy đủ thông tin.");
            return;
        }

        if (!/\S+@\S+\.\S+/.test(email)) {
            setError("Email không hợp lệ.");
            return;
        }

        if (password !== confirmPassword) {
            setError("Mật khẩu không trùng khớp.");
            return;
        }

        setError("");
        setSuccess(false);

        try {
            const response = await fetch("https://localhost:7259/api/Auth/Register", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    userName: username,
                    email: email,
                    password: password,
                    role: role === "sinh viên" ? 1 : 2, // Tránh lỗi undefined
                    createAt: new Date().toISOString(),
                    fullName: "",
                    isActive: true,
                }),
            });

            const responseData = await response.json();

            if (response.ok) {
                setSuccess(true);
            } else {
                setError(responseData.message || "Lỗi đăng ký.");
                if (responseData.errors) {
                    console.log("Lỗi chi tiết:", responseData.errors);
                }
            }
        } catch (error) {
            setError("Lỗi kết nối đến server. Hãy thử lại sau.");
        }
    };

    return (
        <div>
            <h2 style={{ color: "#AAB7B7", textAlign: "center", marginBottom: "30px" }}>Đăng ký</h2>
            <Form onSubmit={handleSubmit}>
                {/* Tên đăng nhập */}
                <Form.Group className="mb-3" controlId="formBasicUsername">
                    <Form.Label style={{ color: "#C0C8CA" }}>Tên đăng nhập</Form.Label>
                    <Form.Control
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        style={{ backgroundColor: "#D4D8DD", color: "#1A2D42" }}
                        placeholder="Nhập tên đăng nhập của bạn"
                    />
                </Form.Group>

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

                {/* Xác nhận mật khẩu */}
                <Form.Group className="mb-3" controlId="formBasicConfirmPassword">
                    <Form.Label style={{ color: "#C0C8CA" }}>Xác nhận mật khẩu</Form.Label>
                    <Form.Control
                        type="password"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                        style={{ backgroundColor: "#D4D8DD", color: "#1A2D42" }}
                        placeholder="Nhập lại mật khẩu của bạn"
                    />
                </Form.Group>

                {/* Chọn vai trò */}
                <Form.Group className="mb-3" controlId="formBasicRole">
                    <Form.Label style={{ color: "#C0C8CA" }}>Vai trò</Form.Label>
                    <Form.Control
                        as="select"
                        value={role}
                        onChange={(e) => setRole(e.target.value)}
                        style={{ backgroundColor: "#D4D8DD", color: "#1A2D42" }}
                    >
                        <option value="">-- Chọn vai trò --</option> {/* Đảm bảo người dùng phải chọn */}
                        <option value="sinh viên">Sinh viên</option>
                        <option value="phụ huynh">Phụ huynh</option>
                    </Form.Control>
                </Form.Group>

                {/* Hiển thị lỗi hoặc thông báo thành công */}
                {error && <Alert variant="danger" style={{ marginBottom: "10px" }}>{error}</Alert>}
                {success && <Alert variant="success" style={{ marginBottom: "10px" }}>Đăng ký thành công!</Alert>}

                {/* Nút đăng ký */}
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
                >
                    Đăng ký
                </Button>
            </Form>

            {/* Chuyển đến trang đăng nhập */}
            <div style={{ textAlign: "center", marginTop: "20px" }}>
                <span style={{ color: "#C0C8CA" }}>Bạn đã có tài khoản? </span>
                <Button
                    variant="link"
                    style={{ color: "#AAB7B7", textDecoration: "none" }}
                    onClick={onLogin}
                >
                    Đăng nhập
                </Button>
            </div>
        </div>
    );
};

export default RegisterForm;
