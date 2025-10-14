
import { Link } from "react-router-dom";

export default function Navbar() {
  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark px-3">
      <Link className="navbar-brand fw-bold" to="/dashboard">
        🏋️ GymApp
      </Link>
      <div className="navbar-nav">
        <Link className="nav-link" to="/socios">
          Socios
        </Link>
        <Link className="nav-link" to="/socios/nuevo">
          Nuevo Socio
        </Link>
      </div>
    </nav>
  );
}
