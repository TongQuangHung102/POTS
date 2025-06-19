import React, { useEffect, useRef, useState } from 'react';
import { useNavigate } from "react-router-dom";
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, ArcElement, Tooltip } from 'chart.js';
import { Bar, Doughnut, Line } from 'react-chartjs-2';
import { BiMessageRoundedDetail } from "react-icons/bi";
import { ProgressBar } from "react-bootstrap";
import '../../pages/student/StudentDashboard.css';
import { getStudentsByUserId } from '../../services/ParentService';
import { formatDateTime } from '../../utils/timeUtils';
import NotificationDropdown from '../../components/Notification';
import { getNotifications, markAllAsRead } from '../../services/NotificationService';
import { initSignalR, stopSignalR } from '../../services/SignalRService';
import MonthSelector from '../../components/MonthSelector';
import { authFetch } from '../../utils/authFetch';
import { getUserSubscriptionPlan } from '../../services/UserService';
ChartJS.register(CategoryScale, LinearScale, BarElement, ArcElement, Tooltip);

const ParentDashboard = () => {
  const [dashboardData, setDashboardData] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [activityData, setActivityData] = useState({});
  const [scoreData, setScoreData] = useState({});
  const [activityOptions, setActivityOptions] = useState({});
  const [scoreOptions, setScoreOptions] = useState({});
  const [rank, setRank] = useState();
  const [subjectGradeId, setSubjectGradeId] = useState();
  const [selectedStudent, setSelectedStudent] = useState();
  const [plan, setPlan] = useState(null);
  const [errorMessage, setErrorMessage] = useState(null);

  const [subjectGrades, setSubjectGrades] = useState([]);
  const [listStudent, setListStudent] = useState([]);

  const [gradeId, setGradeId] = useState();

  const [month, setMonth] = useState(new Date().getMonth() + 1);
  const [year, setYear] = useState(new Date().getFullYear());

  const userId = localStorage.getItem('userId');

  const [isOpen, setIsOpen] = useState(false);
  const [isNotLink, setIsNotLink] = useState(false);
  const [notifications, setNotifications] = useState([]);
  const [hasUnreadNotification, setHasUnreadNotification] = useState(false);
  const [newNotification, setNewNotification] = useState(0);
  const isFirstRender = useRef(true);
  const apiUrl = process.env.REACT_APP_API_URL;

  const navigate = useNavigate();

  useEffect(() => {
    initSignalR(setNewNotification);

    return () => {
      stopSignalR();
    };
  }, []);

  useEffect(() => {
    if (subjectGradeId) {
      setIsLoading(true);
      authFetch(`${apiUrl}/api/Dashboard/student-dashboard/user/${selectedStudent}/subjectGrade/${subjectGradeId}?month=${month}&year=${year}`)
        .then(res => res.json())
        .then(data => {
          setDashboardData(data);
          setIsLoading(false);


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
          setErrorMessage("Không thể tải dữ liệu. Vui lòng thử lại sau.");
        });
    }
  }, [subjectGradeId, selectedStudent, month, year]);

  const fetchSubjectGrades = async () => {
    try {
      setIsLoading(true);
      const response = await authFetch(`${apiUrl}/api/SubjectGrade/get-subject-by-grade/${gradeId}`);
      const data = await response.json();
      setSubjectGrades(data);

      if (data.length > 0) {
        setSubjectGradeId(data[0].id);
      }
    } catch (error) {
      console.error("Có lỗi khi lấy dữ liệu lớp", error);
    }
  }

  const fetchListStudent = async () => {
    try {
      const studentData = await getStudentsByUserId(userId);
      setListStudent(studentData);

      const activeStudent = studentData.find(student => student.isActive === true && student.gradeId != null);

      if (activeStudent) {
        setSelectedStudent(activeStudent.userId);
        setGradeId(activeStudent.gradeId);
      }
    } catch (error) {
      console.error("Có lỗi khi lấy dữ liệu ", error);
    }
  };


  useEffect(() => {
    const fetchNotifications = async () => {
      try {
        const data = await getNotifications(userId);
        const hasUnread = data.some(noti => !noti.isRead);
        setHasUnreadNotification(hasUnread);
        setNotifications(data);
      } catch (error) {
        console.error("Lỗi khi tải thông báo:", error);
      }
    };

    fetchNotifications();
  }, [newNotification]);


  const fetchPlan = async () => {
    try {
      const result = await getUserSubscriptionPlan(selectedStudent);

      setPlan(result);
    } catch (err) {
      console.error("Lỗi khi tải gói", err);
    }
  };


  useEffect(() => {
    setIsLoading(true);
    fetchListStudent();
    setIsLoading(false);
  }, []);


  useEffect(() => {
    if (selectedStudent) {
      setIsLoading(true);
      fetchSubjectGrades();
      fetchPlan();
      setIsLoading(false);
    }
  }, [selectedStudent]);

  useEffect(() => {
    if (isFirstRender.current) {
      isFirstRender.current = false;
      return;
    }

    if (isOpen) return;

    const markRead = async () => {
      await markAllAsRead(userId);
      setNotifications(prev =>
        prev.map(n => ({ ...n, isRead: true }))
      );
      setHasUnreadNotification(false);
    };

    markRead();
  }, [isOpen]);

  const handleMonthChange = (value) => {
    const [newYear, newMonth] = value.split('-'); // Lấy year và month từ value
    setYear(parseInt(newYear));
    setMonth(parseInt(newMonth));
  };

  const activityCardRef = useRef(null);
  const performanceCardRef = useRef(null);

  if (!gradeId) {
    return <div className="loading-dashboard">Không có dữ liệu do chưa liên kết với tài khoản nào của con hoặc con chưa học lần nào!</div>;
  }
  if (errorMessage) {
    return <div className="error-message">{errorMessage}</div>;
  }
  return (
    <div className="dashboard-container">
      <div className="main-content">
        <div class="row mb-3">
          <div class="col-md-2">
            <select class="stats-select" value={selectedStudent} onChange={e => setSelectedStudent(e.target.value || null)}>
              {listStudent.filter(st => st.isActive).map(ls => (
                <option key={ls.userId} value={ls.userId}>
                  {ls.userName}
                </option>
              ))}
            </select>
          </div>
          <div class="col-md-2">
            <select class="stats-select" value={subjectGradeId} onChange={e => setSubjectGradeId(e.target.value || null)} >
              {subjectGrades && subjectGrades.length > 0 ? (
                subjectGrades.map(sg => (
                  <option key={sg.id} value={sg.id}>
                    {sg.name}
                  </option>
                ))
              ) : (
                <option disabled>Vui lòng chọn học sinh</option>
              )}
            </select>
          </div>
          <div class="col-md-2">
            <MonthSelector
              selectedMonth={`${year}-${month}`}
              onMonthChange={handleMonthChange} />
          </div>
          <div class="col-md-6">
            <div className='notification'>
              {hasUnreadNotification && (<p className='new-notification'>Bạn có thông báo mới!</p>)}
              <BiMessageRoundedDetail size={30} style={{ cursor: "pointer" }} onClick={() => setIsOpen(!isOpen)} />
            </div>
          </div>
        </div>
        {dashboardData ? (
          <div className="row">
            {/* Stats Row */}
            <div className="col-md-12">
              <div className="row mb-3">
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

              <div className='row'>
                <div className='col-md-4 d-flex'>
                  <div className="upcoming-submission-parent flex-fill">
                    <div className="card-item h-100">
                      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                        <h5 className="mb-4">Gói đang sử dụng</h5>
                      </div>
                      <div className='package-using'>
                        <h5>{plan.planName}</h5>
                        {plan.price === 0 ? (<p>(Nâng cấp ngay để trải nghiệm thêm nhiều tính năng)</p>)
                          : (
                            <p>Ngày hết hạn: {formatDateTime(plan.endDate)}</p>
                          )}
                        {plan.price === 0 ? (<button onClick={() => navigate('/parent/package')} className='btn btn-primary'>Nâng cấp</button>) : (<button onClick={() => navigate('/parent/package')} className='btn btn-primary'>Gia hạn</button>)}
                      </div>
                    </div>
                  </div>
                </div>
                <div className='col-md-8 d-flex'>
                  <div className="upcoming-submission-parent flex-fill">
                    <div className="student-list card-item h-100">
                      <h5>Danh sách các con</h5>
                      <table>
                        <thead>
                          <tr>
                            <th style={{ width: "15%" }}>Tên</th>
                            <th style={{ width: "15%" }}>Khối</th>
                            <th style={{ width: "30%" }}>Lần đăng nhập gần nhất</th>
                            <th style={{ width: "20%" }}>Xác thực tài khoản</th>
                            <th style={{ width: "20%" }}>Trạng thái tài khoản</th>
                          </tr>
                        </thead>
                        <tbody>
                          {listStudent.length > 0 ? (listStudent.map(st => (
                            <tr>
                              <td>{st.userName}</td>
                              <td>{st.gradeName || 'Chưa có'}</td>
                              <td>{formatDateTime(st.lastLogin)}</td>
                              <td>{st.isActive ? 'Đã xác thực' : 'Chưa xác thực'}</td>
                              <td>{st.isLocked ? 'Khóa' : 'Hoạt động'}</td>
                            </tr>
                          ))
                          ) : (
                            <tr>
                              <td colSpan="5" style={{ textAlign: "center", fontStyle: "italic", color: "gray" }}>
                                Chưa có học sinh nào.
                              </td>
                            </tr>
                          )}
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        ) : (
          <div className="loading-dashboard">Đang tải dữ liệu...</div>
        )}


        {isOpen && <NotificationDropdown notifications={notifications} />}
      </div>
    </div>
  );
}

export default ParentDashboard; 