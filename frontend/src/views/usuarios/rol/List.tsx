import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { RolCreateSwal } from "@/views/usuarios/rol/RolCreateSwal";
import { RolEditSwal } from "@/views/usuarios/rol/RolEditSwal";

interface Rol {
  id: number;
  nombre: string;
}

export default function RolesList() {
  const [roles, setRoles] = useState<Rol[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchRoles = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get("/roles");
      setRoles(res.data.items || res.data);
      setError(null);
    } catch {
      setError("Error al cargar los roles");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRoles();
  }, []);

  const handleDelete = async (id: number) => {
    const result = await Swal.fire({
      title: "¿Eliminar rol?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/roles/${id}`);
        await Swal.fire("Eliminado", "Rol eliminado correctamente", "success");
        fetchRoles();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el rol", "error");
      }
    }
  };

  if (loading) return <p>Cargando roles...</p>;
  if (error) return <p className="text-danger">{error}</p>;

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        ROLES DEL SISTEMA
      </h1>

      <div className="d-flex justify-content-between align-items-center mb-3">
        <button className="btn btn-success" onClick={() => RolCreateSwal(fetchRoles)}>
          ➕ Nuevo Rol
        </button>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>Nombre</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {roles.map((r) => (
            <tr key={r.id}>
              <td>{r.nombre}</td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => RolEditSwal(r.id.toString(), fetchRoles)}
                >
                  ✏️ Editar
                </button>
                <button
                  className="btn btn-sm btn-outline-danger"
                  onClick={() => handleDelete(r.id)}
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
