import { useEffect, useState } from "react";
import { NavLink } from "react-router-dom";
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
  faWeightScale,
  faChevronDown,
  faChevronUp,
  faClipboardList,
  faFileAlt,
  faCheckSquare,
  faChartLine,
  faClipboardUser,
} from "@fortawesome/free-solid-svg-icons";
import "@/styles/Sidebar.css";

export default function Sidebar() {
  const [usuario, setUsuario] = useState<{ rol?: string }>({});
  const rol = usuario?.rol || "Invitado";

  const [open, setOpen] = useState({
    gestion: true,
    rutinas: false,
    agenda: false,
    instalaciones: false,
    pagos: false,
    usuarios: false,
  });

  useEffect(() => {
    const stored = localStorage.getItem("usuario");
    if (stored) setUsuario(JSON.parse(stored));
  }, []);

  const toggle = (section: keyof typeof open) =>
    setOpen({ ...open, [section]: !open[section] });

  return (
    <div className="sidebar" style={{ overflowY: "auto", height: "100vh" }}>
      <ul className="nav flex-column">

        {/* === 🧡 PANEL SOCIO === */}
        {rol === "Socio" && (
          <>
            <li className="nav-item mt-3">
              <NavLink to="/dashboardSocio" className="nav-link fw-bold">
                <FontAwesomeIcon icon={faHouse} className="me-2" />
                Mi Panel
              </NavLink>
            </li>

            <li className="nav-item">
              <NavLink to="/socio/turnos" className="nav-link">
                <FontAwesomeIcon icon={faCalendarDays} className="me-2" />
                Calendario
              </NavLink>
            </li>

            <li className="nav-item">
              <NavLink to="/evolucionfisica" className="nav-link">
                <FontAwesomeIcon icon={faWeightScale} className="me-2" />
                Evolución Física
              </NavLink>
            </li>

            <hr className="sidebar-divider" />
          </>
        )}

        {/* === 🧑‍💼 PANEL ADMIN / STAFF === */}
        {rol !== "Socio" && (
          <>
            <li className="nav-item">
              <NavLink to="/dashboard" className="nav-link fw-bold">
                <FontAwesomeIcon icon={faHouse} className="me-2" />
                Dashboard
              </NavLink>
            </li>

            {/* === Gestión === */}
            <hr className="sidebar-divider" />
            <li className="nav-section" onClick={() => toggle("gestion")}>
              <span className="sidebar-title">
                GESTIÓN{" "}
                <FontAwesomeIcon
                  icon={open.gestion ? faChevronUp : faChevronDown}
                  className="ms-1"
                />
              </span>
            </li>
            {open.gestion && (
              <>
                <li className="nav-item">
                  <NavLink to="/socios" className="nav-link">
                    <FontAwesomeIcon icon={faUsers} className="me-2" /> Socios
                  </NavLink>
                </li>
                <li className="nav-item">
                  <NavLink to="/suscripciones" className="nav-link">
                    <FontAwesomeIcon icon={faCreditCard} className="me-2" /> Suscripciones
                  </NavLink>
                </li>
                <li className="nav-item">
                  <NavLink to="/planes" className="nav-link">
                    <FontAwesomeIcon icon={faMoneyBill} className="me-2" /> Planes
                  </NavLink>
                </li>
              </>
            )}

            {/* === Rutinas === */}
            <hr className="sidebar-divider" />
            <li className="nav-section" onClick={() => toggle("rutinas")}>
              <span className="sidebar-title">
                RUTINAS{" "}
                <FontAwesomeIcon
                  icon={open.rutinas ? faChevronUp : faChevronDown}
                  className="ms-1"
                />
              </span>
            </li>
            {open.rutinas && (
              <>
                <li className="nav-item">
                  <NavLink to="/rutinas/ejercicios" className="nav-link">
                    <FontAwesomeIcon icon={faDumbbell} className="me-2" /> Ejercicios
                  </NavLink>
                </li>
                <li className="nav-item">
                  <NavLink to="/rutinas/plantillas" className="nav-link">
                    <FontAwesomeIcon icon={faClipboardList} className="me-2" /> Rutinas
                  </NavLink>
                </li>
                <li>
                  <NavLink to="/rutinas/cards" className="nav-link">
                    <FontAwesomeIcon icon={faClipboardList} className="me-2" /> Rutinas (Cards)
                  </NavLink>
                </li>

                <li className="nav-item">
                  <NavLink to="/rutinas/plantilla-ejercicios" className="nav-link">
                    <FontAwesomeIcon icon={faFileAlt} className="me-2" /> Planilla de Ejercicios
                  </NavLink>
                </li>
                <li className="nav-item">
                  <NavLink to="/rutinas/grupoMuscular" className="nav-link">
                    <FontAwesomeIcon icon={faDumbbell} className="me-2" /> Grupos Musculares
                  </NavLink>
                </li>
              </>
            )}

            {/* === Agenda === */}
            <hr className="sidebar-divider" />
            <li className="nav-section" onClick={() => toggle("agenda")}>
              <span className="sidebar-title">
                AGENDA{" "}
                <FontAwesomeIcon
                  icon={open.agenda ? faChevronUp : faChevronDown}
                  className="ms-1"
                />
              </span>
            </li>
            {open.agenda && (
              <>
                <li className="nav-item">
                  <NavLink to="/agenda/calendario" className="nav-link">
                    <FontAwesomeIcon icon={faCalendarDays} className="me-2" /> Calendario
                  </NavLink>
                </li>
                <li className="nav-item">
                  <NavLink to="/turnos" className="nav-link">
                    <FontAwesomeIcon icon={faListCheck} className="me-2" /> Turnos Plantilla
                  </NavLink>
                </li>
                <li className="nav-item">
                  <NavLink to="/suscripciones/turnos" className="nav-link">
                    <FontAwesomeIcon icon={faCalendarCheck} className="me-2" /> Turnos por Socio
                  </NavLink>
                </li>
              </>
            )}

            {/* === Instalaciones === */}
            {rol === "Administrador" && (
              <>
                <hr className="sidebar-divider" />
                <li className="nav-section" onClick={() => toggle("instalaciones")}>
                  <span className="sidebar-title">
                    INSTALACIONES{" "}
                    <FontAwesomeIcon
                      icon={open.instalaciones ? faChevronUp : faChevronDown}
                      className="ms-1"
                    />
                  </span>
                </li>
                {open.instalaciones && (
                  <>
                    <li className="nav-item">
                      <NavLink to="/salas" className="nav-link">
                        <FontAwesomeIcon icon={faDumbbell} className="me-2" /> Salas
                      </NavLink>
                    </li>
                    <li className="nav-item">
                      <NavLink to="/personal" className="nav-link">
                        <FontAwesomeIcon icon={faUserTie} className="me-2" /> Personal
                      </NavLink>
                    </li>
                  </>
                )}
              </>
            )}

            {/* === Pagos === */}
            <hr className="sidebar-divider" />
            <li className="nav-section" onClick={() => toggle("pagos")}>
              <span className="sidebar-title">
                PAGOS{" "}
                <FontAwesomeIcon
                  icon={open.pagos ? faChevronUp : faChevronDown}
                  className="ms-1"
                />
              </span>
            </li>
            {open.pagos && (
              <>
                <li className="nav-item">
                  <NavLink to="/ordenes" className="nav-link">
                    <FontAwesomeIcon icon={faFileInvoice} className="me-2" /> Órdenes
                  </NavLink>
                </li>
                {rol === "Administrador" && (
                  <>
                    <li className="nav-item">
                      <NavLink to="/estados" className="nav-link">
                        <FontAwesomeIcon icon={faGears} className="me-2" /> Estados
                      </NavLink>
                    </li>
                    <li className="nav-item">
                      <NavLink to="/pagos/ingresos" className="nav-link">
                        <FontAwesomeIcon icon={faMoneyBill} className="me-2" /> Ingresos
                      </NavLink>
                    </li>
                  </>
                )}
              </>
            )}

            {/* === Usuarios === */}
            {rol === "Administrador" && (
              <>
                <hr className="sidebar-divider" />
                <li className="nav-section" onClick={() => toggle("usuarios")}>
                  <span className="sidebar-title">
                    USUARIOS{" "}
                    <FontAwesomeIcon
                      icon={open.usuarios ? faChevronUp : faChevronDown}
                      className="ms-1"
                    />
                  </span>
                </li>
                {open.usuarios && (
                  <>
                    <li className="nav-item">
                      <NavLink to="/roles" className="nav-link">
                        <FontAwesomeIcon icon={faPuzzlePiece} className="me-2" /> Roles
                      </NavLink>
                    </li>
                    <li className="nav-item">
                      <NavLink to="/usuarios" className="nav-link">
                        <FontAwesomeIcon icon={faUser} className="me-2" /> Usuarios
                      </NavLink>
                    </li>
                  </>
                )}
              </>
            )}
          </>
        )}
      </ul>
    </div>
  );
}
