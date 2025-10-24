import { NavLink, useNavigate } from "react-router-dom";
import Logo from "./Logo";
import SearchBar from "./SearchBar";
import { useAuth } from "../contexts/AuthContext";
import { User, LogOut, Settings } from "lucide-react";
import { useState, useRef, useEffect } from "react";

function PageNav() {
  const navigate = useNavigate();
  const { isAuthenticated, user, logout } = useAuth();
  const [showDropdown, setShowDropdown] = useState(false);
  const dropdownRef = useRef(null);

  const handleLoginClick = () => {
    navigate('/login');
  };

  const handleSignUpClick = () => {
    navigate('/signup');
  };

  const handleLogout = () => {
    logout();
    setShowDropdown(false);
    navigate('/');
  };

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setShowDropdown(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  return (
    <nav className="flex bg-white h-18 items-center justify-around" style={{fontFamily: 'Space Grotesk, sans-serif'}}>
      <Logo />
      <SearchBar button={"left-41"}/>

      <ul className="flex w-md justify-between">
        <li>
          <NavLink to="/">About Us</NavLink>
        </li>

        <li>
          <NavLink to="/">Services</NavLink>
        </li>

        <li>
          <NavLink to="/">Contact</NavLink>
        </li>

        <li>
          <NavLink to="/">Help Center</NavLink>
        </li>
      </ul>
      
      {isAuthenticated ? (
        <div className="flex items-center space-x-3 relative" ref={dropdownRef}>
          <div 
            className="flex items-center space-x-2 cursor-pointer hover:opacity-80 transition-opacity"
            onClick={() => setShowDropdown(!showDropdown)}
          >
            <div className="w-10 h-10 bg-blue-400 rounded-full flex items-center justify-center">
              {user?.avatar ? (
                <img src={user.avatar} alt="Profile" className="w-full h-full rounded-full object-cover" />
              ) : (
                <User size={20} className="text-white" />
              )}
            </div>
            <span className="text-gray-700 font-medium text-sm">
              {user?.fullName || user?.email || 'User'}
            </span>
          </div>

          {/* Dropdown Menu */}
          {showDropdown && (
            <div className="absolute top-12 right-0 w-48 bg-white rounded-lg shadow-lg border border-gray-200 py-2 z-50">
              <button
                onClick={() => {
                  setShowDropdown(false);
                  // Set localStorage và reload trang
                  localStorage.setItem('organizer.active', 'profile');
                  localStorage.setItem('organizer.discoverySub', 'account');
                  // Nếu đã ở trang organizer, reload để update state
                  if (window.location.pathname === '/organizer') {
                    window.location.reload();
                  } else {
                    navigate('/organizer');
                  }
                }}
                className="w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-100 flex items-center space-x-2"
              >
                <Settings size={16} />
                <span>Profile & Settings</span>
              </button>
              <hr className="my-1 border-gray-200" />
              <button
                onClick={handleLogout}
                className="w-full px-4 py-2 text-left text-sm text-red-600 hover:bg-gray-100 flex items-center space-x-2"
              >
                <LogOut size={16} />
                <span>Logout</span>
              </button>
            </div>
          )}
        </div>
      ) : (
        <div className="flex space-x-1">
          <button onClick={handleLoginClick} className="bg-blue-400 text-white rounded-full px-3.5 py-0.5">
            Login
          </button>
          <button onClick={handleSignUpClick} className="bg-white text-blue-400 border-1 border-blue-400 rounded-full px-3.5 py-0.5">
            Sign Up
          </button>
        </div>
      )}
      
    </nav>
  );
}

export default PageNav;
