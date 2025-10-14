import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Personal {
  id: number;
  nombre: string;
  email: string;
  telefono: string;
  rol: string;
  activo: boolean;
}

export default function PersonalList() {
  const [personal, setPersonal] = useState<Personal[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const fetchPersonal = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get("/personal");
      setPersonal(res.data.items || res.data);
    } catch (err) {
      console.error(err);
      setError("Error al cargar el personal.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPersonal();
  }, []);

  const handleDelete = async (id: number) => {
    const result = await Swal.fire({
      title: "¿Eliminar registro?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/personal/${id}`);
        Swal.fire("Eliminado", "El registro fue eliminado correctamente", "success");
        fetchPersonal();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el registro", "error");
      }
    }
  };

  if (loading) return <p>Cargando...</p>;
  if (error) return <p className="text-danger">{error}</p>;

  return (
    <div className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2>Personal</h2>
        <Link to="/personal/nuevo" className="btn btn-success">
          ➕ Nuevo
        </Link>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Email</th>
            <th>Teléfono</th>
            <th>Rol</th>
            <th>Activo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {personal.map((p) => (
            <tr key={p.id}>
              <td>{p.id}</td>
              <td>{p.nombre}</td>
              <td>{p.email}</td>
              <td>{p.telefono}</td>
              <td>{p.rol}</td>
              <td>{p.activo ? "✅" : "❌"}</td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => navigate(`/personal/editar/${p.id}`)}
                >
                  ✏️ Editar
                </button>
                <button
                  className="btn btn-sm btn-outline-danger"
                  onClick={() => handleDelete(p.id)}
                >
                  🗑️ Eliminar
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
