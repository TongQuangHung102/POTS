import React, { useEffect, useRef } from 'react';
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, ArcElement, Tooltip } from 'chart.js';
import { Bar, Doughnut } from 'react-chartjs-2';
import './StudentDashboard.css';
import Header from '../components/Header';
ChartJS.register(CategoryScale, LinearScale, BarElement, ArcElement, Tooltip);

const StudentDashboard = () => {
  const activityData = {
    labels: ['Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7', 'Chủ Nhật'],
    datasets: [{
      data: [3, 5, 4, 4, 3, 3, 4],
      backgroundColor: '#99CCFF',
      borderRadius: 5
    }]
  };

  const activityCardRef = useRef(null);
  const performanceCardRef = useRef(null);
  const previousWidthRef = useRef(window.innerWidth);

  const activityOptions = {
    plugins: {
      legend: {
        display: false
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        max: 5,
        ticks: {
          stepSize: 1
        }
      }
    }
  };

  const performanceData = {
    datasets: [{
      data: [75, 25],
      backgroundColor: ['#99CCFF', '#f8f9fa'],
      borderWidth: 0
    }]
  };

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
    adjustCardHeight();

    window.addEventListener('resize', adjustCardHeight);
    const resizeInterval = setInterval(monitorResize, 300);

    return () => {
      window.removeEventListener('resize', adjustCardHeight);
      clearInterval(resizeInterval);
    };
  }, []);

  return (
    <div className="dashboard-container bg-light">
      <div className="main-content">
        <div class="row">
          <Header />
        </div>
        <div className="row">
          {/* Stats Row */}
          <div className="col-md-9">
            <div className="row mb-4">
              <div className="col-md-3">
                <div className="stats-card">
                  <small className="text-danger">Bài tập đã hoàn thành</small>
                  <div className="stats-number">15</div>
                </div>
              </div>
              <div className="col-md-3">
                <div className="stats-card">
                  <small className="text-primary">Điểm trung bình</small>
                  <div className="stats-number">90</div>
                </div>
              </div>
              <div className="col-md-3">
                <div className="stats-card">
                  <small className="text-success">Thời gian trung bình</small>
                  <div className="stats-number">70</div>
                </div>
              </div>
              <div className="col-md-3">
                <div className="stats-card">
                  <small className="text-warning">Phân loại</small>
                  <div className="stats-number">Giỏi</div>
                </div>
              </div>
            </div>

            {/* Activity and Performance */}
            <div className="row">
              <div className="col-md-8">
                <div className="card" ref={activityCardRef}>
                  <div className="card-body">
                    <div className="d-flex justify-content-between align-items-center">
                      <h5 className="card-title">Thời gian hoạt động</h5>
                      <select className="form-select" style={{ width: "auto" }}>
                        <option>Tuần</option>
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
                      <Doughnut data={performanceData} options={performanceOptions} />
                    </div>
                    <div className="text-center mt-3">
                      <small className="text-success">5th trong khối 9</small>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Upcoming Submission Section */}
            <div className="upcoming-submission">
              <h5 className="mb-4">Khóa học đang tham gia</h5>
              <div className="d-flex align-items-center">
                <div className="submission-icon">
                  <i className="bi bi-file-earmark-text text-primary fs-4"></i>
                </div>
                <div className="flex-grow-1">
                  <h6 className="mb-1">Khối lớp 5</h6>
                  <small className="text-muted d-block">Môn Toán</small>
                </div>
                <div className="text-end">
                  <div className="due-date">
                    <i className="bi bi-calendar me-1"></i>
                    Ngày bắt đầu
                  </div>
                  <small className="text-muted">Thu 21 April 2022</small>
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
                    Người dùng 1 <i className="bi bi-patch-check-fill text-primary"></i>
                  </h5>
                  <p>Học sinh</p>
                  <a href="#profile">Profile</a>
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