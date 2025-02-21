import React from "react";
import Sidebar from "../components/Sidebar";
import { Outlet } from "react-router-dom"; 
import './Layout.css';
const StudentLayout = () => {
  return (
    <div>
      <Sidebar />
      <div>
        <Outlet /> 
      </div>
    </div>
  );
};

export default StudentLayout;