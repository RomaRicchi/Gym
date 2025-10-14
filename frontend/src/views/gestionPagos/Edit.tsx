import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Socio { id: number; nombre: string; }
interface Plan { id: number; nombre: string; }
interface Suscripcion { id: number; }
interface Estado { id: number; nombre: string; }

export default function OrdenPagoEdit() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [form, setForm] = useState({
    socio_id: "",
    plan_id: "",
    suscripcion_id: "",
    monto: "",
    vence_en: "",
    estado_id: "",
    notas: "",
  });

  const [socios, setSocios] = useState<Socio[]>([]);
  const [planes, setPlanes] = useState<Plan[]>([]);
  const [suscripciones, setSuscripciones] = useState<Suscripcion[]>([]);
  const [estados, setEstados] = useState<Estado[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [resOrden, resSocios, resPlanes, resSuscripciones, resEstados] = await Promise.all([
          gymApi.get(`/ordenes-pago/${id}`),
          gymApi.get("/socios"),
          gymApi.get("/planes"),
          gymApi.get("/suscripciones"),
          gymApi.get("/estados-orden-pago"), // ⚠️ asegurate que este endpoint exista
        ]);

        const o = resOrden.data;
        setForm({
          socio_id: o.socio_id?.toString() || "",
          plan_id: o.plan_id?.toString() || "",
          suscripcion_id: o.suscripcion_id?.toString() || "",
          monto: o.monto?.toString() || "",
          vence_en: o.vence_en ? o.vence_en.split("T")[0] : "",
          estado_id: o.estado_id?.toString() || "",
          notas: o.notas || "",
        });

        setSocios(resSocios.data.items || resSocios.data);
        setPlanes(resPlanes.data.items || resPlanes.data);
        setSuscripciones(resSuscripciones.data.items || resSuscripciones.data);
        setEstados(resEstados.data.items || resEstados.data);
      } catch (err) {
        console.error(err);
        Swal.fire("Error", "No se pudieron cargar los datos de la orden", "error");
      }
    };

    fetchData();
  }, [id]);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (parseFloat(form.monto) <= 0) {
      Swal.fire("Error", "El monto debe ser mayor a cero", "error");
      return;
    }

    try {
      await gymApi.put(`/ordenes-pago/${id}`, form);
      Swal.fire("Actualizada", "Orden de pago modificada correctamente", "success");
      navigate("/ordenes");
    } catch {
      Swal.fire("Error", "No se pudo actualizar la orden", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>✏️ Editar Orden de Pago</h2>

      <form onSubmit={handleSubmit} className="mt-4">
        <div className="row mb-3">
          <div className="col-md-4">
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

          <div className="col-md-4">
            <label className="form-label">Plan</label>
            <select
              name="plan_id"
              value={form.plan_id}
              onChange={handleChange}
              className="form-select"
            >
              <option value="">Seleccionar plan...</option>
              {planes.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.nombre}
                </option>
              ))}
            </select>
          </div>

          <div className="col-md-4">
            <label className="form-label">Suscripción</label>
            <select
              name="suscripcion_id"
              value={form.suscripcion_id}
              onChange={handleChange}
              className="form-select"
            >
              <option value="">Seleccionar suscripción...</option>
              {suscripciones.map((s) => (
                <option key={s.id} value={s.id}>
                  #{s.id}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">Monto</label>
            <input
              type="number"
              name="monto"
              value={form.monto}
              onChange={handleChange}
              className="form-control"
              min="0.01"
              step="0.01"
              required
            />
          </div>

          <div className="col-md-6">
            <label className="form-label">Vence En</label>
            <input
              type="date"
              name="vence_en"
              value={form.vence_en}
              onChange={handleChange}
              className="form-control"
              required
            />
          </div>
        </div>

        <div className="mb-3">
          <label className="form-label">Estado</label>
          <select
            name="estado_id"
            value={form.estado_id}
            onChange={handleChange}
            className="form-select"
            required
          >
            <option value="">Seleccionar estado...</option>
            {estados.map((e) => (
              <option key={e.id} value={e.id}>
                {e.nombre}
              </option>
            ))}
          </select>
        </div>

        <div className="mb-3">
          <label className="form-label">Notas</label>
          <textarea
            name="notas"
            value={form.notas}
            onChange={handleChange}
            className="form-control"
            rows={3}
          />
        </div>

        <button type="submit" className="btn btn-primary">
          Guardar cambios
        </button>
      </form>
    </div>
  );
}
