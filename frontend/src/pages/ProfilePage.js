import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Form, Button, Card, Alert } from 'react-bootstrap';
import axios from 'axios';

const Profile = () => {
    const [userInfo, setUserInfo] = useState({
        userName: '',
        email: '',
        phone: '',
        password: '',
        confirmPassword: '',
    });
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Lấy userId từ sessionStorage
    const userId = sessionStorage.getItem('userId');

    useEffect(() => {
        if (!userId) {
            setError('Không có ID người dùng trong session');
            setLoading(false);
            return;
        }

        // Lấy dữ liệu người dùng từ API
        const fetchUserData = async () => {
            try {
                const response = await axios.get(`https://localhost:7259/api/User/get-user-by/${userId}`);
                setUserInfo(response.data); // Dữ liệu người dùng trả về từ API
                setLoading(false);
            } catch (error) {
                setError('Không thể tải dữ liệu người dùng');
                setLoading(false);
            }
        };

        fetchUserData();
    }, [userId]);  // useEffect chỉ chạy khi userId thay đổi

    const handleChange = (e) => {
        setUserInfo({
            ...userInfo,
            [e.target.name]: e.target.value,
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        // Gửi dữ liệu đã chỉnh sửa lên API
        try {
            await axios.put(`https://localhost:7259/api/User/edit-user/${userId}`, {
                userName: userInfo.userName,
                email: userInfo.email,
                password: userInfo.password,
            });
            alert('Cập nhật thông tin thành công');
        } catch (error) {
            setError('Cập nhật thông tin thất bại');
        }
    };

    if (loading) {
        return <div>Đang tải dữ liệu...</div>;
    }

    return (
        <Container>
            <Row className="justify-content-center">
                <Col md={8}>
                    <Card>
                        <Card.Header>
                            <h3>Thông tin cá nhân</h3>
                        </Card.Header>
                        <Card.Body>
                            {error && <Alert variant="danger">{error}</Alert>}
                            <Form onSubmit={handleSubmit}>
                                <Form.Group controlId="formUsername">
                                    <Form.Label>Tên người dùng</Form.Label>
                                    <Form.Control
                                        type="text"
                                        placeholder="Nhập tên người dùng"
                                        name="userName"
                                        value={userInfo.userName}
                                        onChange={handleChange}
                                    />
                                </Form.Group>

                                <Form.Group controlId="formEmail">
                                    <Form.Label>Email</Form.Label>
                                    <Form.Control
                                        type="email"
                                        placeholder="Nhập email"
                                        name="email"
                                        value={userInfo.email}
                                        onChange={handleChange}
                                    />
                                </Form.Group>

                                <Form.Group controlId="formPhone">
                                    <Form.Label>Số điện thoại</Form.Label>
                                    <Form.Control
                                        type="tel"
                                        placeholder="Nhập số điện thoại"
                                        name="phone"
                                        value={userInfo.phone}
                                        onChange={handleChange}
                                    />
                                </Form.Group>

                                <Form.Group controlId="formPassword">
                                    <Form.Label>Mật khẩu</Form.Label>
                                    <Form.Control
                                        type="password"
                                        placeholder="Nhập mật khẩu mới"
                                        name="password"
                                        value={userInfo.password}
                                        onChange={handleChange}
                                    />
                                </Form.Group>

                                <Form.Group controlId="formConfirmPassword">
                                    <Form.Label>Xác nhận mật khẩu</Form.Label>
                                    <Form.Control
                                        type="password"
                                        placeholder="Xác nhận mật khẩu"
                                        name="confirmPassword"
                                        value={userInfo.confirmPassword}
                                        onChange={handleChange}
                                    />
                                </Form.Group>

                                <Button variant="primary" type="submit">
                                    Cập nhật thông tin
                                </Button>
                            </Form>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
};

export default Profile;
