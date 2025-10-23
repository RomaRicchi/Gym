import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface TurnoPlantilla {
  id: number;
  hora_inicio: string;
  duracion_min: number;
  dia_semana?: { nombre: string };
  sala?: { nombre: string };
  personal?: { nombre: string };
}

export default function TurnosAssign() {
  const { id } = useParams(); // id de la suscripción
  const navigate = useNavigate();

  const [turnos, setTurnos] = useState<TurnoPlantilla[]>([]);
  const [selected, setSelected] = useState<number[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchTurnos = async () => {
    try {
      const res = await gymApi.get("/TurnosPlantilla");
      setTurnos(res.data.items || res.data);
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudieron cargar los turnos disponibles", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTurnos();
  }, []);

  const toggleSelect = (turnoId: number) => {
    setSelected((prev) =>
      prev.includes(turnoId) ? prev.filter((id) => id !== turnoId) : [...prev, turnoId]
    );
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (selected.length === 0) {
      Swal.fire("Aviso", "Debe seleccionar al menos un turno", "info");
      return;
    }

    try {
      await Promise.all(
        selected.map((turnoId) =>
          gymApi.post("/SuscripcionesTurno", {
            suscripcionId: Number(id),
            turnoPlantillaId: turnoId,
          })
        )
      );
      Swal.fire("✅ Éxito", "Turnos asignados correctamente", "success");
      navigate(`/suscripciones/turnos`);
    } catch {
      Swal.fire("Error", "No se pudieron asignar los turnos", "error");
    }
  };

  if (loading) return <p className="text-center mt-4">Cargando turnos disponibles...</p>;

  return (
    <div className="container mt-4">
      <h1 className="text-center fw-bold mb-4" style={{ color: "#ff6600" }}>
        ➕ Asignar Turnos a Suscripción #{id}
      </h1>

      <form onSubmit={handleSubmit}>
        <table className="table table-hover text-center align-middle">
          <thead className="table-dark">
            <tr>
              <th></th>
              <th>Día</th>
              <th>Hora</th>
              <th>Sala</th>
              <th>Profesor</th>
              <th>Duración</th>
            </tr>
          </thead>
          <tbody>
            {turnos.map((t) => (
              <tr
                key={t.id}
                className={selected.includes(t.id) ? "table-success" : ""}
                onClick={() => toggleSelect(t.id)}
                style={{ cursor: "pointer" }}
              >
                <td>
                  <input
                    type="checkbox"
                    checked={selected.includes(t.id)}
                    onChange={() => toggleSelect(t.id)}
                  />
                </td>
                <td>{t.dia_semana?.nombre || "—"}</td>
                <td>{t.hora_inicio || "—"}</td>
                <td>{t.sala?.nombre || "—"}</td>
                <td>{t.personal?.nombre || "—"}</td>
                <td>{t.duracion_min || "—"} min</td>
              </tr>
            ))}
          </tbody>
        </table>

        <div className="d-flex justify-content-between mt-3">
          <button type="submit" className="btn btn-primary">
            Guardar turnos seleccionados
          </button>
          <button
            type="button"
            className="btn btn-outline-secondary"
            onClick={() => navigate(`/suscripciones/turnos`)}
          >
            ⬅️ Volver
          </button>
        </div>
      </form>
    </div>
  );
}
