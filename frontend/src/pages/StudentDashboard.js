import React, { useEffect, useRef, useState } from 'react';
import { useNavigate } from "react-router-dom";
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, ArcElement, Tooltip } from 'chart.js';
import { Bar, Doughnut, Line } from 'react-chartjs-2';
import { BiBookmark } from "react-icons/bi";
import { ProgressBar } from "react-bootstrap";
import './StudentDashboard.css';
ChartJS.register(CategoryScale, LinearScale, BarElement, ArcElement, Tooltip);

const StudentDashboard = () => {
  const [dashboardData, setDashboardData] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [activityData, setActivityData] = useState({});
  const [scoreData, setScoreData] = useState({});
  const [activityOptions, setActivityOptions] = useState({});
  const [scoreOptions, setScoreOptions] = useState({});
  const [rank, setRank] = useState();

  const userId = sessionStorage.getItem('userId');
  const gradeId = sessionStorage.getItem('gradeId')

  const navigate = useNavigate();

  useEffect(() => {
    setIsLoading(true);
    fetch(`https://localhost:7259/api/Dashboard/student-dashboard/${userId}`)
      .then(res => res.json())
      .then(data => {
        setDashboardData(data);
        setIsLoading(false);
        console.log(data);

        setActivityData({
          labels: data.activity.labels,
          datasets: [{
            label: "Tổng thời gian luyện tập",
            data: data.activity.data,
            backgroundColor: '#0099FF',
            borderRadius: 5
          }]
        });

        setScoreData({
          labels: data.scoreTime.labels,
          datasets: [{
            label: "Điểm trung bình",
            data: data.scoreTime.scoreData,
            borderColor: '#FF6666',
            backgroundColor: 'rgba(255, 102, 102, 0.2)',
            tension: 0.3, 
            fill: true
          }
          ,
          {
            label: "Thời gian trung bình 1 câu(s)",
            data: data.scoreTime.timeData, 
            borderColor: '#0099FF', 
            backgroundColor: 'rgba(0, 153, 255, 0.2)',
            tension: 0.3,
            fill: false
          }]
        });

        setActivityOptions({
          plugins: {
            title: {
              display: true, 
              text: "Biểu đồ tổng thời gian luyện tập",
              font: {
                size: 13, 
                weight: "bold" 
              },
            },
             legend: { display: true,padding: 20 } 
            },
          scales: {
            y: {
              beginAtZero: true,
            }
          }
        });

        setScoreOptions({
          plugins: { 
            title: {
              display: true, 
              text: "Biểu đồ điểm trung bình và thời gian làm bài", 
              font: {
                size: 13, 
                weight: "bold" 
              },
            },
            legend: { 
            display: true, 
            position: "top",
            padding: 20
          }
        },
          scales: {
            y: {
              beginAtZero: true,
            }
          }
        });

        setRank(
          data.percentiles[1]
        );
      })
      .catch(err => {
        console.error("Lỗi:", err);
        setIsLoading(false);
      });
  }, []);


  const activityCardRef = useRef(null);
  const performanceCardRef = useRef(null);

  if (isLoading) {
    return <div className="loading-dashboard">Đang tải dữ liệu...</div>;
  }
  if (!dashboardData) {
    return <div className="loading-dashboard">Không có dữ liệu hiển thị</div>;
  }

  return (
    <div className="dashboard-container bg-light">
      <div className="main-content">
        <div className="row">
          {/* Stats Row */}
          <div className="col-md-12">
            <div className="row mb-4">
              <div className="col">
                <div className="stats-card">
                  <small className="text-danger">Số lần luyện tập</small>
                  <div className="stats-number">{dashboardData.practiceNumber}</div>
                </div>
              </div>
              <div className="col">
                <div className="stats-card">
                  <small className="text-primary">Điểm trung bình</small>
                  <div className="stats-number">{dashboardData.avg_score}/10</div>
                </div>
              </div>
              <div className="col">
                <div className="stats-card">
                  <small className="text-success">Thời gian trung bình mỗi câu</small>
                  <div className="stats-number">{dashboardData.avg_time}s</div>
                </div>
              </div>
              <div className="col">
                <div className="stats-card">
                  <small className="text-warning">Xếp loại</small>
                  <div className="stats-number">{dashboardData.rate}</div>
                </div>
              </div>
              <div className="col">
                <div className="stats-card">
                  <small className="text-info">Xếp hạng</small>
                  <div className='mt-3' ref={performanceCardRef}>
                    <ProgressBar now={rank} max={100} variant="success" title={`Xếp hạng: ${dashboardData.rank}`} />
                  </div>
                </div>
              </div>
            </div>

            {/* Activity and Performance */}
            <div className="row">
              <div className="col-md-6">
                <div className="card" ref={activityCardRef}>
                  <div className="card-body">
                    <div className="activity-chart">
                      <Line data={scoreData} options={scoreOptions} />
                    </div>
                  </div>
                </div>
              </div>

              <div className="col-md-6">
                <div className="card" ref={activityCardRef}>
                  <div className="card-body">
                    <div className="activity-chart">
                      <Bar data={activityData} options={activityOptions} />
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Upcoming Submission Section */}

            <div className='row'>
              <div className='col-md-8'>
                <div className="upcoming-submission">
                  <h5 className="mb-4">Thông tin khối</h5>
                  <div className="d-flex align-items-center">
                    <div className="submission-icon">
                      <BiBookmark size={30} />
                    </div>
                    <div className="flex-grow-1">
                      <h6 className="mb-1">{dashboardData.student.gradeName}</h6>
                      <small className="text-muted d-block">Môn Toán</small>
                    </div>
                    <div className="text-end">
                      <div className="due-date">
                        <i className="bi bi-calendar me-1"></i>
                        <button className='btn btn-primary' onClick={() => navigate("/student/course")}>Luyện tập</button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div className='col-md-4'>
                <div className="upcoming-submission text-center ">
                  <h5>Gói cơ bản</h5>
                  <div>
                    <small>Sử dụng phiên bản cao hơn để trải nghiệm thêm tính năng</small>
                    <button className="btn btn-primary mt-2 d-block mx-auto">Nâng cấp</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default StudentDashboard; 