import React from "react";
import Sidebar from "../components/Sidebar";
import { Outlet } from "react-router-dom"; 
import './Layout.css';
const StudentLayout = () => {
  return (
    <div>
      <Sidebar />
      <div>
        <Outlet /> {/* Đây là nơi sẽ render nội dung trang con */}
      </div>
    </div>
  );
};

export default StudentLayout;