import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface TurnoPlantilla {
  id: number;
  dia: string;
  hora_inicio: string;
  sala: string;
  cupo: number;
}

export default function TurnosAssign() {
  const { id } = useParams(); // ID de la suscripción
  const navigate = useNavigate();
  const [turnos, setTurnos] = useState<TurnoPlantilla[]>([]);
  const [selected, setSelected] = useState<number[]>([]);
  const [diasPermitidos, setDiasPermitidos] = useState<number>(0);

  useEffect(() => {
    const fetchData = async () => {
      try {
        // 🔹 Cargamos turnos activos y datos de la suscripción
        const [resTurnos, resSuscripcion] = await Promise.all([
          gymApi.get("/turnosplantilla/activos"),
          gymApi.get(`/suscripciones/${id}`),
        ]);

        setTurnos(resTurnos.data.items || resTurnos.data);
        setDiasPermitidos(resSuscripcion.data.plan?.diasPorSemana || 0);
      } catch {
        Swal.fire("Error", "No se pudieron cargar los turnos disponibles", "error");
      }
    };
    fetchData();
  }, [id]);

  const handleToggle = (turnoId: number) => {
    setSelected((prev) =>
      prev.includes(turnoId)
        ? prev.filter((x) => x !== turnoId)
        : prev.length < diasPermitidos
        ? [...prev, turnoId]
        : (Swal.fire("Límite alcanzado", `Este plan permite ${diasPermitidos} turnos por semana`, "info"), prev)
    );
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (selected.length === 0) {
      Swal.fire("Aviso", "Debe seleccionar al menos un turno", "info");
      return;
    }

    try {
      await gymApi.post(`/suscripciones/${id}/asignar-turnos`, { turnos: selected });
      Swal.fire("Guardado", "Turnos asignados correctamente", "success");
      navigate("/suscripciones");
    } catch {
      Swal.fire("Error", "No se pudieron asignar los turnos", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>➕ Asignar Turnos a Suscripción #{id}</h2>
      <p className="text-muted">Seleccioná hasta {diasPermitidos} turnos según el plan contratado.</p>

      <form onSubmit={handleSubmit}>
        <div className="list-group mb-3">
          {turnos.map((t) => (
            <label
              key={t.id}
              className={`list-group-item list-group-item-action ${
                selected.includes(t.id) ? "active" : ""
              }`}
            >
              <input
                type="checkbox"
                checked={selected.includes(t.id)}
                onChange={() => handleToggle(t.id)}
                className="form-check-input me-2"
              />
              <strong>{t.sala}</strong> — {t.dia} ({t.hora_inicio}) 
              <span className="text-muted ms-2">(Cupo: {t.cupo})</span>
            </label>
          ))}
        </div>

        <button type="submit" className="btn btn-primary">
          Guardar Selección
        </button>
      </form>
    </div>
  );
}
