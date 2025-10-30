import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";

interface Usuario {
  id: number;
  alias?: string;
  email: string;
  avatar?: { url?: string } | string;
  rol?: string;
}

export default function Navbar({
  onToggleSidebar,
}: {
  onToggleSidebar?: () => void;
}) {
  const navigate = useNavigate();
  const [usuario, setUsuario] = useState<Usuario | null>(null);

  const BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5144";

  // Cargar usuario al montar y escuchar cambios globales
  useEffect(() => {
    const loadUser = () => {
      const stored = localStorage.getItem("usuario");
      setUsuario(stored ? JSON.parse(stored) : null);
    };

    loadUser();
    window.addEventListener("authChange", loadUser);
    return () => window.removeEventListener("authChange", loadUser);
  }, []);

  //  Cerrar sesión
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
      localStorage.removeItem("token");
      localStorage.removeItem("usuario");
      setUsuario(null);
      navigate("/login");
      Swal.fire("Sesión cerrada", "Hasta pronto 👋", "success");
    }
  };

  // Determinar URL del avatar (acepta string o { url })
  const avatarUrlRaw =
    typeof usuario?.avatar === "string"
      ? usuario.avatar
      : usuario?.avatar?.url;

  const avatarUrl =
    avatarUrlRaw && avatarUrlRaw.trim() !== ""
      ? `${BASE_URL.replace(/\/+$/, "")}/${avatarUrlRaw
          .replace(/^\/+/, "")
          .toLowerCase()}?v=${Date.now()}&id=${usuario?.id || ""}`
      : `${BASE_URL.replace(/\/+$/, "")}/images/user.png`;

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

          <div className="d-flex align-items-center">
            <img
              src="/logo.png"
              alt="Logo FitGym"
              style={{
                width: 42,
                height: 42,
                objectFit: "contain",
                marginRight: "10px",
              }}
            />
            <h3 className="text-white fw-bold m-0">FitGym</h3>
          </div>
        </div>

        {/*  Lado derecho */}
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
                  key={avatarUrl}
                  src={avatarUrl}
                  alt="Avatar"
                  className="rounded-circle me-2"
                  style={{
                    width: 36,
                    height: 36,
                    objectFit: "cover",
                    border: "2px solid white",
                  }}
                  onError={(e) => {
                    const img = e.currentTarget;
                    if (!img.dataset.fallback) {
                      console.warn("⚠️ Avatar no encontrado, usando fallback:", img.src);
                      img.src = `${BASE_URL}/images/user.png`;
                      img.dataset.fallback = "true";
                    }
                  }}
                />
                <span>{usuario.alias || usuario.email}</span>
              </button>

              <ul className="dropdown-menu dropdown-menu-end shadow">
                <li>
                  <Link
                    to={
                      usuario?.rol?.toLowerCase() === "socio"
                        ? "/perfil-socio"
                        : "/perfil"
                    }
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
