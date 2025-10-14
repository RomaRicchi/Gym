import { Link } from "react-router-dom";

export default function Navbar({ onToggleSidebar }: { onToggleSidebar?: () => void }) {
  return (
    <nav
      className="navbar px-4 shadow-sm"
      style={{ backgroundColor: "#ff6b00" }}
    >
      <div className="d-flex align-items-center w-100 justify-content-between">
        <div className="d-flex align-items-center gap-3">
          {/* 🔹 Botón hamburguesa (solo móvil) */}
          <button
            className="btn btn-outline-light me-2"
            onClick={onToggleSidebar}
          >
            <i className="fa fa-bars"></i>
          </button>

          <h3 className="text-white fw-bold m-0">🏋️ FitGym</h3>
        </div>

        <div>
          <Link
            to="/login"
            className="btn btn-outline-light fw-semibold"
            style={{ borderColor: "white" }}
          >
            Iniciar sesión
          </Link>
        </div>
      </div>
    </nav>
  );
}
