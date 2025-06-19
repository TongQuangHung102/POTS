import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { authFetch } from "../utils/authFetch";
import "./ChooseRole.css";

const ChooseRole = () => {
  const { user, setUser } = useAuth(); 
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const apiUrl = process.env.REACT_APP_API_URL;
  const roleRedirects = {
    1: "/student/dashboard",  // Học sinh
    2: "/parent/dashboard",   // Phụ huynh
  };

  const handleRoleSelect = async (roleId) => {
    if (!user?.userId) {
      alert("Không xác định được người dùng!");
      return;
    }

    setLoading(true); 

    try {
      const response = await authFetch(`${apiUrl}/api/User/update-role/${user.userId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(roleId), 
      });
      
      const data = await response.json();
      if (!response.ok) {
        throw new Error(data?.message || "Cập nhật role thất bại");
      }

      alert("Cập nhật vai trò và thành công, vui lòng đăng nhập lại")
      localStorage.clear();
      navigate('/login', { replace: true });
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
        <h2 className="">Chọn vai trò của bạn</h2>
        <div className="choose-role-btn">
          <button
            onClick={() => handleRoleSelect(1)}
            disabled={loading}
            className=""
          >
            Học sinh
          </button>
          <button
            onClick={() => handleRoleSelect(2)}
            disabled={loading}
            className=""
          >
            Phụ huynh
          </button>
        </div>
      </div>
    </div>
  );
};

export default ChooseRole;
