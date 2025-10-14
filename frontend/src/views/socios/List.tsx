import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Socio {
  id: number;
  dni: string;
  nombre: string;
  email: string;
  telefono: string;
  activo: boolean;
  creado_en: string;
}

export default function SociosList() {
  const [socios, setSocios] = useState<Socio[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const navigate = useNavigate();

  const fetchSocios = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get(`/socios?page=${page}&pageSize=${pageSize}&q=${search}`);
      setSocios(res.data.items || res.data);
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

  const handleDelete = async (id: number) => {
    const result = await Swal.fire({
      title: "¿Eliminar socio?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/socios/${id}`);
        Swal.fire("Eliminado", "El socio fue eliminado correctamente", "success");
        fetchSocios(); // recarga la lista
      } catch (err) {
        console.error(err);
        Swal.fire("Error", "No se pudo eliminar el socio", "error");
      }
    }
  };

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearch(e.target.value);
    setPage(1);
  };

  if (loading) return <p>Cargando socios...</p>;

  return (
    <div className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2>Listado de Socios</h2>
        <Link to="/socios/nuevo" className="btn btn-success">
          ➕ Nuevo Socio
        </Link>
      </div>

      <input
        type="text"
        placeholder="Buscar por nombre o DNI..."
        value={search}
        onChange={handleSearch}
        className="form-control mb-3"
      />

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>ID</th>
            <th>DNI</th>
            <th>Nombre</th>
            <th>Email</th>
            <th>Teléfono</th>
            <th>Activo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {socios.map((s) => (
            <tr key={s.id}>
              <td>{s.id}</td>
              <td>{s.dni}</td>
              <td>{s.nombre}</td>
              <td>{s.email}</td>
              <td>{s.telefono}</td>
              <td>{s.activo ? "✅" : "❌"}</td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => navigate(`/socios/editar/${s.id}`)}
                >
                  ✏️ Editar
                </button>
                <button
                  className="btn btn-sm btn-outline-danger"
                  onClick={() => handleDelete(s.id)}
                >
                  🗑️ Eliminar
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* ⏩ Paginación */}
      <div className="d-flex justify-content-between mt-3">
        <button
          className="btn btn-outline-primary"
          onClick={() => setPage((p) => Math.max(p - 1, 1))}
          disabled={page === 1}
        >
          ← Anterior
        </button>
        <span>Página {page}</span>
        <button
          className="btn btn-outline-primary"
          onClick={() => setPage((p) => p + 1)}
          disabled={socios.length < pageSize}
        >
          Siguiente →
        </button>
      </div>
    </div>
  );
}
