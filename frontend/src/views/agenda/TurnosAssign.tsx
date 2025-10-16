import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface TurnoPlantilla {
  id: number;
  sala: string;
  dia: string;
  hora_inicio: string;
  hora_fin: string;
}

export default function TurnosAssign() {
  const { id } = useParams(); // id de suscripción
  const navigate = useNavigate();
  const [turnos, setTurnos] = useState<TurnoPlantilla[]>([]);
  const [selected, setSelected] = useState<number[]>([]);

  useEffect(() => {
    const fetchTurnos = async () => {
      try {
        const res = await gymApi.get("/turnosplantilla");
        setTurnos(res.data.items || res.data);
      } catch {
        Swal.fire("Error", "No se pudieron cargar los turnos disponibles", "error");
      }
    };
    fetchTurnos();
  }, []);

  const handleToggle = (id: number) => {
    setSelected((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (selected.length === 0) {
      Swal.fire("Aviso", "Debe seleccionar al menos un turno", "info");
      return;
    }

    try {
      await gymApi.post(`/suscripciones/${id}/turnos`, { turnos: selected });
      Swal.fire("Guardado", "Turnos asignados correctamente", "success");
      navigate(`/suscripciones/${id}/turnos`);
    } catch {
      Swal.fire("Error", "No se pudieron asignar los turnos", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>➕ Asignar Turnos</h2>
      <p className="text-muted">Seleccioná los turnos que querés agregar a esta suscripción.</p>

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
              <strong>{t.sala}</strong> — {t.dia} ({t.hora_inicio} - {t.hora_fin})
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
