import React from 'react';
import { Card, Row, Col, Table } from 'react-bootstrap';
import { Chart as ChartJS, LineElement, PointElement, CategoryScale, LinearScale, Title, Tooltip, Legend, BarElement } from 'chart.js';
import { Chart } from 'react-chartjs-2';
import Header from '../components/Header';

ChartJS.register(LineElement, PointElement, CategoryScale, LinearScale, Title, Tooltip, Legend, BarElement);

const AdminDashboard = () => {
  // Dữ liệu biểu đồ điểm số trung bình
  const progressData = {
    labels: ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7"],
    datasets: [{
      label: "Điểm số trung bình",
      data: [70, 75, 80, 85, 70, 46, 89],
      backgroundColor: "rgba(54, 162, 235, 0.2)",
      borderColor: "rgba(54, 162, 235, 1)",
      borderWidth: 2,
      fill: true,
      tension: 0.4
    }]
  };

  // Cấu hình cho biểu đồ điểm số trung bình
  const progressConfig = {
    type: 'line',
    data: progressData,
    options: {
      responsive: true,
      plugins: {
        legend: {
          position: 'top',
        },
        title: {
          display: true,
          text: 'Điểm trung bình của các chương'
        }
      },
      scales: {
        y: {
          beginAtZero: true,
          title: {
            display: true,
            text: 'Điểm số'
          }
        },
        x: {
          title: {
            display: true,
            text: 'Thời gian'
          }
        }
      }
    }
  };

  // Dữ liệu biểu đồ tổng thời gian học
  const timeData = {
    labels: ["Ngày 1", "Ngày 2", "Ngày 3", "Ngày 4", "Ngày 5", "Ngày 6", "Ngày 7"],
    datasets: [{
      label: "Tổng Thời Gian Học (giờ)",
      data: [5, 7, 6, 8, 9, 4, 7],
      backgroundColor: "rgba(75, 192, 192, 0.2)",
      borderColor: "rgba(75, 192, 192, 1)",
      borderWidth: 1
    }]
  };

  // Cấu hình cho biểu đồ tổng thời gian học
  const timeConfig = {
    type: 'bar',
    data: timeData,
    options: {
      responsive: true,
      plugins: {
        legend: {
          position: 'top',
        },
        title: {
          display: true,
          text: 'Tổng Thời Gian Học Trong 7 Ngày Gần Nhất'
        }
      },
      scales: {
        y: {
          beginAtZero: true,
          title: {
            display: true,
            text: 'Thời gian (giờ)'
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

  return (
    <div className="main-content">
      <Row>
        <Header/>
      </Row>
      <Row>
        <Col md={12}>
          <Row className="mb-4">
            <Col md={3}>
              <Card className="stats-card">
                <small className="text-danger">Tổng số học sinh</small>
                <div className="stats-number">15</div>
              </Card>
            </Col>
            <Col md={3}>
              <Card className="stats-card">
                <small className="text-primary">Tổng số phụ huynh</small>
                <div className="stats-number">90</div>
              </Card>
            </Col>
            <Col md={3}>
              <Card className="stats-card">
                <small className="text-primary">Tổng số câu hỏi</small>
                <div className="stats-number">90</div>
              </Card>
            </Col>
            <Col md={3}>
              <Card className="stats-card">
                <small className="text-primary">Tổng số phụ huynh</small>
                <div className="stats-number">90</div>
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
                    <Chart id="progressChart" type="line" data={progressData} options={progressConfig.options} />
                  </div>
                </Card.Body>
              </Card>
            </Col>
            <Col md={6}>
              <Card className="shadow">
                <Card.Body>
                  <div id="time-chart-container">
                    <Chart id="timeChart" type="bar" data={timeData} options={timeConfig.options} />
                  </div>
                </Card.Body>
              </Card>
            </Col>
          </Row>
        </Col>
        <Col md={12}>
          <Row>
            <Col md={7}>
              <Card>
                <Card.Body>
                  <div className="card-item">
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                      <h5 className="mb-4">Danh sách nhân viên</h5>
                      <p>Quản lý</p>
                    </div>
                    <Table striped bordered hover responsive>
                      <thead>
                        <tr>
                          <th>Name</th>
                          <th>Email</th>
                          <th>Assigned</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr>
                          <td>
                            <div className="d-flex align-items-center">
                              <img src="https://via.placeholder.com/40" alt="Aurora Logo" className="product-logo me-2" />
                              <div>Aurora</div>
                            </div>
                          </td>
                          <td>abc@gmail.com</td>
                          <td>Toán 5 - Chương 1, 2, 3</td>
                        </tr>
                        <tr>
                          <td>
                            <div className="d-flex align-items-center">
                              <img src="https://via.placeholder.com/40" alt="Bender Logo" className="product-logo me-2" />
                              <div>Bender</div>
                            </div>
                          </td>
                          <td>abc@gmail.com</td>
                          <td>Toán 5 - Chương 3, 4, 5</td>
                        </tr>
                      </tbody>
                    </Table>
                  </div>
                </Card.Body>
              </Card>
            </Col>
            <Col md={5}>
              <Card>
                <Card.Body>
                  <div className="card-item">
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                      <h5 className="mb-4">Danh sách các gói</h5>
                      <p>Xem thêm</p>
                    </div>
                    <div className="d-flex align-items-center mb-1">
                      <div className="submission-icon">
                        <i className="bi bi-box2"></i>
                      </div>
                      <div className="flex-grow-1">
                        <h6 className="mb-1">Cơ bản</h6>
                      </div>
                      <div className="text-end">
                        <div className="due-date">
                          <i className="bi bi-cash"></i>
                          200Đ
                        </div>
                      </div>
                    </div>
                    <div className="d-flex align-items-center mb-1">
                      <div className="submission-icon">
                        <i className="bi bi-box2"></i>
                      </div>
                      <div className="flex-grow-1">
                        <h6 className="mb-1">Nâng cao</h6>
                      </div>
                      <div className="text-end">
                        <div className="due-date">
                          <i className="bi bi-cash"></i>
                          500Đ
                        </div>
                      </div>
                    </div>
                    <div className="d-flex align-items-center mb-1">
                      <div className="submission-icon">
                        <i className="bi bi-box2"></i>
                      </div>
                      <div className="flex-grow-1">
                        <h6 className="mb-1">Miễn phí</h6>
                      </div>
                      <div className="text-end">
                        <div className="due-date">
                          <i className="bi bi-cash"></i>
                          0Đ
                        </div>
                      </div>
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

export default AdminDashboard; 
