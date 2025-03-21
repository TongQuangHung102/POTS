import React, { useEffect, useRef, useState } from 'react';
import { Card, Row, Col, Table } from 'react-bootstrap';
import { Chart as ChartJS, LineElement, PointElement, CategoryScale, LinearScale, Title, Tooltip, Legend, BarElement } from 'chart.js';
import { Chart } from 'react-chartjs-2';
import { BiCube, BiSolidUserCheck } from "react-icons/bi";
import { fetchGrades } from '../services/GradeService';
ChartJS.register(LineElement, PointElement, CategoryScale, LinearScale, Title, Tooltip, Legend, BarElement);

const AdminDashboard = () => {
  const [dashboardData, setDashboardData] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [dataTotalStudent, setDataTotalStudent] = useState({});
  const [dataTime, setDataTime] = useState({});

  const [grades, setGrades] = useState([]);
  const [gradeId, setGradeId] = useState(null)

  useEffect(() => {
    setIsLoading(true);
    fetch(`https://localhost:7259/api/Dashboard/admin-dashboard${gradeId ? `?gradeId=${gradeId}` : ''}`)
      .then(res => res.json())
      .then(data => {
        setDashboardData(data);
        setIsLoading(false);
        console.log(data);

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
          labels: data.activityTime.labels,
          datasets: [{
            label: "Tổng Thời Gian Luyệt Tập (Phút)",
            data: data.activityTime.data,
            backgroundColor: "rgba(75, 192, 192, 0.2)",
            borderColor: "rgba(75, 192, 192, 1)",
            borderWidth: 1
          }]
        })
      });
  }, [gradeId]);

  useEffect(() => {
    const fetchData = async () => {
      const data = await fetchGrades(); // Chờ dữ liệu trả về
      if (data) setGrades(data); // Cập nhật state nếu có dữ liệu
    };

    fetchData(); // Gọi hàm async
  }, []);

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
                      <h5 className="mb-4">Khối</h5>
                    </div>
                    <div>
                      <select value={gradeId ?? ""} onChange={e => setGradeId(e.target.value || null)}>
                        <option value=''>Tất cả lớp</option>
                        {grades.map(cls => (
                          <option key={cls.gradeId} value={cls.gradeId}>
                            {cls.gradeName}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                </Card.Body>
              </Card>
            </Col>
            <Col md={6}>
              <Card>
                <Card.Body>
                  <div className="card-item">
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                      <h5 className="mb-4">Quản lý khối</h5>
                      <p>Quản lý</p>
                    </div>
                    <Table hover responsive>
                      <thead>
                        <tr>
                          <th>Tên</th>
                          <th>Email</th>
                          <th>Khối quản lý</th>
                        </tr>
                      </thead>
                      <tbody>
                        {dashboardData.contentManageAssigns.map((user, index) => (
                          <tr key={index}>
                            <td>
                              <div className="d-flex align-items-center">
                                <BiSolidUserCheck size={20}></BiSolidUserCheck >
                                <div>{user.name}</div>
                              </div>
                            </td>
                            <td>{user.email}</td>
                            <td>{user.gradeAssign}</td>
                          </tr>
                        ))}
                      </tbody>

                    </Table>
                  </div>
                </Card.Body>
              </Card>
            </Col>
            <Col md={4}>
              <Card>
                <Card.Body>
                  <div className="card-item">
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                      <h5 className="mb-4">Danh sách các gói</h5>
                      <p>Xem thêm</p>
                    </div>
                    <div>
                      {dashboardData.subscriptionPlanDashboards.map((plan, index) => (
                        <div className="d-flex align-items-center mb-1" key={index}>
                          <div className="submission-icon">
                            <BiCube size={30}  ></BiCube>
                          </div>
                          <div className="flex-grow-1">
                            <h6 className="mb-1">{plan.name}</h6>
                          </div>
                          <div className="text-end">
                            <div className="due-date">
                              <i className="bi bi-cash"></i> {formatCurrency(plan.price)}
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

export default AdminDashboard; 
