import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { authFetch } from "../../utils/authFetch";
import { fetchGrades } from "../../services/GradeService";
import "../ChooseRole.css";

const ChooseGrade = () => {
  const [grades, setGrades] = useState([]);
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const userId = localStorage.getItem('userId');
  const apiUrl = process.env.REACT_APP_API_URL;  
  useEffect(() => {
    const fetchGrade = async () => {
      try {
        setLoading(true);
        const data = await fetchGrades(); 
        setGrades(data); 
      } catch (error) {
        console.error("Lỗi khi lấy danh sách lớp:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchGrade();
  }, []);
  
  const handleGradeSelect = async (gradeId) => {
    setLoading(true); 
    try {
      const response = await authFetch(`${apiUrl}/api/User/update-grade/${userId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(gradeId), 
      });
      
      const data = await response.json();
      if (!response.ok) {
        throw new Error(data?.message || "Cập nhật lớp thất bại");
      }
      alert("Cập nhật lớp thành công")
      localStorage.setItem('gradeId', gradeId);
      navigate('/student/dashboard', { replace: true });
    } catch (error) {
      console.error("Lỗi:", error);
      alert(error.message);
    } finally {
      setLoading(false); 
    }
  };

  return (
    <div className="choose-role-container">
      <div className="choose-role">
        <h2 className="">Vui lòng chọn đúng khối bạn đang học</h2>
        {loading ? (
          <p>Đang tải...</p>
        ) : (
          <div className="choose-role-btn">
            {grades.filter(g => g.gradeIsVisible).map((grade) => (
              <button
                key={grade.gradeId}
                onClick={() => handleGradeSelect(grade.gradeId)}
                disabled={loading}
              >
                {grade.gradeName}
              </button>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default ChooseGrade;
