
import React from "react";
import { NavLink } from "react-router-dom";
import { BiGrid , BiBookOpen, BiSolidDashboard, BiAlignRight, BiSolidHourglass , BiSolidUserDetail, BiPackage, BiInfoSquare } from "react-icons/bi";

const Sidebar = () => {
  return (
    <div className="sidebar d-none d-md-block">
      <div className="mb-4">
        <h5 className="d-flex align-items-center">
          <BiBookOpen className="me-2" />
          Learn & Earn
        </h5>
      </div>
      <nav className="nav flex-column">
        <NavLink to="/" className="nav-link" activeClassName="active">
          <BiSolidDashboard  className="me-2" /> Dashboard
        </NavLink>
        <NavLink to="/admin/listchapter" className="nav-link" activeClassName="active">
          <BiAlignRight className="me-2" /> Chương trình
        </NavLink>
        <NavLink to="/competitions" className="nav-link" activeClassName="active">
          <BiSolidHourglass className="me-2" /> Cuộc thi
        </NavLink>
        <NavLink to="/users" className="nav-link" activeClassName="active">
          <BiSolidUserDetail className="me-2" /> Người dùng
        </NavLink>
        <NavLink to="/profile" className="nav-link" activeClassName="active">
          <BiPackage  className="me-2" /> Gói
        </NavLink>
        <NavLink to="/profile" className="nav-link" activeClassName="active">
          <BiInfoSquare className="me-2" /> Thông tin cá nhân
        </NavLink>
      </nav>
    </div>
  );
};

export default Sidebar;
