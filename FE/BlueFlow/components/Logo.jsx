import { Link } from "react-router-dom";
import styles from "./Logo.module.css";

function Logo() {
  return (
    <Link to="/">
      <img src="/imgs/logo.png" alt="BlueFlow Logo" className={styles.logo} />
    </Link>
  );
}

export default Logo;
