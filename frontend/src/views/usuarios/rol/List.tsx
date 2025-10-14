import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Rol {
  id: number;
  nombre: string;
}

export default function RolesList() {
  const [roles, setRoles] = useState<Rol[]>([]);
  const navigate = useNavigate();

  const fetchRoles = async () => {
    try {
      const res = await gymApi.get("/roles");
      setRoles(res.data.items || res.data);
    } catch {
      Swal.fire("Error", "No se pudieron cargar los roles", "error");
    }
  };

  useEffect(() => {
    fetchRoles();
  }, []);

  const handleDelete = async (id: number) => {
    const confirm = await Swal.fire({
      title: "¿Eliminar rol?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (confirm.isConfirmed) {
      try {
        await gymApi.delete(`/roles/${id}`);
        Swal.fire("Eliminado", "Rol eliminado correctamente", "success");
        fetchRoles();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el rol", "error");
      }
    }
  };

  return (
    <div className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2>🧩 Roles del Sistema</h2>
        <button className="btn btn-success" onClick={() => navigate("/roles/nuevo")}>
          ➕ Nuevo Rol
        </button>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {roles.map((r) => (
            <tr key={r.id}>
              <td>{r.id}</td>
              <td>{r.nombre}</td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => navigate(`/roles/editar/${r.id}`)}
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
