import React from "react";
import { Link } from "react-router-dom";
import { BiGrid } from "react-icons/bi";

const Sidebar = () => {
  return (
    <div className="sidebar d-none d-md-block">
      <div className="mb-4">
        <h5 className="d-flex align-items-center">
          <BiGrid className="me-2" />
          Learn & Earn
        </h5>
      </div>
      <nav className="nav flex-column">
        <Link to="/" className="nav-link active">
          <BiGrid className="me-2" /> Dashboard
        </Link>
        <Link to="/admin/listchapter" className="nav-link">
          <BiGrid className="me-2" /> Chương trình
        </Link>
        <Link to="/competitions" className="nav-link">
          <BiGrid className="me-2" /> Cuộc thi
        </Link>
        <Link to="/users" className="nav-link">
          <BiGrid className="me-2" /> Người dùng
        </Link>
        <Link to="/profile" className="nav-link">
          <BiGrid className="me-2" /> Thông tin cá nhân
        </Link>
      </nav>
    </div>
  );
};

export default Sidebar;
