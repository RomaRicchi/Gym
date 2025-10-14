import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Sala { id: number; nombre: string; }
interface Personal { id: number; nombre: string; }

export default function TurnoPlantillaCreate() {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    sala_id: "",
    personal_id: "",
    dia_semana_id: "",
    hora_inicio: "",
    duracion_min: "",
    cupo: "",
    activo: true,
  });

  const [salas, setSalas] = useState<Sala[]>([]);
  const [personal, setPersonal] = useState<Personal[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const resSalas = await gymApi.get("/salas");
        const resPersonal = await gymApi.get("/personal");
        setSalas(resSalas.data.items || resSalas.data);
        setPersonal(resPersonal.data.items || resPersonal.data);
      } catch {
        Swal.fire("Error", "No se pudieron cargar los datos", "error");
      }
    };
    fetchData();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const target = e.target as HTMLInputElement;
    const { name, value, type } = target;
    setForm({ ...form, [name]: type === "checkbox" ? target.checked : value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!form.hora_inicio) {
      Swal.fire("Error", "Debe especificar la hora de inicio", "error");
      return;
    }

    try {
      await gymApi.post("/turnos-plantilla", form);
      Swal.fire("Guardado", "Turno creado correctamente", "success");
      navigate("/turnos");
    } catch {
      Swal.fire("Error", "No se pudo crear el turno", "error");
    }
  };

  const dias = ["Lunes","Martes","Miércoles","Jueves","Viernes","Sábado","Domingo"];

  return (
    <div className="container mt-4">
      <h2>➕ Nuevo Turno</h2>

      <form onSubmit={handleSubmit} className="mt-4">
        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">Sala</label>
            <select name="sala_id" value={form.sala_id} onChange={handleChange} className="form-select" required>
              <option value="">Seleccionar sala...</option>
              {salas.map((s) => (
                <option key={s.id} value={s.id}>{s.nombre}</option>
              ))}
            </select>
          </div>

          <div className="col-md-6">
            <label className="form-label">Profesor</label>
            <select name="personal_id" value={form.personal_id} onChange={handleChange} className="form-select" required>
              <option value="">Seleccionar profesor...</option>
              {personal.map((p) => (
                <option key={p.id} value={p.id}>{p.nombre}</option>
              ))}
            </select>
          </div>
        </div>

        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">Día de la Semana</label>
            <select name="dia_semana_id" value={form.dia_semana_id} onChange={handleChange} className="form-select" required>
              <option value="">Seleccionar día...</option>
              {dias.map((d, i) => (
                <option key={i + 1} value={i + 1}>{d}</option>
              ))}
            </select>
          </div>

          <div className="col-md-3">
            <label className="form-label">Hora Inicio</label>
            <input type="time" name="hora_inicio" value={form.hora_inicio} onChange={handleChange} className="form-control" required />
          </div>

          <div className="col-md-3">
            <label className="form-label">Duración (min)</label>
            <input type="number" name="duracion_min" value={form.duracion_min} onChange={handleChange} className="form-control" min={10} required />
          </div>
        </div>

        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">Cupo Máximo</label>
            <input type="number" name="cupo" value={form.cupo} onChange={handleChange} className="form-control" min={1} required />
          </div>

          <div className="col-md-6 d-flex align-items-center">
            <div className="form-check mt-4">
              <input type="checkbox" name="activo" checked={form.activo} onChange={handleChange} className="form-check-input" id="activo" />
              <label className="form-check-label" htmlFor="activo">Activo</label>
            </div>
          </div>
        </div>

        <button type="submit" className="btn btn-primary">Guardar</button>
      </form>
    </div>
  );
}
