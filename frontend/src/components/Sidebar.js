import React from "react";
import { NavLink } from "react-router-dom";
import { BiBookOpen, BiSolidDashboard, BiAlignRight, BiSolidHourglass, BiSolidUserDetail, BiPackage, BiInfoSquare } from "react-icons/bi";
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
      { path: "/admin/chapter", icon: <BiAlignRight />, label: "Chương trình" },
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
      { path: "/student/course", icon: <BiAlignRight />, label: "Khóa học" },
      { path: "/competitions", icon: <BiSolidHourglass />, label: "Cuộc thi" },
      { path: "/profile", icon: <BiInfoSquare />, label: "Thông tin cá nhân" },
    ],
    content_manager: [
      { path: "/student/dashboard", icon: <BiSolidDashboard />, label: "Dashboard" },
      { path: "/student/course", icon: <BiAlignRight />, label: "Khóa học" },
      { path: "/competitions", icon: <BiSolidHourglass />, label: "Cuộc thi" },
      { path: "/profile", icon: <BiInfoSquare />, label: "Thông tin cá nhân" },
    ],
    guest: [
      { path: "/student/dashboard", icon: <BiSolidDashboard />, label: "Dashboard" },
      { path: "/contentmanage/question", icon: <BiAlignRight />, label: "Câu hỏi" },
    ], 
  };

  const menuItems = menuConfig[role];

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
          <NavLink key={index} to={item.path} className="nav-link">
            {item.icon} {item.label}
          </NavLink>
        ))}
      </nav>
    </div>
  );
};

export default Sidebar;
