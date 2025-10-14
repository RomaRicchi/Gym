import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export default function RolEdit() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [nombre, setNombre] = useState("");

  useEffect(() => {
    const fetchRol = async () => {
      try {
        const res = await gymApi.get(`/roles/${id}`);
        setNombre(res.data.nombre);
      } catch {
        Swal.fire("Error", "No se pudo cargar el rol", "error");
      }
    };
    fetchRol();
  }, [id]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!nombre.trim()) {
      Swal.fire("Error", "Debe ingresar un nombre", "error");
      return;
    }

    try {
      await gymApi.put(`/roles/${id}`, { nombre });
      Swal.fire("Actualizado", "Rol modificado correctamente", "success");
      navigate("/roles");
    } catch {
      Swal.fire("Error", "No se pudo actualizar el rol", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>✏️ Editar Rol</h2>
      <form onSubmit={handleSubmit} className="mt-4">
        <div className="mb-3">
          <label className="form-label">Nombre</label>
          <input
            type="text"
            value={nombre}
            onChange={(e) => setNombre(e.target.value)}
            className="form-control"
            required
          />
        </div>

        <button type="submit" className="btn btn-primary">
          Guardar Cambios
        </button>
      </form>
    </div>
  );
}
