import React, { useState } from "react";
import { Form, Button, Alert } from "react-bootstrap";

const ForgotPasswordForm = ({ onLogin }) => {
    const [email, setEmail] = useState("");
    const [error, setError] = useState("");
    const [success, setSuccess] = useState(false);
    const [loading, setLoading] = useState(false);


    const isValidEmail = (email) => /\S+@\S+\.\S+/.test(email);

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!email) {
            setError("Vui lòng điền email.");
            return;
        }

        if (!isValidEmail(email)) {
            setError("Email không hợp lệ.");
            return;
        }

        setError("");
        setSuccess(false);
        setLoading(true);

        try {
            const API_URL = "https://localhost:7259/api/Auth/Reset-password";

            const response = await fetch(API_URL, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email }),
            });

            if (!response.ok) {
                const responseData = await response.json();
                setError(responseData.message || "Không thể đặt lại mật khẩu. Vui lòng thử lại.");
                return;
            }

            setSuccess(true);
            setEmail("");
        } catch {
            setError("Lỗi kết nối đến server. Hãy thử lại sau.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div>
            <h2
                style={{
                    color: "#AAB7B7",
                    textAlign: "center",
                    marginBottom: "30px",
                }}
            >
                Quên Mật Khẩu
            </h2>
            <Form onSubmit={handleSubmit}>

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


                {error && (
                    <Alert variant="danger" style={{ marginBottom: "10px" }}>
                        {error}
                    </Alert>
                )}
                {success && (
                    <Alert variant="success" style={{ marginBottom: "10px" }}>
                        Đã gửi email đặt lại mật khẩu! Vui lòng kiểm tra hộp thư.
                    </Alert>
                )}


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
                    disabled={loading}
                >
                    {loading ? "Đang gửi yêu cầu..." : "Đặt lại mật khẩu"}
                </Button>
            </Form>


            <div style={{ textAlign: "center", marginTop: "20px" }}>
                <Button
                    variant="link"
                    style={{ color: "#AAB7B7", textDecoration: "none" }}
                    onClick={onLogin}
                >
                    Quay lại đăng nhập
                </Button>
            </div>
        </div>
    );
};

export default ForgotPasswordForm;