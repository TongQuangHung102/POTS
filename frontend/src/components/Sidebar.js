import React from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { BiBookOpen, BiSolidDashboard, BiAlignRight, BiSolidHourglass, BiSolidUserDetail, BiPackage, BiInfoSquare, BiLogOut } from "react-icons/bi";
import { useAuth } from "../hooks/useAuth";

const Sidebar = () => {
  const { user } = useAuth();
  const roleId = user?.roleId;

  const roleMap = {
    1: "student",
    2: "parent",
    3: "admin",
    4: "content_manager"
  };

  const role = roleMap[roleId] || "guest";

  // Danh sách menu theo từng role
  const menuConfig = {
    admin: [
      { path: "/admin/dashboard", icon: <BiSolidDashboard />, label: "Dashboard" },
      { path: "/admin/grades", icon: <BiAlignRight />, label: "Chương trình" },
      { path: "/competitions", icon: <BiSolidHourglass />, label: "Cuộc thi" },
      { path: "/admin/users", icon: <BiSolidUserDetail />, label: "Người dùng" },
      { path: "/admin/package", icon: <BiPackage />, label: "Gói" },
      { path: "/profile", icon: <BiInfoSquare />, label: "Thông tin cá nhân" },
    ],
    Parent: [
      { path: "/", icon: <BiSolidDashboard />, label: "Dashboard" },
      { path: "/teacher/classes", icon: <BiAlignRight />, label: "Lớp học" },
      { path: "/competitions", icon: <BiSolidHourglass />, label: "Cuộc thi" },
      { path: "/profile", icon: <BiInfoSquare />, label: "Thông tin cá nhân" },
    ],
    student: [
      { path: "/student/dashboard", icon: <BiSolidDashboard />, label: "Dashboard" },
      { path: "/student/course", icon: <BiAlignRight />, label: "Luyện tập" },
      { path: "/student/package", icon: <BiPackage />, label: "Gói" },
      { path: "/competitions", icon: <BiSolidHourglass />, label: "Cuộc thi" },
      { path: "/profile", icon: <BiInfoSquare />, label: "Thông tin cá nhân" },
    ],
    content_manager: [
      { path: "/student/dashboard", icon: <BiSolidDashboard />, label: "Dashboard" },
      { path: "/content_manage/grades", icon: <BiAlignRight />, label: "Quản lý nội dung" },
      { path: "/competitions", icon: <BiSolidHourglass />, label: "Cuộc thi" },
      { path: "/profile", icon: <BiInfoSquare />, label: "Thông tin cá nhân" },
    ],
    guest: [
      { path: "/student/dashboard", icon: <BiSolidDashboard />, label: "Dashboard" },
      { path: "/contentmanage/question", icon: <BiAlignRight />, label: "Câu hỏi" },
    ],
  };

  const menuItems = menuConfig[role];

  const navigate = useNavigate();
  const { setUser } = useAuth();

  const handleLogout = () => {
    sessionStorage.removeItem("token");
    sessionStorage.removeItem("userId");
    sessionStorage.removeItem("roleId");
    setUser(null);

    navigate("/login");
  };

  return (
    <div className="sidebar d-none d-md-block">
      <div className="mb-4">
        <h5 className="d-flex align-items-center">
          <BiBookOpen className="me-2" />
          Learn & Earn
        </h5>
      </div>
      <nav className="nav flex-column">
        {menuItems.map((item, index) => (
          <NavLink key={index} to={item.path} className="nav-link" end={false}>
            {item.icon} {item.label}
          </NavLink>
        ))}
      </nav>
      <div className="mb-4 border-top pt-3 text-danger">
        <button onClick={handleLogout} type="button" className="btn btn-default btn-sm">
          <BiLogOut size={18} /> Log out
        </button>
      </div>
    </div>
  );
};

export default Sidebar;
