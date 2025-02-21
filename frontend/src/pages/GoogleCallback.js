import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

const GoogleCallback = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const handleGoogleLogin = async () => {
      try {
        const response = await fetch("https://localhost:7259/api/Auth/google-callback", {
          method: "GET",
        });

        if (!response.ok) {
          throw new Error("Lỗi xác thực Google");
        }

        const data = await response.json();
        console.log("Google Login Response:", data);

        sessionStorage.setItem("token", data.token);
        sessionStorage.setItem("userId", data.userId);
        sessionStorage.setItem("role", data.role);

        switch (data.role) {
          case 1:
            navigate("/admin");
            break;
          case 2:
            navigate("/teacher-dashboard");
            break;
          case 3:
            navigate("/parent-dashboard");
            break;
          case 4:
            navigate("/student-dashboard");
            break;
          default:
            navigate("/"); 
        }
      } catch (error) {
        console.error("Lỗi khi xử lý Google Login:", error);
        navigate("/login"); 
      }
    };

    handleGoogleLogin();
  }, [navigate]);

  return <div>Đang xử lý đăng nhập Google...</div>;
};

export default GoogleCallback;
