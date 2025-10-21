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

// 🧩 Vistas principales
import Dashboard from "@/views/Dashboard";
import Login from "@/views/usuarios/Login";
import PerfilView from "@/views/usuarios/perfil/PerfilView";

// 🧍 Socios
import SociosList from "@/views/socios/List";

// 👨‍🏫 Personal
import PersonalList from "@/views/personal/List";

// 💳 Suscripciones
import SuscripcionesList from "@/views/suscripciones/List";

// 💰 Planes
import PlanesList from "@/views/suscripciones/planes/List";

// 🏋️ Salas
import SalasList from "@/views/salas/List";

// 📆 Turnos plantilla
import TurnosPlantillaList from "@/views/agenda/turnoPlantilla/List";
import TurnoPlantillaCreate from "@/views/agenda/turnoPlantilla/Create";
import TurnoPlantillaEdit from "@/views/agenda/turnoPlantilla/Edit";
import TurnosAssign from "@/views/agenda/suscripcionTurno/TurnosAssign";
import TurnosList from "@/views/agenda/suscripcionTurno/TurnosList";
import AgendaCalendar from "@/views/agenda/AgendaCalendar";

// 🧾 Órdenes de pago
import OrdenesList from "@/views/gestionPagos/List";

// 📑 Comprobantes
import ComprobantesList from "@/views/gestionPagos/comprobantes/List";
import ComprobanteUpload from "@/views/gestionPagos/comprobantes/Upload";

// ⚙️ Estados de orden de pago
import EstadosList from "@/views/gestionPagos/estado/List";

// 🔗 Orden-Turno
import OrdenTurnosList from "@/views/agenda/ordenTurno/List";
import OrdenTurnoAssign from "@/views/agenda/ordenTurno/Assign";

// 🧩 Roles
import RolesList from "@/views/usuarios/rol/List";

// 👤 Usuarios
import UsuariosList from "@/views/usuarios/List";
import ResetPassword from "@/views/usuarios/ResetPassword";

/* 🔝 Scroll automático al cambiar de ruta */
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
        {/* 🔐 Rutas protegidas: solo accesibles si hay sesión */}
        
          <Route element={<Layout />}>
            {/* 🏠 Dashboard */}
            <Route path="/" element={<Navigate to="/dashboard" />} />
            <Route path="/dashboard" element={<Dashboard />} />

            {/* 👤 Perfil */}
            <Route path="/perfil" element={<PerfilView />} />

            {/* 🧍 Socios */}
            <Route path="/socios" element={<SociosList />} />

            {/* 👨‍🏫 Personal */}
            <Route path="/personal" element={<PersonalList />} />

            {/* 💳 Suscripciones */}
            <Route path="/suscripciones" element={<SuscripcionesList />} />

            {/* 💰 Planes */}
            <Route path="/planes" element={<PlanesList />} />

            {/* 🏋️ Salas */}
            <Route path="/salas" element={<SalasList />} />

            {/* 🗓️ Agenda y Turnos */}
            <Route path="/agenda/calendario" element={<AgendaCalendar />} />

            {/* Turnos plantilla */}
            <Route path="/turnos" element={<TurnosPlantillaList />} />
            <Route path="/turnos/nuevo" element={<TurnoPlantillaCreate />} />
            <Route path="/turnos/editar/:id" element={<TurnoPlantillaEdit />} />

            {/* Turnos por suscripción */}
            <Route path="/suscripciones/:id/asignar-turnos" element={<TurnosAssign />} />
            <Route path="/suscripciones/turnos" element={<TurnosList />} />

            {/* Turnos por orden */}
            <Route path="/ordenes/:id/turnos" element={<OrdenTurnosList />} />
            <Route path="/ordenes/:id/asignar-turnos" element={<OrdenTurnoAssign />} />


            {/* 🧾 Órdenes de Pago */}
            <Route path="/ordenes" element={<OrdenesList />} />

            {/* 📑 Comprobantes */}
            <Route path="/comprobantes" element={<ComprobantesList />} />
            <Route path="/ordenes/:id/comprobantes" element={<ComprobantesList />} />
            <Route path="/ordenes/:id/subir-comprobante" element={<ComprobanteUpload />} />

            {/* ⚙️ Estados */}
            <Route path="/estados" element={<EstadosList />} />

            {/* 🔗 Orden - Turnos */}
            <Route path="/ordenes/:id/turnos" element={<OrdenTurnosList />} />
            <Route path="/ordenes/:id/asignar-turnos" element={<OrdenTurnoAssign />} />

            {/* 🧩 Roles */}
            <Route path="/roles" element={<RolesList />} />

            {/* 👤 Usuarios */}
            <Route path="/usuarios" element={<UsuariosList />} />
          </Route>
       

        {/* 🌐 Ruta pública: Login */}
        <Route path="/login" element={<Login />} />
        <Route path="/reset-password" element={<ResetPassword/>} />
        {/* 🚧 Redirección para rutas inexistentes */}
        <Route path="*" element={<Navigate to="/dashboard" />} />
      </Routes>
    </Router>
  );
}
