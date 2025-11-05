import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { EvaluacionCreateSwal } from "@/views/rutinas/evaluacion/EvaluacionCreateSwal";
import { EvaluacionEditSwal } from "@/views/rutinas/evaluacion/EvaluacionEditSwal";

interface Evaluacion {
  id: number;
  rutinaAsignadaNombre?: string;
  personalNombre?: string;
  fechaEvaluacion: string;
  observaciones?: string;
}

export default function EvaluacionesList() {
  const [evaluaciones, setEvaluaciones] = useState<Evaluacion[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchEvaluaciones = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get("/evaluaciones");
      setEvaluaciones(res.data.items || res.data);
      setError(null);
    } catch {
      setError("Error al cargar las evaluaciones");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchEvaluaciones();
  }, []);

  const handleDelete = async (id: number) => {
    const result = await Swal.fire({
      title: "¿Eliminar evaluación?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/evaluaciones/${id}`);
        await Swal.fire("Eliminado", "Evaluación eliminada correctamente", "success");
        fetchEvaluaciones();
      } catch {
        Swal.fire("Error", "No se pudo eliminar la evaluación", "error");
      }
    }
  };

  if (loading) return <p>Cargando evaluaciones...</p>;
  if (error) return <p className="text-danger">{error}</p>;

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.3rem", letterSpacing: "2px" }}
      >
        🧾 EVALUACIONES
      </h1>

      <div className="d-flex justify-content-between align-items-center mb-3">
        <button
          className="btn btn-success"
          onClick={() => EvaluacionCreateSwal(fetchEvaluaciones)}
        >
          ➕ Nueva Evaluación
        </button>
      </div>

      <div className="table-responsive">
        <table className="table table-striped table-hover align-middle">
          <thead className="table-dark">
            <tr>
              <th>Rutina Asignada</th>
              <th>Profesor</th>
              <th>Fecha</th>
              <th>Observaciones</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {evaluaciones.map((e) => (
              <tr key={e.id}>
                <td>{e.rutinaAsignadaNombre || "-"}</td>
                <td>{e.personalNombre || "-"}</td>
                <td>{new Date(e.fechaEvaluacion).toLocaleDateString("es-AR")}</td>
                <td style={{ maxWidth: "400px", whiteSpace: "pre-wrap" }}>
                  {e.observaciones || "-"}
                </td>
                <td>
                  <button
                    className="btn btn-sm btn-warning me-2"
                    onClick={() => EvaluacionEditSwal(e.id.toString(), fetchEvaluaciones)}
                  >
                    ✏️
                  </button>
                  <button
                    className="btn btn-sm btn-danger"
                    onClick={() => handleDelete(e.id)}
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
