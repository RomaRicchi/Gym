import { useState } from "react";
import { Outlet } from "react-router-dom";
import Sidebar from "@/components/Sidebar";
import Navbar from "@/components/Navbar";
import "@/styles/Layout.css";

export default function Layout() {
  const [isSidebarOpen, setSidebarOpen] = useState(true);

  const toggleSidebar = () => setSidebarOpen(!isSidebarOpen);

  return (
    <div className="layout">
      {/* 🟠 Navbar FitGym */}
      <Navbar onToggleSidebar={toggleSidebar} />

      {/* Contenedor principal */}
      <div className="d-flex">
        {/* Sidebar colapsable */}
        <div
          className={`sidebar-container ${isSidebarOpen ? "open" : "closed"}`}
        >
          <Sidebar />
        </div>

        {/* Contenido central */}
        <main className="flex-grow-1 p-4">
          <Outlet />
        </main>
      </div>

      {/* Overlay para móvil */}
      {isSidebarOpen && (
        <div
          className="sidebar-overlay d-md-none"
          onClick={toggleSidebar}
        ></div>
      )}
    </div>
  );
}
