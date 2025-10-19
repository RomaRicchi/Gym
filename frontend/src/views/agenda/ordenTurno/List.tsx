import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Turno {
  id: number;
  turno_plantilla_id: number;
  dia?: string;
  hora_inicio?: string;
  sala?: string;
}

export default function OrdenTurnosList() {
  const { id } = useParams(); // ID de la orden
  const [turnos, setTurnos] = useState<Turno[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchTurnos = async () => {
    try {
      const res = await gymApi.get(`/ordenes/${id}/turnos`);
      setTurnos(res.data.items || res.data);
    } catch {
      Swal.fire("Error", "No se pudieron cargar los turnos asociados", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTurnos();
  }, [id]);

  const handleDelete = async (turnoId: number) => {
    const confirm = await Swal.fire({
      title: "¿Quitar turno?",
      text: "Esto desvinculará el turno de esta orden.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, quitar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (confirm.isConfirmed) {
      try {
        await gymApi.delete(`/orden-turnos/${turnoId}`);
        Swal.fire("Quitado", "Turno eliminado correctamente", "success");
        fetchTurnos();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el turno", "error");
      }
    }
  };

  if (loading) return <p>Cargando turnos...</p>;

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        TURNOS ASIGNADOS A SOCIO
      </h1>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <Link to={`/ordenes/${id}/asignar-turnos`} className="btn btn-success">
          ➕ Asignar Turnos
        </Link>
      </div>

      {turnos.length === 0 ? (
        <p>No hay turnos asociados a esta orden.</p>
      ) : (
        <table className="table table-striped table-hover">
          <thead className="table-dark">
            <tr>
              <th>ID</th>
              <th>Turno Plantilla</th>
              <th>Día</th>
              <th>Hora Inicio</th>
              <th>Sala</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {turnos.map((t) => (
              <tr key={t.id}>
                <td>{t.id}</td>
                <td>{t.turno_plantilla_id}</td>
                <td>{t.dia || "—"}</td>
                <td>{t.hora_inicio || "—"}</td>
                <td>{t.sala || "—"}</td>
                <td>
                  <button
                    className="btn btn-sm btn-outline-danger"
                    onClick={() => handleDelete(t.id)}
                  >
                    🗑️ Quitar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
