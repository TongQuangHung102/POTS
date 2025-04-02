import React, { useEffect, useRef, useState } from 'react';
import { useNavigate } from "react-router-dom";
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, ArcElement, Tooltip } from 'chart.js';
import { Bar, Line } from 'react-chartjs-2';
import '../../pages/StudentDashboard.css';
import { getSubjectGradesByGrade } from '../../services/SubjectGradeService';
import { formatDateTime } from '../../utils/timeUtils';
ChartJS.register(CategoryScale, LinearScale, BarElement, ArcElement, Tooltip);

const Report = () => {
  const [dashboardData, setDashboardData] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isData, setIsData] = useState(true);
  const [rateData, setRateData] = useState({});
  const [activityOptions, setActivityOptions] = useState({});
  const [rateOptions, setRateOptions] = useState({});
  const [activityData, setActivityData] = useState({});
  const [subjects, setSubjects] = useState([]);
  const [subjectGradeId, setSubjectGradeId] = useState(null);


  const [managedGrades, setManagedGrades] = useState(() => {
    const savedGrades = sessionStorage.getItem("managedGrades");
    return savedGrades ? JSON.parse(savedGrades) : [];
  });

  const [selectedGrade, setSelectedGrade] = useState(() => {
    return managedGrades.length > 0 ? managedGrades[0] : null;
  });

  useEffect(() => {
    sessionStorage.setItem("managedGrades", JSON.stringify(managedGrades));
    if (managedGrades.length > 0 && !selectedGrade) {
      setSelectedGrade(managedGrades[0]);
    }
    if (managedGrades.length === 0) {
      setIsData(false);
    }
  }, [managedGrades, dashboardData]);

  const fetchSubjectGrades = async (gradeId) => {
    try {
      const data = await getSubjectGradesByGrade(gradeId);
      setSubjects(data);
      if (data.length > 0) {
        setSubjectGradeId(data[0].id);
      }
    } catch (error) {
      console.error("Có lỗi khi lấy dữ liệu lớp", error);
    }
  };

  //lay mon hoc dua tren gradeId
  useEffect(() => {
    if (selectedGrade) {
      fetchSubjectGrades(selectedGrade.id);
    }
  }, [selectedGrade]);

  const handleGradeChange = (event) => {
    const gradeId = parseInt(event.target.value, 10);
    const grade = managedGrades.find(g => g.id === gradeId);
    setSelectedGrade(grade);
};


  const navigate = useNavigate();

  useEffect(() => {
    if (subjectGradeId) { // Chỉ fetch khi subjectGradeId đã có giá trị
      setIsLoading(true);
      fetch(`https://localhost:7259/api/Report/dashboard?subjectGradeId=${subjectGradeId}`)
        .then(res => res.json())
        .then(data => {
          setDashboardData(data);
          setIsLoading(false);
          console.log(subjectGradeId);

          setActivityData({
            labels: data.totalReportByReason.labels,
            datasets: [{
              label: "Số lượng báo cáo",
              data: data.totalReportByReason.data,
              backgroundColor: '#0099FF',
              borderRadius: 5
            }]
          });

          setRateData({
            labels: data.rateRoports.labels,
            datasets: [{
              label: "Tỷ lệ báo cáo",
              data: data.rateRoports.data,
              borderColor: '#FF6666',
              backgroundColor: 'rgba(255, 102, 102, 0.2)',
              tension: 0.3,
              fill: true
            }]
          });

          setActivityOptions({
            plugins: {
              title: {
                display: true,
                text: "Biểu đồ",
                font: { size: 13, weight: "bold" },
              },
              legend: { display: true, padding: 20 }
            },
            scales: { y: { beginAtZero: true } }
          });

          setRateOptions({
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
        })
        .catch(err => {
          console.error("Lỗi:", err);
          setIsLoading(false);
        });
    }
  }, [subjectGradeId]); // Chạy khi subjectGradeId thay đổi

  const statusMap = {
    Pending: "Chưa xem xét",
    Reject: "Không hợp lệ",
    Resolved: "Hợp lệ"
};



  const activityCardRef = useRef(null);

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
            {managedGrades.length > 0 ? (
              <select class="stats-select" value={selectedGrade?.id} onChange={handleGradeChange}>
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
          <div class="col-md-3">
            {subjects.length > 0 ? (
              <select class="stats-select" value={subjectGradeId} onChange={(e) => {
                setSubjectGradeId(e.target.value);
              }}>
                {subjects.map((subject) => (
                  <option key={subject.id} value={subject.id}>{subject.name}</option>
                ))}
              </select>
            ) : (
              <p>Không có môn học nào</p>
            )}
          </div>
        </div>
        <div className="row">
          {/* Stats Row */}
          <div className="col-md-12">
            <div className="row mb-3">
              <div className="col">
                <div className="stats-card">
                  <small className="text-danger">Số báo cáo</small>
                  <div className="stats-number">{dashboardData.totalReport}</div>
                </div>
              </div>
              <div className="col">
                <div className="stats-card">
                  <small className="text-primary">Số báo cáo hợp lệ</small>
                  <div className="stats-number">{dashboardData.validReport}</div>
                </div>
              </div>
              <div className="col">
                <div className="stats-card">
                  <small className="text-success">Số báo cáo không hợp lệ</small>
                  <div className="stats-number">{dashboardData.inValidReport}</div>
                </div>
              </div>
              <div className="col">
                <div className="stats-card">
                  <small className="text-warning">Số báo cáo chưa giải quyết</small>
                  <div className="stats-number">{dashboardData.pendingReport}</div>
                </div>
              </div>
            </div>

            {/* Activity and Performance */}
            <div className="row">
              <div className="col-md-6">
                <div className="card" ref={activityCardRef}>
                  <div className="card-body">
                    <div className="activity-chart">
                      <Line data={rateData} options={rateOptions} />
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
              <div className='col-md-12'>
                <div className="upcoming-submission-report">
                  <div class="student-list">
                  <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                      <h5 className="mb-4">Danh sách báo cáo</h5>
                      <p onClick={() => navigate('/content_manage/report/list')} style={{ cursor: "pointer" }}>Chi tiết</p>
                    </div>
                    <table>
                      <thead>
                        <tr>
                          <th style={{ width: "10%" }}>Id</th>
                          <th style={{ width: "35%" }}>Lý do</th>
                          <th style={{ width: "15%" }}>Số lần</th>
                          <th style={{ width: "15%" }}>Id câu hỏi</th>
                          <th style={{ width: "25%" }}>Trạng thái</th>
                        </tr>
                      </thead>
                      <tbody>
                        {dashboardData.reportInDashboards.length > 0 ? (dashboardData.reportInDashboards.map(st => (
                          <tr>
                            <td>{st.id}</td>
                            <td>{st.reason}</td>
                            <td>{st.reportCount}</td>
                            <td>{st.questionId}</td>
                            <td>{statusMap[st.status] || "Không xác định"}</td>
                          </tr>
                        ))
                        ) : (
                          <tr>
                            <td colSpan="5" style={{ textAlign: "center", fontStyle: "italic", color: "gray" }}>
                              Chưa có báo cáo nào.
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
      </div>
    </div>
  );
}

export default Report; 