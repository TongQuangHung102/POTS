import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "../ChooseRole.css";

const ChooseGrade = () => {
  const [grades, setGrades] = useState([]);
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const userId = sessionStorage.getItem('userId');

  useEffect(() => {
    const fetchRoles = async () => {
      try {
        setLoading(true);
        const response = await fetch('https://localhost:7259/api/Grade/get-all-grade');
        const data = await response.json();

        // Kiểm tra xem data có phải là mảng không
        if (Array.isArray(data)) {
          setGrades(data);
        } else {
          console.error("Dữ liệu trả về không phải là mảng", data);
        }
      } catch (error) {
        console.error("Lỗi khi lấy danh sách lớp:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchRoles();
  }, []);

  const handleGradeSelect = async (gradeId) => {
    setLoading(true);
    try {
      const response = await fetch(`https://localhost:7259/api/User/update-grade/${userId}`, {
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
      sessionStorage.setItem("gradeId", gradeId);
      navigate("/student");
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
        <h2 className="">Vui lòng chọn đúng lớp bạn đang học</h2>
        {loading ? (
          <p>Đang tải...</p>
        ) : (
          <div className="choose-role-btn">
            {grades && Array.isArray(grades) && grades.map((grade) => (
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
