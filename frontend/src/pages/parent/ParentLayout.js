import React from "react";
import Sidebar from "../../components/Sidebar";
import { Outlet } from "react-router-dom"; 
import '../../pages/Layout.css';
const ParentLayout = () => {
  return (
    <div>
      <Sidebar />
      <div>
        <Outlet />
      </div>
    </div>
  );
};

export default ParentLayout;