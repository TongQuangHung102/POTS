import React, { useEffect, useRef, useState } from 'react';
import { useNavigate } from "react-router-dom";
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, ArcElement, Tooltip } from 'chart.js';
import { Bar, Doughnut, Line } from 'react-chartjs-2';
import { BiMessageRoundedDetail } from "react-icons/bi";
import { ProgressBar } from "react-bootstrap";
import '../../pages/StudentDashboard.css';
import { getStudentsByUserId } from '../../services/ParentService';
import { formatDateTime } from '../../utils/timeUtils';
import NotificationDropdown from '../../components/Notification';
import { getNotifications } from '../../services/NotificationService';
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

  const [subjectGrades, setSubjectGrades] = useState([]);
  const [listStudent, setListStudent] = useState([]);

  const [gradeId, setGradeId] = useState();

  const userId = sessionStorage.getItem('userId');

  const [isOpen, setIsOpen] = useState(false);
  const [notifications, setNotifications] = useState([]);

  const navigate = useNavigate();

  useEffect(() => {
    if (subjectGradeId) { // Chỉ fetch khi subjectGradeId đã có giá trị
      setIsLoading(true);
      fetch(`https://localhost:7259/api/Dashboard/student-dashboard/user/${selectedStudent}/subjectGrade/${subjectGradeId}`)
        .then(res => res.json())
        .then(data => {
          setDashboardData(data);
          setIsLoading(false);
          console.log(subjectGradeId);

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
        });
    }
  }, [subjectGradeId,selectedStudent ]); // Chạy khi subjectGradeId thay đổi

  const fetchSubjectGrades = async () => {
    try {
      setIsLoading(true);
      const response = await fetch(`https://localhost:7259/api/SubjectGrade/get-subject-by-grade/${gradeId}`);
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
      if (studentData.length > 0) {
        setSelectedStudent(studentData[0].userId);
        setGradeId(studentData[0].gradeId)
      }
    } catch (error) {
      console.error("Có lỗi khi lấy dữ liệu lớp", error);
    }
  };

  useEffect(() => {
    const fetchNotifications = async () => {
      try {
        const data = await getNotifications(userId);
        setNotifications(data);
      } catch (error) {
        console.error("Lỗi khi tải thông báo:", error);
      }
    };
  
    fetchNotifications();
  }, [userId]);

  useEffect(() => {
    setIsLoading(true);
    fetchListStudent();
    setIsLoading(false);
  }, []);


  useEffect(() => {
    if (selectedStudent) {
      setIsLoading(true);
      fetchSubjectGrades();
      setIsLoading(false);
    }
  }, [selectedStudent]);



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
        <div class="row mb-3">
          <div class="col-md-3">
            <select class="stats-select" value={selectedStudent} onChange={e => setSelectedStudent(e.target.value || null)}>
              {listStudent.map(ls => (
                <option key={ls.userId} value={ls.userId}>
                  {ls.userName}
                </option>
              ))}
            </select>
          </div>
          <div class="col-md-3">
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
          <div class="col-md-6">
            <div className='notification'>
              <BiMessageRoundedDetail size={30} style={{ cursor: "pointer" }} onClick={() => setIsOpen(!isOpen)}/>
            </div>
          </div>
        </div>
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
              <div className='col-md-4'>
                <div className="upcoming-submission-parent">
                  <div className="card-item">
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                      <h5 className="mb-4">Gói đang sử dụng</h5>
                    </div>
                    <div className='package-using'>
                      <h5>Gói cơ bản</h5>
                      <p>(Nâng cấp ngay để trải nghiệm thêm nhiều tính năng)</p>
                      <button className='btn btn-primary'>Nâng cấp</button>
                    </div>
                  </div>
                </div>
              </div>
              <div className='col-md-8'>
                <div className="upcoming-submission-parent">
                  <div class="student-list">
                    <h5>Danh sách các con</h5>
                    <table>
                      <thead>
                        <tr>
                          <th style={{ width: "30%" }}>Tên</th>
                          <th style={{ width: "25%" }}>Khối</th>
                          <th style={{ width: "45%" }}>Lần đăng nhập gần nhất</th>
                        </tr>
                      </thead>
                      <tbody>
                        {listStudent.length > 0 ? (listStudent.map(st => (
                          <tr>
                            <td>{st.userName}</td>
                            <td>{st.gradeName}</td>
                            <td>{formatDateTime(st.lastLogin)}</td>
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

        {isOpen && <NotificationDropdown notifications={notifications}/>}
      </div>
    </div>
  );
}

export default ParentDashboard; 