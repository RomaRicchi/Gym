import { useState } from "react";
import { useNavigate } from "react-router-dom";
import gymApi from "@/api/gymApi";

export default function PersonalCreate() {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    nombre: "",
    email: "",
    telefono: "",
    rol: "",
    activo: true,
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === "checkbox" ? checked : value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await gymApi.post("/personal", form);
      navigate("/personal");
    } catch {
      alert("Error al crear el registro");
    }
  };

  return (
    <div className="container mt-4">
      <h2>➕ Nuevo Personal</h2>

      <form onSubmit={handleSubmit} className="mt-4">
        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">Nombre</label>
            <input
              name="nombre"
              value={form.nombre}
              onChange={handleChange}
              className="form-control"
              required
            />
          </div>
          <div className="col-md-6">
            <label className="form-label">Email</label>
            <input
              name="email"
              type="email"
              value={form.email}
              onChange={handleChange}
              className="form-control"
              required
            />
          </div>
        </div>

        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">Teléfono</label>
            <input
              name="telefono"
              value={form.telefono}
              onChange={handleChange}
              className="form-control"
            />
          </div>
          <div className="col-md-6">
            <label className="form-label">Rol</label>
            <input
              name="rol"
              value={form.rol}
              onChange={handleChange}
              className="form-control"
              placeholder="Ej: Profesor, Recepción, Admin"
            />
          </div>
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
