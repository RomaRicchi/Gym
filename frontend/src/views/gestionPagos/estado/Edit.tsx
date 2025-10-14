import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export default function EstadoEdit() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [form, setForm] = useState({
    nombre: "",
    descripcion: "",
  });

  useEffect(() => {
    gymApi.get(`/estados-orden-pago/${id}`).then((res) => {
      const e = res.data;
      setForm({
        nombre: e.nombre || "",
        descripcion: e.descripcion || "",
      });
    });
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      await gymApi.put(`/estados-orden-pago/${id}`, form);
      Swal.fire("Actualizado", "Estado modificado correctamente", "success");
      navigate("/estados");
    } catch {
      Swal.fire("Error", "No se pudo actualizar el estado", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>✏️ Editar Estado</h2>
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
          <label className="form-label">Descripción</label>
          <textarea
            name="descripcion"
            value={form.descripcion}
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
