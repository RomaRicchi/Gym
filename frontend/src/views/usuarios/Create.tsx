import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Rol { id: number; nombre: string; }

export default function UsuarioCreate() {
  const navigate = useNavigate();
  const [roles, setRoles] = useState<Rol[]>([]);
  const [form, setForm] = useState({
    email: "",
    alias: "",
    rol_id: "",
    password: "",
    estado: true,
  });

  useEffect(() => {
    const fetchRoles = async () => {
      try {
        const res = await gymApi.get("/roles");
        setRoles(res.data.items || res.data);
      } catch {
        Swal.fire("Error", "No se pudieron cargar los roles", "error");
      }
    };
    fetchRoles();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const target = e.target as HTMLInputElement;
    const { name, value, type } = target;
    setForm({ ...form, [name]: type === "checkbox" ? target.checked : value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!form.email || !form.password) {
      Swal.fire("Error", "Debe ingresar email y contraseña", "error");
      return;
    }

    try {
      await gymApi.post("/usuarios", form);
      Swal.fire("Guardado", "Usuario creado correctamente", "success");
      navigate("/usuarios");
    } catch {
      Swal.fire("Error", "No se pudo crear el usuario", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>➕ Nuevo Usuario</h2>

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

          <div className="col-md-6">
            <label className="form-label">Contraseña</label>
            <input
              type="password"
              name="password"
              value={form.password}
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
            onChange={handleChange}
            className="form-check-input"
            id="estado"
          />
          <label className="form-check-label" htmlFor="estado">
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
