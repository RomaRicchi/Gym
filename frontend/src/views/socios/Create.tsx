import { useState } from "react";
import { useNavigate } from "react-router-dom";
import gymApi from "@/api/gymApi";

export default function SociosCreate() {
  const navigate = useNavigate();

  const [form, setForm] = useState({
    dni: "",
    nombre: "",
    email: "",
    telefono: "",
    activo: true,
  });

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  // 🧩 Maneja los cambios de inputs
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === "checkbox" ? checked : value });
  };

  // 🧾 Enviar el formulario
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(false);

    try {
      await gymApi.post("/socios", form);
      setSuccess(true);
      setTimeout(() => navigate("/socios"), 1200); // redirige luego de 1.2s
    } catch (err: any) {
      console.error(err);
      setError("Error al crear el socio. Verifique los datos.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mt-4">
      <h2>🧍 Alta de nuevo socio</h2>
      <p className="text-muted">Complete los datos del socio y guarde los cambios.</p>

      <form onSubmit={handleSubmit} className="mt-4">
        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">DNI</label>
            <input
              type="text"
              name="dni"
              value={form.dni}
              onChange={handleChange}
              className="form-control"
              required
            />
          </div>
          <div className="col-md-6">
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
        </div>

        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">Email</label>
            <input
              type="email"
              name="email"
              value={form.email}
              onChange={handleChange}
              className="form-control"
              required
            />
          </div>
          <div className="col-md-6">
            <label className="form-label">Teléfono</label>
            <input
              type="text"
              name="telefono"
              value={form.telefono}
              onChange={handleChange}
              className="form-control"
              required
            />
          </div>
        </div>

        <div className="form-check mb-3">
          <input
            type="checkbox"
            className="form-check-input"
            id="activo"
            name="activo"
            checked={form.activo}
            onChange={handleChange}
          />
          <label className="form-check-label" htmlFor="activo">
            Activo
          </label>
        </div>

        {error && <div className="alert alert-danger">{error}</div>}
        {success && <div className="alert alert-success">✅ Socio creado correctamente</div>}

        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? "Guardando..." : "Guardar socio"}
        </button>
      </form>
    </div>
  );
}
