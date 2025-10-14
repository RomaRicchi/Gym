import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Rol { id: number; nombre: string; }

export default function UsuarioEdit() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [roles, setRoles] = useState<Rol[]>([]);
  const [form, setForm] = useState({
    email: "",
    alias: "",
    rol_id: "",
    estado: true,
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [resUsuario, resRoles] = await Promise.all([
          gymApi.get(`/usuarios/${id}`),
          gymApi.get("/roles"),
        ]);

        setForm({
          email: resUsuario.data.email,
          alias: resUsuario.data.alias,
          rol_id: resUsuario.data.rol_id.toString(),
          estado: !!resUsuario.data.estado,
        });
        setRoles(resRoles.data.items || resRoles.data);
      } catch {
        Swal.fire("Error", "No se pudieron cargar los datos", "error");
      }
    };
    fetchData();
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const target = e.target as HTMLInputElement;
    const { name, value, type } = target;
    setForm({ ...form, [name]: type === "checkbox" ? target.checked : value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await gymApi.put(`/usuarios/${id}`, form);
      Swal.fire("Actualizado", "Usuario modificado correctamente", "success");
      navigate("/usuarios");
    } catch {
      Swal.fire("Error", "No se pudo actualizar el usuario", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>✏️ Editar Usuario</h2>

      <form onSubmit={handleSubmit} className="mt-4">
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
            <label className="form-label">Alias</label>
            <input
              type="text"
              name="alias"
              value={form.alias}
              onChange={handleChange}
              className="form-control"
              required
            />
          </div>
        </div>

        <div className="row mb-3">
          <div className="col-md-6">
            <label className="form-label">Rol</label>
            <select
              name="rol_id"
              value={form.rol_id}
              onChange={handleChange}
              className="form-select"
              required
            >
              <option value="">Seleccionar rol...</option>
              {roles.map((r) => (
                <option key={r.id} value={r.id}>
                  {r.nombre}
                </option>
              ))}
            </select>
          </div>

          <div className="col-md-6 d-flex align-items-center">
            <div className="form-check mt-4">
              <input
                type="checkbox"
                name="estado"
                checked={form.estado}
                onChange={handleChange}
                className="form-check-input"
                id="estado"
              />
              <label className="form-check-label" htmlFor="estado">
                Activo
              </label>
            </div>
          </div>
        </div>

        <button type="submit" className="btn btn-primary">
          Guardar Cambios
        </button>
      </form>
    </div>
  );
}
