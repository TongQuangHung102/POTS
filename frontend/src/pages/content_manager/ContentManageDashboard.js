import React, { useEffect, useRef, useState } from 'react';
import { Card, Row, Col, Table } from 'react-bootstrap';
import { Chart as ChartJS, LineElement, PointElement, CategoryScale, LinearScale, Title, Tooltip, Legend, BarElement } from 'chart.js';
import { Chart } from 'react-chartjs-2';
import { useNavigate } from 'react-router-dom';
import { BiCube, BiSolidUserCheck } from "react-icons/bi";

ChartJS.register(LineElement, PointElement, CategoryScale, LinearScale, Title, Tooltip, Legend, BarElement);

const ContentManageDashboard = () => {
    const [dashboardData, setDashboardData] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [dataTotalStudent, setDataTotalStudent] = useState({});
    const [dataTime, setDataTime] = useState({});

    const [managedGrades, setManagedGrades] = useState(() => {
        const savedGrades = sessionStorage.getItem("managedGrades");
        return savedGrades ? JSON.parse(savedGrades) : [];
    });

    const [selectedGrade, setSelectedGrade] = useState(() => {
        return managedGrades.length > 0 ? managedGrades[0] : null;
    });

    const navigate = useNavigate();

    useEffect(() => {
        sessionStorage.setItem("managedGrades", JSON.stringify(managedGrades));
        if (managedGrades.length > 0 && !selectedGrade) {
            setSelectedGrade(managedGrades[0]);
        }
    }, [managedGrades]);

    useEffect(() => {
        setIsLoading(true);
        fetchData(selectedGrade.id);
        setIsLoading(false);
    }, []);

    const fetchData = async (gradeId) => {
        fetch(`https://localhost:7259/api/Dashboard/content-manage-dashboard/${gradeId}`)
            .then(res => res.json())
            .then(data => {
                setDashboardData(data);
                setDataTotalStudent({
                    labels: data.totalStudentDto.labels,
                    datasets: [{
                        label: "Học sinh mới",
                        data: data.totalStudentDto.data,
                        backgroundColor: "rgba(54, 162, 235, 0.2)",
                        borderColor: "rgba(54, 162, 235, 1)",
                        borderWidth: 2,
                        fill: true,
                        tension: 0.4
                    }]
                });

                setDataTime({
                    labels: data.activityDto.labels,
                    datasets: [{
                        label: "Tổng Thời Gian Luyệt Tập (Phút)",
                        data: data.activityDto.data,
                        backgroundColor: "rgba(75, 192, 192, 0.2)",
                        borderColor: "rgba(75, 192, 192, 1)",
                        borderWidth: 1
                    }]
                })
            });
    };


    const progressConfig = {
        type: 'line',
        data: dataTotalStudent,
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: 'Học sinh đăng ký mới'
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Học sinh'
                    }
                },
                x: {
                    title: {
                        display: true,
                        text: 'Ngày'
                    }
                }
            }
        }
    };

    // Cấu hình cho biểu đồ tổng thời gian học
    const timeConfig = {
        type: 'bar',
        data: dataTime,
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: 'Tổng Thời Gian Luyện Tập'
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Thời gian (Phút)'
                    }
                },
                x: {
                    title: {
                        display: true,
                        text: 'Ngày'
                    }
                }
            }
        }
    };

    const formatCurrency = (amount) => {
        return new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(amount);
    };

    const handleGradeChange = (event) => {
        const gradeId = parseInt(event.target.value, 10);
        const grade = managedGrades.find(g => g.id === gradeId);
        setSelectedGrade(grade);
    };

    if (isLoading) {
        return <div className="loading-dashboard">Đang tải dữ liệu...</div>;
    }
    if (!dashboardData) {
        return <div className="loading-dashboard">Không có dữ liệu hiển thị</div>;
    }
    return (
        <div className="main-content">
            <Row>
                <Col md={12}>
                    <Row className="mb-4">
                        <Col md={3}>
                            <Card className="stats-card">
                                <small className="text-danger">Tổng số học sinh</small>
                                <div className="stats-number">{dashboardData.totalStudent}</div>
                            </Card>
                        </Col>
                        <Col md={3}>
                            <Card className="stats-card">
                                <small className="text-primary">Tổng số học sinh mới</small>
                                <div className="stats-number">{dashboardData.newStudent}</div>
                            </Card>
                        </Col>
                        <Col md={3}>
                            <Card className="stats-card">
                                <small className="text-primary">Tổng số câu hỏi</small>
                                <div className="stats-number">{dashboardData.totalQuestion}</div>
                            </Card>
                        </Col>
                        <Col md={3}>
                            <Card className="stats-card">
                                <small className="text-primary">Tổng số câu hỏi AI</small>
                                <div className="stats-number">{dashboardData.totalQuestionAi}</div>
                            </Card>
                        </Col>
                    </Row>
                </Col>
                <Col md={12}>
                    <Row className="mb-4">
                        <Col md={6}>
                            <Card className="shadow">
                                <Card.Body>
                                    <div id="chart-container">
                                        <Chart id="progressChart" type="line" data={dataTotalStudent} options={progressConfig.options} />
                                    </div>
                                </Card.Body>
                            </Card>
                        </Col>
                        <Col md={6}>
                            <Card className="shadow">
                                <Card.Body>
                                    <div id="time-chart-container">
                                        <Chart id="timeChart" type="bar" data={dataTime} options={timeConfig.options} />
                                    </div>
                                </Card.Body>
                            </Card>
                        </Col>
                    </Row>
                </Col>
                <Col md={12}>
                    <Row>
                        <Col md={2}>
                            <Card>
                                <Card.Body>
                                    <div className="card-item">
                                        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                            <h5 className="mb-4">Khối đang xem</h5>
                                        </div>
                                        {managedGrades.length > 0 ? (
                                            <select value={selectedGrade?.id} onChange={handleGradeChange}>
                                                {managedGrades.map((grade) => (
                                                    <option key={grade.id} value={grade.id}>
                                                        {grade.name}
                                                    </option>
                                                ))}
                                            </select>
                                        ) : (
                                            <p>Không có khối nào để quản lý</p>
                                        )}
                                    </div>
                                </Card.Body>
                            </Card>

                        </Col>
                        <Col md={5}>
                            <Card>
                                <Card.Body>
                                    <div className="card-item">
                                        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                            <h5 className="mb-4">Danh sách các chương</h5>
                                            <summary><p className='cursor-pointer' onClick={() => navigate(`/content_manage/grades/${selectedGrade.id}/list_tests`)}>Xem thêm</p></summary>
                                        </div>
                                        <div>
                                            {dashboardData.chapters.map((c, index) => (
                                                <div className="d-flex align-items-center mb-1" key={index}>
                                                    <div className="flex-grow-1">
                                                        <h6 className="mb-1">Chương {c.order}: {c.name}</h6>
                                                    </div>
                                                    <div className="text-end">
                                                        <div className="due-date">
                                                            <button className='btn btn-outline-primary btn-sm' onClick={() => navigate(`/content_manage/grades/${selectedGrade.id}/chapters/${c.id}`)}>Quản lý</button>
                                                        </div>
                                                    </div>
                                                </div>
                                            ))}
                                        </div>
                                    </div>
                                </Card.Body>
                            </Card>
                        </Col>
                        <Col md={5}>
                            <Card>
                                <Card.Body>
                                    <div className="card-item">
                                        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                            <h5 className="mb-4">Danh sách bài kiểm tra</h5>
                                            <summary><p className='cursor-pointer' onClick={() => navigate(`/content_manage/grades/${selectedGrade.id}/list_tests`)}>Xem thêm</p></summary>
                                        </div>
                                        <div>
                                            {dashboardData.testDashboards.map((t, index) => (
                                                <div className="d-flex align-items-center mb-1" key={index}>
                                                    <div className="flex-grow-1">
                                                        <h6 className="mb-1">{t.testName}</h6>
                                                    </div>
                                                    <div className="text-end">
                                                        <div className="due-date">
                                                            <button className='btn btn-outline-primary btn-sm' onClick={() => navigate(`/content_manage/grades/${selectedGrade.id}/list_tests/${t.id}/questions`)}>Quản lý</button>
                                                        </div>
                                                    </div>
                                                </div>
                                            ))}
                                        </div>
                                    </div>
                                </Card.Body>
                            </Card>

                        </Col>

                    </Row>
                </Col>
            </Row>
        </div>
    );
};

export default ContentManageDashboard; 
