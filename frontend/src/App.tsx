import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
  useLocation,
} from "react-router-dom";
import { useEffect } from "react";
import Layout from "@/components/Layout";

import "bootstrap/dist/css/bootstrap.min.css";
import "@/styles/main.css";

/* === Vistas administrativas === */
import Dashboard from "@/views/Dashboard";
import Login from "@/views/usuarios/Login";
import PerfilView from "@/views/usuarios/perfil/PerfilView";
import SociosList from "@/views/socios/List";
import PersonalList from "@/views/personal/List";
import SuscripcionesList from "@/views/suscripciones/List";
import PlanesList from "@/views/suscripciones/planes/List";
import SalasList from "@/views/salas/List";
import TurnosPlantillaList from "@/views/agenda/turnoPlantilla/List";
import TurnosList from "@/views/agenda/suscripcionTurno/TurnosList";
import AgendaCalendar from "@/views/agenda/AgendaCalendar";
import OrdenesList from "@/views/gestionPagos/List";
import ComprobantesList from "@/views/gestionPagos/comprobantes/List";
import ComprobanteUpload from "@/views/gestionPagos/comprobantes/Upload";
import EstadosList from "@/views/gestionPagos/estado/List";
import RolesList from "@/views/usuarios/rol/List";
import UsuariosList from "@/views/usuarios/List";
import ResetPassword from "@/views/usuarios/ResetPassword";

/* === Vistas del SOCIO === */
import DashboardSocio from "@/views/DashboardSocio";
import PlanesSocio from "@/views/socios/PlanesSocio";
import SuscripcionesSocio from "@/views/socios/SuscripcionesSocio";
import TurnosSocio from "@/views/socios/TurnosSocio";
import RutinasSocio from "@/views/socios/RutinasSocio";

/* Scroll automático */
function ScrollToTop() {
  const { pathname } = useLocation();
  useEffect(() => {
    window.scrollTo(0, 0);
  }, [pathname]);
  return null;
}

export default function App() {
  return (
    <Router>
      <ScrollToTop />
      <Routes>
        {/* 🔒 Panel Administrativo */}
        <Route element={<Layout />}>
          <Route path="/" element={<Navigate to="/dashboard" />} />
          <Route path="/dashboard" element={<Dashboard />} />

          {/* Perfil administrativo */}
          <Route path="/perfil" element={<PerfilView />} />

          {/* Gestión */}
          <Route path="/socios" element={<SociosList />} />
          <Route path="/personal" element={<PersonalList />} />
          <Route path="/suscripciones" element={<SuscripcionesList />} />
          <Route path="/planes" element={<PlanesList />} />
          <Route path="/salas" element={<SalasList />} />
          <Route path="/agenda/calendario" element={<AgendaCalendar />} />
          <Route path="/turnos" element={<TurnosPlantillaList />} />
          <Route path="/suscripciones/turnos" element={<TurnosList />} />
          <Route path="/ordenes" element={<OrdenesList />} />
          <Route path="/comprobantes" element={<ComprobantesList />} />
          <Route path="/ordenes/:id/comprobantes" element={<ComprobantesList />} />
          <Route path="/ordenes/:id/subir-comprobante" element={<ComprobanteUpload />} />
          <Route path="/estados" element={<EstadosList />} />
          <Route path="/roles" element={<RolesList />} />
          <Route path="/usuarios" element={<UsuariosList />} />
       

          {/* 🧡 Panel del Socio */}
          <Route path="/dashboardSocio" element={<DashboardSocio />} />
          <Route path="/socio/planesSocio" element={<PlanesSocio />} />
          <Route path="/socio/suscripcionesSocio" element={<SuscripcionesSocio />} />
          <Route path="/socio/turnosSocio" element={<TurnosSocio />} />
          <Route path="/socio/rutinasSocio" element={<RutinasSocio />} />
        </Route>
        {/* 🌐 Rutas públicas */}
        <Route path="/login" element={<Login />} />
        <Route path="/reset-password" element={<ResetPassword />} />

        {/* Redirección general */}
        <Route path="*" element={<Navigate to="/dashboard" />} />
      </Routes>
    </Router>
  );
}
