import React, { useEffect, useRef, useState } from 'react';
import { useNavigate } from "react-router-dom";
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, ArcElement, Tooltip } from 'chart.js';
import { Bar, Line } from 'react-chartjs-2';
import { BiBookmark } from "react-icons/bi";
import { ProgressBar } from "react-bootstrap";
import MonthSelector from '../../components/MonthSelector';
import { useAuth } from '../../hooks/useAuth';
import { authFetch } from '../../utils/authFetch';
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
  const [subjectGradeId, setSubjectGradeId] = useState();

  const [subjectGrades, setSubjectGrades] = useState([]);

  const [month, setMonth] = useState(new Date().getMonth() + 1);
  const [year, setYear] = useState(new Date().getFullYear());

  const { user, loading } = useAuth();


  const navigate = useNavigate();
  const apiUrl = process.env.REACT_APP_API_URL;

  useEffect(() => {
    if (subjectGradeId) { 
      setIsLoading(true);
      authFetch(`${apiUrl}/api/Dashboard/student-dashboard/user/${user.userId}/subjectGrade/${subjectGradeId}?month=${month}&year=${year}`)
        .then(res => res.json())
        .then(data => {
          setDashboardData(data);
          setIsLoading(false);

          setActivityData({
            labels: data.activity.labels,
            datasets: [{
              label: "Tổng thời gian luyện tập(phút)",
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
            },
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
                font: { size: 13, weight: "bold" },
              },
              legend: { display: true, padding: 20 }
            },
            scales: { y: { beginAtZero: true } }
          });

          setScoreOptions({
            plugins: {
              title: {
                display: true,
                text: "Biểu đồ điểm trung bình và thời gian làm bài",
                font: { size: 13, weight: "bold" },
              },
              legend: { display: true, position: "top", padding: 20 }
            },
            scales: { y: { beginAtZero: true } }
          });

          setRank(data.percentiles[1]);
        })
        .catch(err => {
          console.error("Lỗi:", err);
          setIsLoading(false);
        });
    }
  }, [subjectGradeId, month, year]); // Chạy khi subjectGradeId thay đổi

  const fetchSubjectGrades = async () => {
    try {
      setIsLoading(true);
      const response = await authFetch(`${apiUrl}/api/SubjectGrade/get-subject-by-grade/${user.gradeId}`);
      const data = await response.json();
      setSubjectGrades(data);

      if (data.length > 0) {
        setSubjectGradeId(data[0].id);
      }
    } catch (error) {
      console.error("Có lỗi khi lấy dữ liệu lớp", error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if(!loading){
      fetchSubjectGrades();
    }
  }, [loading, user]);

  const handleMonthChange = (value) => {
    const [newYear, newMonth] = value.split('-');
    setYear(parseInt(newYear));
    setMonth(parseInt(newMonth));
  };



  const activityCardRef = useRef(null);
  const performanceCardRef = useRef(null);

  if (isLoading) {
    return <div className="loading-dashboard">Đang tải dữ liệu...</div>;
  }
  if (!dashboardData) {
    return <div className="loading-dashboard">Không có dữ liệu hiển thị</div>;
  }

  return (
    <div className="dashboard-container">
      <div className="main-content">
        <div className="row">
          <div className="col-md-12">
            <div className="row mb-3">
              <div className="col-md-3">
                {subjectGrades.length > 0 ? (
                  <select className="stats-select" value={subjectGradeId} onChange={(e) => setSubjectGradeId(Number(e.target.value))}>
                    {subjectGrades.map((g) => (
                      <option key={g.id} value={g.id}>
                        {g.name}
                      </option>
                    ))}
                  </select>
                ) : (
                  <p>Không có khối nào để quản lý</p>
                )}
              </div>
              <div className="col-md-3">
                <MonthSelector
                  selectedMonth={`${year}-${month}`}
                  onMonthChange={handleMonthChange} />
              </div>
            </div>
          </div>
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
                    <ProgressBar now={dashboardData.percentiles[0]} max={100} variant="success" title={`Xếp hạng: ${dashboardData.rank}`} />
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

            <div className='row mb-4'>
              <div className='col-md-4'>
                <div className="upcoming-submission">
                  <div className="card-item">
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                      <h5 className="mb-4">Thông báo</h5>
                    </div>
                    <p className='system-message'>{dashboardData.message}</p>
                  </div>
                </div>
              </div>
              <div className='col-md-8'>
                <div className="upcoming-submission">
                  <h5 className="mb-4">Thông tin khối</h5>
                  <div className="d-flex align-items-center">
                    <div className="submission-icon">
                      <BiBookmark size={30} />
                    </div>
                    <div className="flex-grow-1">
                      <h6 className="mb-1">{dashboardData.student.gradeName}</h6>
                    </div>
                    <div className="text-end">
                      <div className="due-date">
                        <i className="bi bi-calendar me-1"></i>
                        <button className='btn btn-primary' onClick={() => navigate(`/student/grade/${user.gradeId}`)}>Luyện tập</button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          {/* <div className='col-md-12'>
            <div className='leaderboard'>
              <h3 className='leaderboard-title'>Bảng xếp hạng</h3>
              <div className='row'>
                <div className='col-md-6'>
                  <div className="leaderboard-container">
                    <div className="time-filter">
                      Bảng xếp hạng điểm
                    </div>

                    <table className="leaderboard-table">
                      <tbody>
                        {leaderboardData.map((user) => (
                          <tr key={user.rank} className="leaderboard-row">
                            <td className="rank-cell">
                              <div className='rank-custom'>{user.rank}</div>
                            </td>
                            <td className="user-cell">
                              <div className="user-info">
                                <span className="user-name">{user.name}</span>
                                <span className="user-points">{user.points.toLocaleString()}</span>
                              </div>
                            </td>
                            <td className="score-cell">{user.score}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                    <div className="time-filter">
                      Thứ hạng của bạn: 300
                    </div>
                  </div>
                </div>
                <div className='col-md-6'>
                  <div className="leaderboard-container">
                    <div className="time-filter">
                      Bảng xếp hạng chăm chỉ
                    </div>

                    <table className="leaderboard-table">
                      <tbody>
                        {leaderboardData.map((user) => (
                          <tr key={user.rank} className="leaderboard-row">
                            <td className="rank-cell">
                              <div className='rank-custom'>{user.rank}</div>
                            </td>
                            <td className="user-cell">
                              <div className="user-info">
                                <span className="user-name">{user.name}</span>
                                <span className="user-points">{user.points.toLocaleString()}</span>
                              </div>
                            </td>
                            <td className="score-cell">{user.score}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                    <div className="time-filter">
                      Thứ hạng của bạn: 300
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div> */}
        </div>
      </div>
    </div>
  );
}

export default StudentDashboard; 