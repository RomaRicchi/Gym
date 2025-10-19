import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";

interface Usuario {
  id: number;
  alias?: string;
  email: string;
  avatar?: { url?: string };
}

export default function Navbar({ onToggleSidebar }: { onToggleSidebar?: () => void }) {
  const navigate = useNavigate();
  const [usuario, setUsuario] = useState<Usuario | null>(null);

  // 🧩 URL base del backend (usa variable VITE_API_URL si está definida)
  const BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5144";

  // 🧩 Cargar usuario desde localStorage al montar
  useEffect(() => {
    const stored = localStorage.getItem("usuario");
    if (stored) setUsuario(JSON.parse(stored));
  }, []);

  const handleLogout = async () => {
    const result = await Swal.fire({
      title: "¿Cerrar sesión?",
      text: "Se cerrará tu sesión actual.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, salir",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (result.isConfirmed) {
      localStorage.removeItem("authToken");
      localStorage.removeItem("usuario");
      setUsuario(null);
      navigate("/login");
      Swal.fire("Sesión cerrada", "Hasta pronto 👋", "success");
    }
  };

  return (
    <nav className="navbar px-4 shadow-sm" style={{ backgroundColor: "#ff6b00" }}>
      <div className="d-flex align-items-center w-100 justify-content-between">
        {/* 🔹 Lado izquierdo */}
        <div className="d-flex align-items-center gap-3">
          <button
            className="btn btn-outline-light me-2"
            onClick={onToggleSidebar}
          >
            <i className="fa fa-bars"></i>
          </button>

          <h3 className="text-white fw-bold m-0">FitGym</h3>
        </div>

        {/* 🔹 Lado derecho */}
        <div className="d-flex align-items-center">
          {!usuario ? (
            <Link
              to="/login"
              className="btn btn-outline-light fw-semibold"
              style={{ borderColor: "white" }}
            >
              Iniciar sesión
            </Link>
          ) : (
            <div className="dropdown">
              <button
                className="btn btn-outline-light dropdown-toggle d-flex align-items-center"
                data-bs-toggle="dropdown"
                aria-expanded="false"
                style={{ borderColor: "white" }}
              >
                <img
                  src={
                    usuario.avatar?.url
                      ? `${BASE_URL}${usuario.avatar.url}`
                      : `${BASE_URL}/images/user.png`
                  }
                  alt="Avatar"
                  className="rounded-circle me-2"
                  style={{
                    width: 36,
                    height: 36,
                    objectFit: "cover",
                    border: "2px solid white",
                  }}
                />
                <span>{usuario.alias || usuario.email}</span>
              </button>

              <ul className="dropdown-menu dropdown-menu-end shadow">
                <li>
                  <Link
                    to="/perfil"
                    className="dropdown-item d-flex align-items-center gap-2"
                    style={{ textDecoration: "none", color: "inherit" }}
                  >
                    <i className="fa fa-user"></i> Perfil
                  </Link>
                </li>
                <li>
                  <hr className="dropdown-divider" />
                </li>
                <li>
                  <button
                    className="dropdown-item text-danger d-flex align-items-center gap-2"
                    onClick={handleLogout}
                  >
                    <i className="fa fa-door-open"></i> Cerrar sesión
                  </button>
                </li>
              </ul>
            </div>
          )}
        </div>
      </div>
    </nav>
  );
}
