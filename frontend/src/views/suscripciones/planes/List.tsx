import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { PlanCreateSwal } from "@/views/suscripciones/planes/PlanCreateSwal";
import { PlanEditSwal } from "@/views/suscripciones/planes/PlanEditSwal";

interface Plan {
  id: number;
  nombre: string;
  dias_por_semana: number;
  precio: number;
  activo: boolean;
}

export default function PlanesList() {
  const [planes, setPlanes] = useState<Plan[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchPlanes = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get("/planes");
      setPlanes(res.data.items || res.data);
    } catch {
      Swal.fire("Error", "No se pudieron cargar los planes", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPlanes();
  }, []);

  const handleDelete = async (id: number) => {
    const confirm = await Swal.fire({
      title: "¿Eliminar plan?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (confirm.isConfirmed) {
      try {
        await gymApi.delete(`/planes/${id}`);
        Swal.fire("Eliminado", "Plan eliminado correctamente", "success");
        fetchPlanes();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el plan", "error");
      }
    }
  };

  if (loading) return <p>Cargando planes...</p>;

  return (
    <div className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2>Planes</h2>
        <button onClick={() => PlanCreateSwal(fetchPlanes)} className="btn btn-success">
          ➕ Nuevo Plan
        </button>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>Nombre</th>
            <th>Días/semana</th>
            <th>Precio</th>
            <th>Activo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {planes.map((p) => (
            <tr key={p.id}>
              <td>{p.nombre}</td>
              <td>{p.dias_por_semana}</td>
              <td>${p.precio}</td>
              <td>{p.activo ? "✅" : "❌"}</td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => PlanEditSwal(p.id.toString(), fetchPlanes)}
                >
                  ✏️ Editar
                </button>
                <button
                  className="btn btn-sm btn-outline-danger"
                  onClick={() => handleDelete(p.id)}
                >
                  🗑️ Eliminar
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
