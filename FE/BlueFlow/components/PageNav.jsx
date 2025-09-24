import { NavLink } from "react-router-dom";
import Logo from "./Logo";
import SearchBar from "./searchBar";

function PageNav() {
  return (
    <nav className="flex bg-white h-18 items-center justify-around">
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
      <h6 className="w-28">Icon</h6>
    </nav>
  );
}

export default PageNav;
