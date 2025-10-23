import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Turno {
  id: number;
  turnoPlantilla?: {
    hora_inicio: string;
    duracion_min: number;
    dia_semana?: { nombre: string };
    sala?: { nombre: string };
    personal?: { nombre: string };
  };
}

export default function TurnosList() {
  const { id } = useParams(); // id de la suscripción
  const navigate = useNavigate();
  const [turnos, setTurnos] = useState<Turno[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchTurnos = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get(`/SuscripcionesTurno/suscripcion/${id}`);
      setTurnos(res.data.items || res.data);
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudieron cargar los turnos", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTurnos();
  }, [id]);

  const handleDelete = async (turnoId: number) => {
    const confirm = await Swal.fire({
      title: "¿Eliminar turno?",
      text: "Esta acción quitará el turno de la suscripción.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (confirm.isConfirmed) {
      try {
        await gymApi.delete(`/SuscripcionesTurno/${turnoId}`);
        Swal.fire("Eliminado", "Turno eliminado correctamente", "success");
        fetchTurnos();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el turno", "error");
      }
    }
  };

  const handleAssign = () => {
    navigate(`/suscripciones/${id}/asignar-turnos`);
  };

  if (loading) return <p className="text-center mt-4">Cargando turnos...</p>;

  return (
    <div className="container mt-4">
      <h1 className="text-center fw-bold mb-4" style={{ color: "#ff6600" }}>
        🧾 Turnos de la Suscripción #{id}
      </h1>

      <div className="d-flex justify-content-between mb-3">
        <button className="btn btn-success" onClick={handleAssign}>
          ➕ Asignar nuevos turnos
        </button>
        <button className="btn btn-outline-secondary" onClick={() => navigate("/suscripciones")}>
          ⬅️ Volver a suscripciones
        </button>
      </div>

      {turnos.length === 0 ? (
        <p className="text-center">No hay turnos asignados.</p>
      ) : (
        <table className="table table-striped table-hover text-center align-middle">
          <thead className="table-dark">
            <tr>
              <th>#</th>
              <th>Día</th>
              <th>Hora</th>
              <th>Sala</th>
              <th>Profesor</th>
              <th>Duración</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {turnos.map((t) => (
              <tr key={t.id}>
                <td>{t.id}</td>
                <td>{t.turnoPlantilla?.dia_semana?.nombre || "—"}</td>
                <td>{t.turnoPlantilla?.hora_inicio || "—"}</td>
                <td>{t.turnoPlantilla?.sala?.nombre || "—"}</td>
                <td>{t.turnoPlantilla?.personal?.nombre || "—"}</td>
                <td>{t.turnoPlantilla?.duracion_min || "—"} min</td>
                <td>
                  <button
                    className="btn btn-sm btn-outline-danger"
                    onClick={() => handleDelete(t.id)}
                  >
                    🗑️ Eliminar
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
