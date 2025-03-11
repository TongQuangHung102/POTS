import React, { useEffect, useRef, useState } from 'react';
import { useNavigate } from "react-router-dom";
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, ArcElement, Tooltip } from 'chart.js';
import { Bar, Doughnut } from 'react-chartjs-2';
import { BiBookmark } from "react-icons/bi";
import './StudentDashboard.css';
ChartJS.register(CategoryScale, LinearScale, BarElement, ArcElement, Tooltip);

const StudentDashboard = () => {
  const [dashboardData, setDashboardData] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [activityData, setActivityData] = useState({});
  const [activityOptions, setActivityOptions] = useState({});
  const [rank, setRank] = useState({});

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
            data: data.activity.data,
            backgroundColor: '#99CCFF',
            borderRadius: 5
          }]
        });
  
        setActivityOptions({
          plugins: { legend: { display: false } },
          scales: {
            y: {
              beginAtZero: true,
              max: Math.ceil(data.avg_time * 20),
              ticks: { stepSize: 1 }
            }
          }
        });
  
        setRank({
          datasets: [{
            data: [data.percentiles[1], data.percentiles[0]],
            backgroundColor: ['#99CCFF', '#f8f9fa'],
            borderWidth: 0
          }]
        });
      })
      .catch(err => {
        console.error("Lỗi:", err);
        setIsLoading(false);
      });
  }, []);
  

  const activityCardRef = useRef(null);
  const performanceCardRef = useRef(null);
  const previousWidthRef = useRef(window.innerWidth);


  const performanceOptions = {
    responsive: true,
    maintainAspectRatio: true,
    cutout: '80%',
    plugins: {
      legend: {
        display: false
      }
    }
  };

  const adjustCardHeight = () => {
    if (activityCardRef.current && performanceCardRef.current) {
      const col1 = activityCardRef.current;
      const col2 = performanceCardRef.current;

      col1.style.height = "auto";
      col2.style.height = "auto";

      const maxHeight = Math.max(col1.offsetHeight, col2.offsetHeight);

      col1.style.height = `${maxHeight}px`;
      col2.style.height = `${maxHeight}px`;
    }
  };

  const monitorResize = () => {
    const currentWidth = window.innerWidth;

    if (currentWidth !== previousWidthRef.current) {
      previousWidthRef.current = currentWidth;
      adjustCardHeight();
    }
  };

  useEffect(() => {
    setTimeout(() => {
      adjustCardHeight();
    }, 200);

    window.addEventListener('resize', adjustCardHeight);
    const resizeInterval = setInterval(monitorResize, 300);

    return () => {
      window.removeEventListener('resize', adjustCardHeight);
      clearInterval(resizeInterval);
    };
  }, []);

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
          <div className="col-md-9">
            <div className="row mb-4">
              <div className="col-md-3">
                <div className="stats-card">
                  <small className="text-danger">Số lần luyện tập</small>
                  <div className="stats-number">{dashboardData.practiceNumber}</div>
                </div>
              </div>
              <div className="col-md-3">
                <div className="stats-card">
                  <small className="text-primary">Điểm trung bình</small>
                  <div className="stats-number">{dashboardData.avg_score}/10</div>
                </div>
              </div>
              <div className="col-md-3">
                <div className="stats-card">
                  <small className="text-success">Thời gian trung bình mỗi câu</small>
                  <div className="stats-number">{dashboardData.avg_time}s</div>
                </div>
              </div>
              <div className="col-md-3">
                <div className="stats-card">
                  <small className="text-warning">Xếp loại</small>
                  <div className="stats-number">{dashboardData.rate}</div>
                </div>
              </div>
            </div>

            {/* Activity and Performance */}
            <div className="row">
              <div className="col-md-8">
                <div className="card" ref={activityCardRef}>
                  <div className="card-body">
                    <div className="d-flex justify-content-between align-items-center">
                      <h5 className="card-title">Thời gian luyện tập(phút)</h5>
                      <select className="form-select" style={{ width: "auto" }}>
                        <option>7 ngày gần nhất</option>
                      </select>
                    </div>
                    <div className="activity-chart">
                      <Bar data={activityData} options={activityOptions} />
                    </div>
                  </div>
                </div>
              </div>
              <div className="col-md-4">
                <div className="card" ref={performanceCardRef}>
                  <div className="card-body">
                    <div className="d-flex justify-content-between align-items-center">
                      <h5 className="card-title">Xếp hạng</h5>
                    </div>
                    <div className="progress-circle">
                      <Doughnut data={rank} options={performanceOptions} />
                    </div>
                    <div className="text-center mt-3">
                      <small className="text-success">Thứ {dashboardData.rank} trong khối </small>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Upcoming Submission Section */}
            <div className="upcoming-submission">
              <h5 className="mb-4">Thông tin khối</h5>
              <div className="d-flex align-items-center">
                <div className="submission-icon">
                <BiBookmark size={30}/>
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

          {/* Right Sidebar */}
          <div className="col-md-3">
            <div className="row">
              {/* Profile Section */}
              <div className="card">
                <div className="card-body text-center">
                  <div className="position-relative d-inline-block">
                    <img
                      src="https://static.vecteezy.com/system/resources/thumbnails/008/442/086/small_2x/illustration-of-human-icon-user-symbol-icon-modern-design-on-blank-background-free-vector.jpg"
                      className="profile-image"
                      alt="User profile"
                    />
                  </div>
                  <h5 className="mt-3 mb-1">
                  {dashboardData.student.name}<i className="bi bi-patch-check-fill text-primary"></i>
                  </h5>
                  <p>Học sinh</p>
                  <a onClick={() => navigate("/student/profile")}>Profile</a>
                </div>
              </div>

              <div className="card mt-3">
                <div className="card-body text-center">
                  <h5>Gói cơ bản</h5>
                  <div className="mt-auto pt-5">
                    <div className="card p-3">
                      <small>Sử dụng phiên bản cao hơn để trải nghiệm thêm tính năg</small>
                      <button className="btn btn-outline-primary mt-2">Nâng cấp</button>
                    </div>
                  </div>
                </div>
              </div>

              <div className="card mt-3">
                <div className="card-body text-center">
                  <h5>Cuộc thi sắp diễn ra</h5>
                  <h6>Cuộc thi tuần 1</h6>
                  <p>2/5/2025 - 7/5/2025</p>
                  <button className="btn btn-outline-primary mt-2">Tham gia</button>
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