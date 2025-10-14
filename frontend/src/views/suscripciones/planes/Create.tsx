import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export default function PlanCreate() {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    nombre: "",
    dias_por_semana: "",
    precio: "",
    activo: true,
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === "checkbox" ? checked : value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!form.nombre.trim() || !form.precio) {
      Swal.fire("Error", "Debe ingresar un nombre y precio válidos", "error");
      return;
    }

    try {
      await gymApi.post("/planes", form);
      Swal.fire("Guardado", "Plan creado correctamente", "success");
      navigate("/planes");
    } catch {
      Swal.fire("Error", "No se pudo crear el plan", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>➕ Nuevo Plan</h2>
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
          Guardar
        </button>
      </form>
    </div>
  );
}
