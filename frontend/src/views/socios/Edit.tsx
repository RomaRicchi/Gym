import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export default function SociosEdit() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [form, setForm] = useState({
    dni: "",
    nombre: "",
    email: "",
    telefono: "",
    activo: true,
  });

  useEffect(() => {
    gymApi
      .get(`/socios/${id}`)
      .then((res) => setForm(res.data))
      .catch(() => Swal.fire("Error", "No se pudo cargar el socio", "error"));
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === "checkbox" ? checked : value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await gymApi.put(`/socios/${id}`, form);
      Swal.fire("Guardado", "Los datos fueron actualizados", "success");
      navigate("/socios");
    } catch {
      Swal.fire("Error", "No se pudo actualizar el socio", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>✏️ Editar Socio</h2>

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

        <button type="submit" className="btn btn-primary">
          Guardar cambios
        </button>
      </form>
    </div>
  );
}
