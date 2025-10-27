import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Pagination from "@/components/Pagination";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { mostrarFormNuevoSocio } from "@/views/socios/SociosCreateSwal";
import { mostrarFormEditarSocio } from "@/views/socios/SociosEditSwal";
import { crearOrdenDePago } from "@/views/gestionPagos/formOrdenPago";

import $ from "jquery";
import "select2";
interface Socio {
  id: number;
  dni: string;
  nombre: string;
  email: string;
  telefono: string;
  activo: boolean;
  creado_en: string;
  fechaNacimiento?: string;
  planActual?: string;
}

export default function SociosList() {
  const [socios, setSocios] = useState<Socio[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalItems, setTotalItems] = useState(0);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const navigate = useNavigate();

  const fetchSocios = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get(`/socios?page=${page}&pageSize=${pageSize}&q=${search}`);
      console.log("📦 DATA BACKEND:", res.data);
      const data = res.data;

      setSocios(data.items || []);
      setTotalItems(data.totalItems || data.total || 0); 
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudieron cargar los socios", "error");
    } finally {
      setLoading(false);
    }
  };


  useEffect(() => {
    fetchSocios();
  }, [page, search]);

  const handleDelete = async (id: number, nombre: string) => {
    const result = await Swal.fire({
      title: `¿Dar de baja a ${nombre}?`,
      text: "El socio no se eliminará del sistema, solo se marcará como inactivo.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, dar de baja",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (result.isConfirmed) {
      try {
        await gymApi.patch(`/socios/${id}/bajaLogica?value=false`);
        Swal.fire({
          icon: "success",
          title: "Socio dado de baja",
          text: `${nombre} fue marcado como inactivo.`,
          timer: 1800,
          showConfirmButton: false,
        });
        fetchSocios(); // recarga la lista
      } catch (err) {
        console.error(err);
        Swal.fire("Error", "No se pudo dar de baja al socio.", "error");
      }
    }
  };


  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearch(e.target.value);
    setPage(1);
  };

  const handleSuscribirse = async (id: number, nombre: string) => {
    await crearOrdenDePago({ id, nombre });
  };


  if (loading) return <p>Cargando socios...</p>;

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        SOCIOS
      </h1>

      <div className="d-flex justify-content-between align-items-center mb-3">
        
        <div className="flex-grow-1 d-flex justify-content-start">
          <input
            type="text"
            placeholder="Buscar por nombre o DNI..."
            value={search}
            onChange={handleSearch}
            className="form-control"
            style={{ width: "50%" }}
          />
        </div>

        <button
          className="btn btn-success ms-3"
          onClick={async () => {
            const creado = await mostrarFormNuevoSocio();
            if (creado) fetchSocios();
          }}
        >
          ➕ Nuevo Socio
        </button>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>DNI</th>
            <th>Nombre</th>
            <th>Email</th>
            <th>Teléfono</th>
            <th>Plan actual</th>
            <th>Activo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {socios.map((s) => (
            <tr key={s.id}>
              <td>{s.dni}</td>
              <td>{s.nombre}</td>
              <td>{s.email}</td>
              <td>{s.telefono}</td>
              <td>{s.planActual || "— Sin plan —"}</td>
              <td>{s.activo ? "✅" : "❌"}</td>
              <td>
                <button
                  className="btn btn-sm btn-warning"
                  onClick={async () => {
                    const actualizado = await mostrarFormEditarSocio(s.id);
                    if (actualizado) fetchSocios();
                  }}
                >
                  ✏️ 
                </button>
                <button
                  className="btn btn-sm btn-success"
                  onClick={() => handleSuscribirse(s.id, s.nombre)}
                >
                  💳
                </button>
                <button
                  className="btn btn-sm btn-danger"
                  onClick={() => handleDelete(s.id, s.nombre)}
                >
                  🗑️ 
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <Pagination
        currentPage={page}
        totalPages={Math.ceil(totalItems / pageSize)}
        totalItems={totalItems}
        pageSize={pageSize}
        onPageChange={(newPage) => setPage(newPage)}
      />

    </div>
  );

}
