import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface OrdenPago {
  id: number;
  socio_id: number;
  plan_id: number;
  suscripcion_id: number;
  monto: number;
  vence_en: string;
  estado_id: number;
  notas?: string;
}

export default function OrdenesList() {
  const [ordenes, setOrdenes] = useState<OrdenPago[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  const fetchOrdenes = async () => {
    try {
      const res = await gymApi.get("/ordenes-pago");
      setOrdenes(res.data.items || res.data);
    } catch {
      Swal.fire("Error", "No se pudieron cargar las órdenes de pago", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrdenes();
  }, []);

  const handleDelete = async (id: number) => {
    const confirm = await Swal.fire({
      title: "¿Eliminar orden?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (confirm.isConfirmed) {
      try {
        await gymApi.delete(`/ordenes-pago/${id}`);
        Swal.fire("Eliminada", "Orden eliminada correctamente", "success");
        fetchOrdenes();
      } catch {
        Swal.fire("Error", "No se pudo eliminar la orden", "error");
      }
    }
  };

  if (loading) return <p>Cargando órdenes...</p>;

  return (
    <div className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2>Órdenes de Pago</h2>
        <button onClick={() => navigate("/ordenes/nueva")} className="btn btn-success">
          ➕ Nueva Orden
        </button>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>ID</th>
            <th>Socio</th>
            <th>Plan</th>
            <th>Monto</th>
            <th>Vence</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {ordenes.map((o) => (
            <tr key={o.id}>
              <td>{o.id}</td>
              <td>{o.socio_id}</td>
              <td>{o.plan_id}</td>
              <td>${o.monto.toFixed(2)}</td>
              <td>{new Date(o.vence_en).toLocaleDateString()}</td>
              <td>
                {o.estado_id === 1 && <span className="text-warning">Pendiente</span>}
                {o.estado_id === 2 && <span className="text-success">Pagada</span>}
                {o.estado_id === 3 && <span className="text-danger">Vencida</span>}
              </td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => navigate(`/ordenes/editar/${o.id}`)}
                >
                  ✏️ Editar
                </button>
                <button
                  className="btn btn-sm btn-outline-danger"
                  onClick={() => handleDelete(o.id)}
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
