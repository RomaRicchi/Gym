import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export default function RolCreate() {
  const navigate = useNavigate();
  const [nombre, setNombre] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!nombre.trim()) {
      Swal.fire("Error", "Debe ingresar un nombre", "error");
      return;
    }

    try {
      await gymApi.post("/roles", { nombre });
      Swal.fire("Guardado", "Rol creado correctamente", "success");
      navigate("/roles");
    } catch {
      Swal.fire("Error", "No se pudo crear el rol", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>➕ Nuevo Rol</h2>
      <form onSubmit={handleSubmit} className="mt-4">
        <div className="mb-3">
          <label className="form-label">Nombre</label>
          <input
            type="text"
            value={nombre}
            onChange={(e) => setNombre(e.target.value)}
            className="form-control"
            placeholder="Ej: Administrador, Profesor..."
            required
          />
        </div>

        <button type="submit" className="btn btn-primary">
          Guardar
        </button>
      </form>
    </div>
  );
}
