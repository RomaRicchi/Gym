import { useEffect, useState } from "react";
import { Link, useLocation, NavLink } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faHouse,
  faUsers,
  faCreditCard,
  faMoneyBill,
  faDumbbell,
  faCalendarDays,
  faUserTie,
  faFileInvoice,
  faGears,
  faPuzzlePiece,
  faUser,
  faListCheck,      
  faCalendarCheck,  
} from "@fortawesome/free-solid-svg-icons";
import "@/styles/Sidebar.css";

export default function Sidebar() {
    const [usuario, setUsuario] = useState<{ rol?: string }>({});
    const location = useLocation();

    useEffect(() => {
      const stored = localStorage.getItem("usuario");
      if (stored) setUsuario(JSON.parse(stored));
    }, []);

    const rol = usuario?.rol || "Invitado";

  return (
    <div className="sidebar">
      <ul className="nav flex-column">

        {/* 🏠 Dashboard */}
        <li className="nav-item">
          <NavLink
            to="/dashboard"
            className={({ isActive }) =>
              `nav-link ${isActive ? "active-link" : ""}`
            }
          >
            <FontAwesomeIcon icon={faHouse} className="me-2" />
            Dashboard
          </NavLink>
        </li>

        {/* Gestión */}
        <li className="nav-section mt-3">
          <span className="text-uppercase small text-muted">Gestión</span>
        </li>

        <li className="nav-item">
          <NavLink
            to="/socios"
            className={({ isActive }) =>
              `nav-link ${isActive ? "active-link" : ""}`
            }
          >
            <FontAwesomeIcon icon={faUsers} className="me-2" />
            Socios
          </NavLink>
        </li>

        <li className="nav-item">
          <NavLink
            to="/suscripciones"
            className={({ isActive }) =>
              `nav-link ${isActive ? "active-link" : ""}`
            }
          >
            <FontAwesomeIcon icon={faCreditCard} className="me-2" />
            Suscripciones
          </NavLink>
        </li>

        <li className="nav-item">
          <NavLink
            to="/planes"
            className={({ isActive }) =>
              `nav-link ${isActive ? "active-link" : ""}`
            }
          >
            <FontAwesomeIcon icon={faMoneyBill} className="me-2" />
            Planes
          </NavLink>
        </li>

        <hr className="sidebar-divider" />

        {/* 🗓️ Agenda */}
        <li className="nav-section">
          <span className="text-uppercase small text-muted">Agenda</span>
        </li>

        <li className="nav-item">
          <NavLink
            to="/agenda/calendario"
            className={({ isActive }) =>
              `nav-link ${isActive ? "active-link" : ""}`
            }
          >
            <FontAwesomeIcon icon={faCalendarDays} className="me-2" />
            Calendario
          </NavLink>
        </li>

        <li className="nav-item">
          <NavLink
            to="/turnos"
            className={({ isActive }) =>
              `nav-link ${isActive ? "active-link" : ""}`
            }
          >
            <FontAwesomeIcon icon={faListCheck} className="me-2" />
            Turnos Plantilla
          </NavLink>
        </li>

        <li className="nav-item">
          <NavLink
            to="/suscripciones/turnos"
            className={({ isActive }) =>
              `nav-link ${isActive ? "active-link" : ""}`
            }
          >
            <FontAwesomeIcon icon={faCalendarCheck} className="me-2" />
            Turnos por Socio
          </NavLink>
        </li>

        <hr className="sidebar-divider" />

        {/* Instalaciones */}
        {rol === "Administrador" && (
        <>
          <li className="nav-section">
            <span className="text-uppercase small text-muted">Instalaciones</span>
          </li>

          <li className="nav-item">
            <NavLink
              to="/salas"
              className={({ isActive }) =>
                `nav-link ${isActive ? "active-link" : ""}`
              }
            >
              <FontAwesomeIcon icon={faDumbbell} className="me-2" />
              Salas
            </NavLink>
          </li>

          <li className="nav-item">
            <NavLink
              to="/personal"
              className={({ isActive }) =>
                `nav-link ${isActive ? "active-link" : ""}`
              }
            >
              <FontAwesomeIcon icon={faUserTie} className="me-2" />
              Personal
            </NavLink>
          </li>
       

        <hr className="sidebar-divider" />
        </>
        )}
        {/* Pagos */}
        <li className="nav-section">
          <span className="text-uppercase small text-muted">Pagos</span>
        </li>

        <li className="nav-item">
          <NavLink
            to="/ordenes"
            className={({ isActive }) =>
              `nav-link ${isActive ? "active-link" : ""}`
            }
          >
            <FontAwesomeIcon icon={faFileInvoice} className="me-2" />
            Órdenes
          </NavLink>
        </li>
        {rol === "Administrador" && (
        <>
        <li className="nav-item">
          <NavLink
            to="/estados"
            className={({ isActive }) =>
              `nav-link ${isActive ? "active-link" : ""}`
            }
          >
            <FontAwesomeIcon icon={faGears} className="me-2" />
            Estados
          </NavLink>
        </li>
        </>
        )}
        <hr className="sidebar-divider" />

        {/* Usuarios */}
        {rol === "Administrador" && (
        <>
        <li className="nav-section">
          <span className="text-uppercase small text-muted">Usuarios</span>
        </li>

        <li className="nav-item">
          <NavLink
            to="/roles"
            className={({ isActive }) =>
              `nav-link ${isActive ? "active-link" : ""}`
            }
          >
            <FontAwesomeIcon icon={faPuzzlePiece} className="me-2" />
            Roles
          </NavLink>
        </li>

        <li className="nav-item">
          <NavLink
            to="/usuarios"
            className={({ isActive }) =>
              `nav-link ${isActive ? "active-link" : ""}`
            }
          >
            <FontAwesomeIcon icon={faUser} className="me-2" />
            Usuarios
          </NavLink>
        </li>
        </>
      )}
      </ul>
    </div>
  );
}
