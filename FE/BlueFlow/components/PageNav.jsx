import { NavLink, useNavigate } from "react-router-dom";
import Logo from "./Logo";
import SearchBar from "./searchBar";

function PageNav() {
  const navigate = useNavigate();

  const handleLoginClick = () => {
    navigate('/login');
  };

  const handleSignUpClick = () => {
    navigate('/signup');
  };

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
      <div className="flex space-x-1 ">
        <button onClick={handleLoginClick} className="bg-blue-400 text-white rounded-full px-3.5 py-0.5 ">
          Login
        </button>
        <button onClick={handleSignUpClick} className="bg-white text-blue-400 border-1 border-blue-400 rounded-full px-3.5 py-0.5 ">
          Sign Up
        </button>
      </div>
      
    </nav>
  );
}

export default PageNav;
