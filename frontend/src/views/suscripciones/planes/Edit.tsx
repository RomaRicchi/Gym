import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export default function PlanEdit() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [form, setForm] = useState({
    nombre: "",
    dias_por_semana: "",
    precio: "",
    activo: false,
  });

  useEffect(() => {
    const fetchPlan = async () => {
      try {
        const res = await gymApi.get(`/planes/${id}`);
        const p = res.data;
        setForm({
          nombre: p.nombre,
          dias_por_semana: p.dias_por_semana.toString(),
          precio: p.precio,
          activo: !!p.activo,
        });
      } catch {
        Swal.fire("Error", "No se pudo cargar el plan", "error");
      }
    };
    fetchPlan();
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === "checkbox" ? checked : value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await gymApi.put(`/planes/${id}`, form);
      Swal.fire("Actualizado", "Plan modificado correctamente", "success");
      navigate("/planes");
    } catch {
      Swal.fire("Error", "No se pudo actualizar el plan", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>✏️ Editar Plan</h2>
      <form onSubmit={handleSubmit} className="mt-4">
        <div className="mb-3">
          <label className="form-label">Nombre</label>
          <input
            type="text"
            name="nombre"
            value={form.nombre}
            onChange={handleChange}
            className="form-control"
            required
          />
        </div>

        <div className="mb-3">
          <label className="form-label">Días por semana</label>
          <input
            type="number"
            name="dias_por_semana"
            value={form.dias_por_semana}
            onChange={handleChange}
            className="form-control"
            min={1}
            max={7}
            required
          />
        </div>

        <div className="mb-3">
          <label className="form-label">Precio</label>
          <input
            type="number"
            name="precio"
            value={form.precio}
            onChange={handleChange}
            className="form-control"
            min={0}
            step="0.01"
            required
          />
        </div>

        <div className="form-check mb-3">
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

        <button type="submit" className="btn btn-primary">
          Guardar Cambios
        </button>
      </form>
    </div>
  );
}
