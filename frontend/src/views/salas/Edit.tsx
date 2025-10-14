import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export default function SalaEdit() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [form, setForm] = useState({
    nombre: "",
    capacidad: "",
    activa: false,
  });

  useEffect(() => {
    gymApi.get(`/salas/${id}`).then((res) => {
      const data = res.data;
      setForm({
        nombre: data.nombre || "",
        capacidad: data.capacidad?.toString() || "",
        activa: !!data.activa, // ✅ tinyint → boolean
      });
    });
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const target = e.target as HTMLInputElement;
    const { name, value, type } = target;
    setForm({ ...form, [name]: type === "checkbox" ? target.checked : value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      await gymApi.put(`/salas/${id}`, {
        ...form,
        activa: form.activa ? 1 : 0,
      });
      Swal.fire("Actualizada", "Sala modificada correctamente", "success");
      navigate("/salas");
    } catch {
      Swal.fire("Error", "No se pudo actualizar la sala", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h2>✏️ Editar Sala</h2>
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
          <label className="form-label">Capacidad</label>
          <input
            type="number"
            name="capacidad"
            value={form.capacidad}
            onChange={handleChange}
            className="form-control"
            min={1}
            required
          />
        </div>

        <div className="form-check mb-3">
          <input
            type="checkbox"
            name="activa"
            checked={form.activa}
            onChange={handleChange}
            className="form-check-input"
            id="activa"
          />
          <label className="form-check-label" htmlFor="activa">
            Activa
          </label>
        </div>

        <button type="submit" className="btn btn-primary">
          Guardar cambios
        </button>
      </form>
    </div>
  );
}
