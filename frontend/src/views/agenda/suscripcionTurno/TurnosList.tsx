import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Turno {
  id: number;
  turno_plantilla_id: number;
  dia: string;
  hora_inicio: string;
  hora_fin: string;
}

export default function TurnosList() {
  const { id } = useParams(); // id de la suscripción
  const [turnos, setTurnos] = useState<Turno[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchTurnos = async () => {
    try {
      const res = await gymApi.get(`/suscripciones/${id}/turnos`);
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
    const result = await Swal.fire({
      title: "¿Eliminar turno?",
      text: "Esto quitará el turno de la suscripción.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
    });

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/suscripcion-turnos/${turnoId}`);
        Swal.fire("Eliminado", "Turno quitado correctamente", "success");
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
        TURNOS
      </h1>
      <div className="d-flex justify-content-between align-items-center mb-3">
        
        <Link to={`/suscripciones/${id}/asignar-turnos`} className="btn btn-success">
          ➕ Asignar Turnos
        </Link>
      </div>

      {turnos.length === 0 ? (
        <p>No hay turnos asignados.</p>
      ) : (
        <table className="table table-striped table-hover">
          <thead className="table-dark">
            <tr>
              <th>ID</th>
              <th>Plantilla</th>
              <th>Día</th>
              <th>Hora Inicio</th>
              <th>Hora Fin</th>
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
                <td>{t.hora_fin || "—"}</td>
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
