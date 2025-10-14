import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Sala { id: number; nombre: string; }
interface Personal { id: number; nombre: string; }

export default function TurnoPlantillaEdit() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [form, setForm] = useState({
    sala_id: "",
    personal_id: "",
    dia_semana_id: "",
    hora_inicio: "",
    duracion_min: "",
    cupo: "",
    activo: false,
  });

  const [salas, setSalas] = useState<Sala[]>([]);
  const [personal, setPersonal] = useState<Personal[]>([]);

  // 🔹 Cargar datos del turno y listas relacionadas
  useEffect(() => {
    const fetchData = async () => {
      try {
        const [resTurno, resSalas, resPersonal] = await Promise.all([
          gymApi.get(`/turnos-plantilla/${id}`),
          gymApi.get("/salas"),
          gymApi.get("/personal"),
        ]);

        const t = resTurno.data;

        setForm({
          sala_id: t.sala_id?.toString() || "",
          personal_id: t.personal_id?.toString() || "",
          dia_semana_id: t.dia_semana_id?.toString() || "",
          hora_inicio: t.hora_inicio || "",
          duracion_min: t.duracion_min?.toString() || "",
          cupo: t.cupo?.toString() || "",
          activo: !!t.activo, // ✅ tinyint → boolean
        });

        setSalas(resSalas.data.items || resSalas.data);
        setPersonal(resPersonal.data.items || resPersonal.data);
      } catch (err) {
        console.error(err);
        Swal.fire("Error", "No se pudieron cargar los datos del turno", "error");
      }
    };

    fetchData();
  }, [id]);

  // 🔹 Manejar cambios
  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const target = e.target as HTMLInputElement;
    const { name, value, type } = target;
    setForm({ ...form, [name]: type === "checkbox" ? target.checked : value });
  };

  // 🔹 Validar y enviar actualización
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const duracion = parseInt(form.duracion_min);
    const cupo = parseInt(form.cupo);

    if (duracion < 10) {
      Swal.fire("Error", "La duración mínima debe ser de 10 minutos", "error");
      return;
    }

    if (cupo < 1) {
      Swal.fire("Error", "El cupo debe ser mayor a 0", "error");
      return;
    }

    try {
      await gymApi.put(`/turnos-plantilla/${id}`, {
        ...form,
        duracion_min: duracion,
        cupo,
        activo: form.activo ? 1 : 0, // ✅ boolean → tinyint
      });

      Swal.fire("Actualizado", "Turno modificado correctamente", "success");
      navigate("/turnos");
    } catch {
      Swal.fire("Error", "No se pudo actualizar el turno", "error");
    }
  };

  const dias = ["Lunes","Martes","Miércoles","Jueves","Viernes","Sábado","Domingo"];

  return (
    <div className="container mt-4">
      <h2>✏️ Editar Turno</h2>

      <form onSubmit={handleSubmit} className="mt-4">
        {/* SALA y PERSONAL */}
        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">Sala</label>
            <select
              name="sala_id"
              value={form.sala_id}
              onChange={handleChange}
              className="form-select"
              required
            >
              <option value="">Seleccionar sala...</option>
              {salas.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.nombre}
                </option>
              ))}
            </select>
          </div>

          <div className="col-md-6">
            <label className="form-label">Profesor</label>
            <select
              name="personal_id"
              value={form.personal_id}
              onChange={handleChange}
              className="form-select"
              required
            >
              <option value="">Seleccionar profesor...</option>
              {personal.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.nombre}
                </option>
              ))}
            </select>
          </div>
        </div>

        {/* DÍA, HORA, DURACIÓN */}
        <div className="row mb-3">
          <div className="col-md-4">
            <label className="form-label">Día de la Semana</label>
            <select
              name="dia_semana_id"
              value={form.dia_semana_id}
              onChange={handleChange}
              className="form-select"
              required
            >
              <option value="">Seleccionar día...</option>
              {dias.map((d, i) => (
                <option key={i + 1} value={i + 1}>
                  {d}
                </option>
              ))}
            </select>
          </div>

          <div className="col-md-4">
            <label className="form-label">Hora de Inicio</label>
            <input
              type="time"
              name="hora_inicio"
              value={form.hora_inicio}
              onChange={handleChange}
              className="form-control"
              required
            />
          </div>

          <div className="col-md-4">
            <label className="form-label">Duración (min)</label>
            <input
              type="number"
              name="duracion_min"
              value={form.duracion_min}
              onChange={handleChange}
              className="form-control"
              min={10}
              required
            />
          </div>
        </div>

        {/* CUPO y ACTIVO */}
        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">Cupo Máximo</label>
            <input
              type="number"
              name="cupo"
              value={form.cupo}
              onChange={handleChange}
              className="form-control"
              min={1}
              required
            />
          </div>

          <div className="col-md-6 d-flex align-items-center">
            <div className="form-check mt-4">
              <input
                type="checkbox"
                name="activo"
                checked={form.activo}
                onChange={handleChange}
                className="form-check-input"
                id="activo"
              />
              <label className="form-check-label" htmlFor="activo">
                Activo
              </label>
            </div>
          </div>
        </div>

        {/* BOTÓN */}
        <button type="submit" className="btn btn-primary">
          Guardar Cambios
        </button>
      </form>
    </div>
  );
}
