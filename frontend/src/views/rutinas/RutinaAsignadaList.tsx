import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { RutinaAsignadaCreateSwal } from "@/views/rutinas/RutinaAsignadaCreateSwal";
import { RutinaAsignadaEditSwal } from "@/views/rutinas/RutinaAsignadaEditSwal";

interface RutinaAsignada {
  id: number;
  rutinaPlantillaNombre?: string;
  socioNombre?: string;
  personalNombre?: string;
  fechaAsignacion: string;
  observaciones?: string;
}

export default function RutinaAsignadaList() {
  const [rutinas, setRutinas] = useState<RutinaAsignada[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchRutinas = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get("/rutinaasignada");
      setRutinas(res.data.items || res.data);
      setError(null);
    } catch {
      setError("Error al cargar las rutinas asignadas");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRutinas();
  }, []);

  const handleDelete = async (id: number) => {
    const result = await Swal.fire({
      title: "¿Eliminar rutina asignada?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/rutinasasignadas/${id}`);
        await Swal.fire("Eliminado", "Rutina eliminada correctamente", "success");
        fetchRutinas();
      } catch {
        Swal.fire("Error", "No se pudo eliminar la rutina", "error");
      }
    }
  };

  if (loading) return <p>Cargando rutinas asignadas...</p>;
  if (error) return <p className="text-danger">{error}</p>;

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.3rem", letterSpacing: "2px" }}
      >
        🧡 RUTINAS ASIGNADAS
      </h1>

      <div className="d-flex justify-content-between align-items-center mb-3">
        <button
          className="btn btn-success"
          onClick={() => RutinaAsignadaCreateSwal(fetchRutinas)}
        >
          ➕ Nueva Asignación
        </button>
      </div>

      <div className="table-responsive">
        <table className="table table-striped table-hover align-middle">
          <thead className="table-dark">
            <tr>
              <th>Rutina</th>
              <th>Socio</th>
              <th>Profesor</th>
              <th>Fecha</th>
              <th>Observaciones</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {rutinas.map((r) => (
              <tr key={r.id}>
                <td>{r.rutinaPlantillaNombre || "-"}</td>
                <td>{r.socioNombre || "-"}</td>
                <td>{r.personalNombre || "-"}</td>
                <td>{new Date(r.fechaAsignacion).toLocaleDateString("es-AR")}</td>
                <td>{r.observaciones || "-"}</td>
                <td>
                  <button
                    className="btn btn-sm btn-warning me-2"
                    onClick={() => RutinaAsignadaEditSwal(r.id.toString(), fetchRutinas)}
                  >
                    ✏️
                  </button>
                  <button
                    className="btn btn-sm btn-danger"
                    onClick={() => handleDelete(r.id)}
                  >
                    🗑️
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
