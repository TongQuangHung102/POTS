import React from "react";
import Sidebar from "../components/Sidebar";
import { Outlet } from "react-router-dom"; 
import './AdminLayout.css';
const AdminLayout = () => {
  return (
    <div>
      <Sidebar />
      <div>
        <Outlet /> {/* Đây là nơi sẽ render nội dung trang con */}
      </div>
    </div>
  );
};

export default AdminLayout;
