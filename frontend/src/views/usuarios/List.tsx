import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import Pagination from "@/components/Pagination";
import gymApi from "@/api/gymApi";
import { UsuarioEditSwal } from "@/views/usuarios/UsuarioEditSwal";

interface Usuario {
  id: number;
  email: string;
  alias: string;
  rol: string;
  estado: boolean | number;
}

export default function UsuariosList() {
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalItems, setTotalItems] = useState(0);
  const [search, setSearch] = useState("");
  
  const fetchUsuarios = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get(
        `/usuarios?page=${page}&pageSize=${pageSize}&q=${search}`
      );

      const data = res.data;
      const items = data.items || data;
      setTotalItems(data.totalItems || items.length);

      const adaptados = items.map((u: any) => ({
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
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsuarios();
  }, [page, search]);

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

  if (loading)
    return (
      <div className="text-center mt-5">
        <div className="spinner-border text-warning" role="status"></div>
        <p className="mt-3">Cargando usuarios...</p>
      </div>
    );

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        USUARIOS
      </h1>

      <div className="d-flex justify-content-between align-items-center mb-3">
        <input
          type="text"
          placeholder="Buscar por alias o email..."
          className="form-control"
          value={search}
          onChange={(e) => {
            setSearch(e.target.value);
            setPage(1);
          }}
          style={{ width: "40%" }}
        />
      </div>

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
