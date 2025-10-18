import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { PersonalCreateSwal } from "@/views/personal/PersonalCreateSwal";
import { PersonalEditSwal } from "@/views/personal/PersonalEditSwal";

interface Personal {
  id: number;
  nombre: string;
  email: string;
  telefono: string;
  direccion: string;     
  especialidad: string;
  rol: string;
  activo: boolean;
}

export default function PersonalList() {
  const [personal, setPersonal] = useState<Personal[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchPersonal = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get("/personal");
      const data = res.data.items || res.data;
      setPersonal(data);
      setError(null);
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
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        PERSONAL
      </h1>

      <div className="d-flex justify-content-between align-items-center mb-3">
        <button
          onClick={() => PersonalCreateSwal(fetchPersonal)}
          className="btn btn-success"
        >
          ➕ Nuevo Personal
        </button>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>Nombre</th>
            <th>Email</th>
            <th>Teléfono</th>
            <th>Dirección</th>    
            <th>Especialidad</th>
            <th>Rol</th>
            <th>Activo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {personal.map((p) => (
            <tr key={p.id}>
              <td>{p.nombre}</td>
              <td>{p.email || "-"}</td>
              <td>{p.telefono || "-"}</td>
              <td>{p.direccion || "-"}</td> 
              <td>{p.especialidad || "-"}</td>
              <td>{p.rol || "(Sin rol)"}</td>
              <td>{p.activo ? "✅" : "❌"}</td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => PersonalEditSwal(p.id.toString(), fetchPersonal)}
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
