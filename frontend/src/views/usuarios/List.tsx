import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Usuario {
  id: number;
  email: string;
  alias: string;
  rol_id: number;
  estado: number;
}

export default function UsuariosList() {
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const navigate = useNavigate();

  const fetchUsuarios = async () => {
    try {
      const res = await gymApi.get("/usuarios");
      setUsuarios(res.data.items || res.data);
    } catch {
      Swal.fire("Error", "No se pudieron cargar los usuarios", "error");
    }
  };

  useEffect(() => {
    fetchUsuarios();
  }, []);

  const handleDelete = async (id: number) => {
    const confirm = await Swal.fire({
      title: "¿Desactivar usuario?",
      text: "Podrás volver a activarlo más tarde.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, desactivar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (confirm.isConfirmed) {
      try {
        await gymApi.delete(`/usuarios/${id}`);
        Swal.fire("Actualizado", "Usuario desactivado correctamente", "success");
        fetchUsuarios();
      } catch {
        Swal.fire("Error", "No se pudo actualizar el usuario", "error");
      }
    }
  };

  return (
    <div className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2>👤 Usuarios del Sistema</h2>
        <button className="btn btn-success" onClick={() => navigate("/usuarios/nuevo")}>
          ➕ Nuevo Usuario
        </button>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>ID</th>
            <th>Alias</th>
            <th>Email</th>
            <th>Rol</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {usuarios.map((u) => (
            <tr key={u.id}>
              <td>{u.id}</td>
              <td>{u.alias}</td>
              <td>{u.email}</td>
              <td>{u.rol_id}</td>
              <td>{u.estado ? "✅ Activo" : "❌ Inactivo"}</td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => navigate(`/usuarios/editar/${u.id}`)}
                >
                  ✏️ Editar
                </button>
                <button
                  className="btn btn-sm btn-outline-danger"
                  onClick={() => handleDelete(u.id)}
                >
                  🔒 Desactivar
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
