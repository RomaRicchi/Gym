import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { UsuarioEditSwal } from "@/views/usuarios/UsuarioEditSwal";

interface Usuario {
  id: number;
  email: string;
  alias: string;
  rol: string; // 👈 mostrar nombre del rol
  estado: boolean | number;
}

export default function UsuariosList() {
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const navigate = useNavigate();

  // 🔹 Cargar usuarios
  const fetchUsuarios = async () => {
    try {
      const res = await gymApi.get("/usuarios");
      const data = res.data.items || res.data;

      // Adaptar datos en caso de estructura diferente
      const adaptados = data.map((u: any) => ({
        id: u.id,
        email: u.email,
        alias: u.alias,
        rol: u.rol?.nombre || u.rol || "(Sin rol)",
        estado: u.estado ?? 0,
      }));

      setUsuarios(adaptados);
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudieron cargar los usuarios", "error");
    }
  };

  useEffect(() => {
    fetchUsuarios();
  }, []);

  // 🔸 Desactivar usuario
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

  // 🔸 Render
  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        USUARIOS
      </h1>


      <table className="table table-striped table-hover align-middle">
        <thead className="table-dark text-center">
          <tr>
            <th>Alias</th>
            <th>Email</th>
            <th>Rol</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {usuarios.length === 0 ? (
            <tr>
              <td colSpan={5} className="text-center text-muted py-3">
                No hay usuarios registrados.
              </td>
            </tr>
          ) : (
            usuarios.map((u) => (
              <tr key={u.id} className="text-center">
                <td>{u.alias}</td>
                <td>{u.email || "—"}</td>
                <td>{u.rol}</td>
                <td>{u.estado ? "✅ Activo" : "❌ Inactivo"}</td>
                <td>
                  <div className="d-flex justify-content-center gap-2">
                    <button
                      className="btn btn-warning btn-sm"
                      onClick={() => UsuarioEditSwal(u.id, fetchUsuarios)}
                      title="Editar usuario"
                    >
                      ✏️
                    </button>
                    <button
                      className="btn btn-sm btn-outline-danger"
                      onClick={() => handleDelete(u.id)}
                      title="Desactivar usuario"
                    >
                      🔒
                    </button>
                  </div>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
