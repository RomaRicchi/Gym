import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Socio { id: number; nombre: string; }
interface Plan { id: number; nombre: string; }

export default function SuscripcionCreate() {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    socio_id: "",
    plan_id: "",
    inicio: "",
    fin: "",
    estado: false, 
  });

  const [socios, setSocios] = useState<Socio[]>([]);
  const [planes, setPlanes] = useState<Plan[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const resSocios = await gymApi.get("/socios");
        const resPlanes = await gymApi.get("/planes");
        setSocios(resSocios.data.items || resSocios.data);
        setPlanes(resPlanes.data.items || resPlanes.data);
      } catch (err) {
        Swal.fire("Error", "No se pudieron cargar los datos", "error");
      }
    };
    fetchData();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    // Aseguramos que es un HTMLInputElement para acceder a `type` y `checked` de forma segura.
    const target = e.target as HTMLInputElement; 
    const { name, value, type } = target;
    
    // Si es un checkbox, usamos el booleano `checked`, si no, usamos el `value`.
    setForm({ ...form, [name]: type === "checkbox" ? target.checked : value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (new Date(form.fin) < new Date(form.inicio)) {
      Swal.fire("Error", "La fecha de fin no puede ser anterior al inicio", "error");
      return;
    }

    try {
      await gymApi.post("/suscripciones", form);
      Swal.fire("Guardado", "Suscripción creada correctamente", "success");
      navigate("/suscripciones");
    } catch {
      Swal.fire("Error", "No se pudo crear la suscripción", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>➕ Nueva Suscripción</h2>

      <form onSubmit={handleSubmit} className="mt-4">
        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">Socio</label>
            <select
              name="socio_id"
              value={form.socio_id}
              onChange={handleChange}
              className="form-select"
              required
            >
              <option value="">Seleccionar socio...</option>
              {socios.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.nombre}
                </option>
              ))}
            </select>
          </div>

          <div className="col-md-6">
            <label className="form-label">Plan</label>
            <select
              name="plan_id"
              value={form.plan_id}
              onChange={handleChange}
              className="form-select"
              required
            >
              <option value="">Seleccionar plan...</option>
              {planes.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.nombre}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">Inicio</label>
            <input
              type="date"
              name="inicio"
              value={form.inicio}
              onChange={handleChange}
              className="form-control"
              required
            />
          </div>
          <div className="col-md-6">
            <label className="form-label">Fin</label>
            <input
              type="date"
              name="fin"
              value={form.fin}
              onChange={handleChange}
              className="form-control"
              required
            />
          </div>
        </div>

        <div className="form-check mb-3">
          <input
            type="checkbox"
            name="estado"
            checked={form.estado} 
            className="form-check-input"
            id="estado"
          />
          <label className="form-check-label" htmlFor="estado">
            Activa
          </label>
        </div>

        <button type="submit" className="btn btn-primary">
          Guardar
        </button>
      </form>
    </div>
  );
}