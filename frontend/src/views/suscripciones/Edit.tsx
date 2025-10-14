import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Socio { id: number; nombre: string; }
interface Plan { id: number; nombre: string; }

export default function SuscripcionEdit() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [form, setForm] = useState({
    socio_id: "",
    plan_id: "",
    inicio: "",
    fin: "",
    estado: false, // ✅ valor inicial booleano seguro
  });

  const [socios, setSocios] = useState<Socio[]>([]);
  const [planes, setPlanes] = useState<Plan[]>([]);

  // 🔹 Cargar datos iniciales de la suscripción, socios y planes
  useEffect(() => {
    const fetchData = async () => {
      try {
        const [resSuscripcion, resSocios, resPlanes] = await Promise.all([
          gymApi.get(`/suscripciones/${id}`),
          gymApi.get("/socios"),
          gymApi.get("/planes"),
        ]);

        const s = resSuscripcion.data;

        setForm({
          socio_id: s.socio_id?.toString() || "",
          plan_id: s.plan_id?.toString() || "",
          inicio: s.inicio ? s.inicio.split("T")[0] : "",
          fin: s.fin ? s.fin.split("T")[0] : "",
          estado: !!s.estado, // ✅ convierte tinyint/undefined a boolean
        });

        setSocios(resSocios.data.items || resSocios.data);
        setPlanes(resPlanes.data.items || resPlanes.data);
      } catch (err) {
        Swal.fire("Error", "No se pudieron cargar los datos", "error");
      }
    };
    fetchData();
  }, [id]);

  // 🔹 Manejo genérico de cambios en inputs
  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const target = e.target as HTMLInputElement;
    const { name, value, type } = target;
    setForm({ ...form, [name]: type === "checkbox" ? target.checked : value });
  };

  // 🔹 Envío del formulario
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (new Date(form.fin) < new Date(form.inicio)) {
      Swal.fire("Error", "La fecha de fin no puede ser anterior al inicio", "error");
      return;
    }

    try {
      await gymApi.put(`/suscripciones/${id}`, {
        ...form,
        estado: form.estado ? 1 : 0, // ✅ convierte boolean → tinyint(1)
      });

      Swal.fire("Actualizada", "Suscripción modificada correctamente", "success");
      navigate("/suscripciones");
    } catch {
      Swal.fire("Error", "No se pudo actualizar la suscripción", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>✏️ Editar Suscripción</h2>

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
            checked={form.estado} // ✅ booleano seguro
            onChange={handleChange}
            className="form-check-input"
            id="estado"
          />
          <label className="form-check-label" htmlFor="estado">
            Activa
          </label>
        </div>

        <button type="submit" className="btn btn-primary">
          Guardar cambios
        </button>
      </form>
    </div>
  );
}
