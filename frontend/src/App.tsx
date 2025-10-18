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

// 🧾 Órdenes de pago
import OrdenesList from "@/views/gestionPagos/List";
import OrdenPagoCreate from "@/views/gestionPagos/Create";
import OrdenPagoEdit from "@/views/gestionPagos/Edit";

// 📑 Comprobantes
import ComprobantesList from "@/views/gestionPagos/comprobantes/List";
import ComprobanteUpload from "@/views/gestionPagos/comprobantes/Upload";

// ⚙️ Estados de orden de pago
import EstadosList from "@/views/gestionPagos/estado/List";
import EstadoCreate from "@/views/gestionPagos/estado/Create";
import EstadoEdit from "@/views/gestionPagos/estado/Edit";

// 🔗 Orden-Turno
import OrdenTurnosList from "@/views/agenda/ordenTurno/List";
import OrdenTurnoAssign from "@/views/agenda/ordenTurno/Assign";

// 🧩 Roles
import RolesList from "@/views/usuarios/rol/List";

// 👤 Usuarios
import UsuariosList from "@/views/usuarios/List";
import UsuarioCreate from "@/views/usuarios/Create";
import UsuarioEdit from "@/views/usuarios/Edit";

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

            {/* 📆 Turnos Plantilla */}
            <Route path="/turnos" element={<TurnosPlantillaList />} />
            <Route path="/turnos/nuevo" element={<TurnoPlantillaCreate />} />
            <Route path="/turnos/editar/:id" element={<TurnoPlantillaEdit />} />

            {/* 🧾 Órdenes de Pago */}
            <Route path="/ordenes" element={<OrdenesList />} />
            <Route path="/ordenes/nueva" element={<OrdenPagoCreate />} />
            <Route path="/ordenes/editar/:id" element={<OrdenPagoEdit />} />

            {/* 📑 Comprobantes */}
            <Route path="/comprobantes" element={<ComprobantesList />} />
            <Route path="/ordenes/:id/comprobantes" element={<ComprobantesList />} />
            <Route path="/ordenes/:id/subir-comprobante" element={<ComprobanteUpload />} />

            {/* ⚙️ Estados */}
            <Route path="/estados" element={<EstadosList />} />
            <Route path="/estados/nuevo" element={<EstadoCreate />} />
            <Route path="/estados/editar/:id" element={<EstadoEdit />} />

            {/* 🔗 Orden - Turnos */}
            <Route path="/ordenes/:id/turnos" element={<OrdenTurnosList />} />
            <Route path="/ordenes/:id/asignar-turnos" element={<OrdenTurnoAssign />} />

            {/* 🧩 Roles */}
            <Route path="/roles" element={<RolesList />} />

            {/* 👤 Usuarios */}
            <Route path="/usuarios" element={<UsuariosList />} />
            <Route path="/usuarios/nuevo" element={<UsuarioCreate />} />
            <Route path="/usuarios/editar/:id" element={<UsuarioEdit />} />
          </Route>
       

        {/* 🌐 Ruta pública: Login */}
        <Route path="/login" element={<Login />} />

        {/* 🚧 Redirección para rutas inexistentes */}
        <Route path="*" element={<Navigate to="/dashboard" />} />
      </Routes>
    </Router>
  );
}
