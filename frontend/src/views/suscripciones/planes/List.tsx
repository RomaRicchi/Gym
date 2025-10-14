import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Plan {
  id: number;
  nombre: string;
  dias_por_semana: number;
  precio: number;
  activo: number;
}

export default function PlanesList() {
  const [planes, setPlanes] = useState<Plan[]>([]);
  const navigate = useNavigate();

  const fetchPlanes = async () => {
    try {
      const res = await gymApi.get("/planes");
      setPlanes(res.data.items || res.data);
    } catch {
      Swal.fire("Error", "No se pudieron cargar los planes", "error");
    }
  };

  useEffect(() => {
    fetchPlanes();
  }, []);

  const handleDelete = async (id: number) => {
    const confirm = await Swal.fire({
      title: "¿Desactivar plan?",
      text: "Podrás volver a activarlo más tarde.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, desactivar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (confirm.isConfirmed) {
      try {
        await gymApi.delete(`/planes/${id}`);
        Swal.fire("Actualizado", "Plan desactivado correctamente", "success");
        fetchPlanes();
      } catch {
        Swal.fire("Error", "No se pudo actualizar el plan", "error");
      }
    }
  };

  return (
    <div className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2>📋 Planes de Membresía</h2>
        <button className="btn btn-success" onClick={() => navigate("/planes/nuevo")}>
          ➕ Nuevo Plan
        </button>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Días por semana</th>
            <th>Precio</th>
            <th>Activo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {planes.map((p) => (
            <tr key={p.id}>
              <td>{p.id}</td>
              <td>{p.nombre}</td>
              <td>{p.dias_por_semana}</td>
              <td>${p.precio.toFixed(2)}</td>
              <td>{p.activo ? "✅" : "❌"}</td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => navigate(`/planes/editar/${p.id}`)}
                >
                  ✏️ Editar
                </button>
                <button
                  className="btn btn-sm btn-outline-danger"
                  onClick={() => handleDelete(p.id)}
                >
                  🔒 Desactivar
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
