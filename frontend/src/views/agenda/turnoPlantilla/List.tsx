import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface TurnoPlantilla {
  id: number;
  sala_id: number;
  personal_id: number;
  dia_semana_id: number;
  hora_inicio: string;
  duracion_min: number;
  cupo: number;
  activo: boolean | number;
}

export default function TurnosPlantillaList() {
  const [turnos, setTurnos] = useState<TurnoPlantilla[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  const fetchTurnos = async () => {
    try {
      const res = await gymApi.get("/turnos-plantilla");
      const data = res.data.items || res.data;
      const parsed = data.map((t: any) => ({ ...t, activo: Boolean(t.activo) }));
      setTurnos(parsed);
    } catch {
      Swal.fire("Error", "No se pudieron cargar los turnos", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTurnos();
  }, []);

  const handleDelete = async (id: number) => {
    const result = await Swal.fire({
      title: "¿Eliminar turno?",
      text: "Esto quitará el turno plantilla del sistema.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/turnos-plantilla/${id}`);
        Swal.fire("Eliminado", "Turno eliminado correctamente", "success");
        fetchTurnos();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el turno", "error");
      }
    }
  };

  if (loading) return <p>Cargando turnos...</p>;

  const diasSemana = ["Lunes","Martes","Miércoles","Jueves","Viernes","Sábado","Domingo"];

  return (
    <div className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2>Turnos Plantilla</h2>
        <button onClick={() => navigate("/turnos/nuevo")} className="btn btn-success">
          ➕ Nuevo Turno
        </button>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>ID</th>
            <th>Sala</th>
            <th>Profesor</th>
            <th>Día</th>
            <th>Inicio</th>
            <th>Duración</th>
            <th>Cupo</th>
            <th>Activo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {turnos.map((t) => (
            <tr key={t.id}>
              <td>{t.id}</td>
              <td>{t.sala_id}</td>
              <td>{t.personal_id}</td>
              <td>{diasSemana[t.dia_semana_id - 1] || "-"}</td>
              <td>{t.hora_inicio}</td>
              <td>{t.duracion_min} min</td>
              <td>{t.cupo}</td>
              <td>{t.activo ? "✅" : "❌"}</td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => navigate(`/turnos/editar/${t.id}`)}
                >
                  ✏️ Editar
                </button>
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
    </div>
  );
}
