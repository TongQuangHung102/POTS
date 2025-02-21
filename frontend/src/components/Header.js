import { BiSolidUserCircle } from "react-icons/bi";  
import { BiLogOut } from "react-icons/bi";
import { useNavigate } from "react-router-dom"; 
import { useAuth } from "../hooks/useAuth"; 

import './Header.css';

const Header = () => {
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
      <div className="header-container">
        <div className="header-card">
          <div className="menu-icon">
            <BiSolidUserCircle size={24} />
          </div>
          <div className="search-container">
            <button onClick={handleLogout} type="button" className="btn btn-default btn-sm">
              <BiLogOut size={18} /> Log out
            </button>
          </div>
        </div>
      </div>
  );
};

export default Header;
