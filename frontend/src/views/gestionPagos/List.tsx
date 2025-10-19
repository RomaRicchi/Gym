import { useEffect, useState } from "react";
import { Button } from "react-bootstrap";
import Swal from "sweetalert2";
import { Link } from "react-router-dom";
import gymApi from "@/api/gymApi"

export default function OrdenesList() {
  const [ordenes, setOrdenes] = useState<any[]>([]);
  const [estados, setEstados] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  // 🔹 Cargar estados y órdenes
  useEffect(() => {
    const fetchData = async () => {
      try {
        const [resEstados, resOrdenes] = await Promise.all([
          gymApi.get("/estados"),
          gymApi.get("/ordenes")
        ]);
        setEstados(resEstados.data);
        setOrdenes(resOrdenes.data);
      } catch (err) {
        console.error(err);
        Swal.fire("Error", "No se pudieron cargar las órdenes de pago", "error");
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, []);

  if (loading)
    return (
      <div className="text-center mt-5">
        <div className="spinner-border text-primary" role="status"></div>
        <p className="mt-3">Cargando órdenes...</p>
      </div>
    );

  return (
    <div className="container mt-3">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        ORDENES DE PAGO
      </h1>

      <div className="table-responsive">
        <table className="table table-striped table-bordered align-middle text-center">
          <thead className="table-dark">
            <tr>
              <th>Socio</th>
              <th>Plan</th>
              <th>Monto</th>
              <th>Vence</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {ordenes.length === 0 ? (
              <tr>
                <td colSpan={7}>No hay órdenes registradas</td>
              </tr>
            ) : (
              ordenes.map((o) => (
                <tr key={o.id}>
                  <td>{o.socio?.nombre || "—"}</td>
                  <td>{o.plan?.nombre || "—"}</td>
                  <td>${o.monto.toFixed(2)}</td>
                  <td>{new Date(o.venceEn).toLocaleDateString()}</td>
                  <td>
                    <span
                      className={`badge bg-${
                        o.estado?.nombre === "verificado"
                          ? "success"
                          : o.estado?.nombre === "pendiente"
                          ? "warning"
                          : o.estado?.nombre === "rechazado"
                          ? "danger"
                          : "secondary"
                      }`}
                    >
                      {o.estado?.nombre || "Sin estado"}
                    </span>
                  </td>
                  <td>
                    <div className="d-flex justify-content-center gap-2">
                      <Link
                        to={`/ordenes/${o.id}/comprobantes`}
                        className="btn btn-sm btn-outline-primary"
                      >
                        📎 Comprobantes
                      </Link>

                      <Link
                        to={`/ordenes/editar/${o.id}`}
                        className="btn btn-sm btn-outline-warning"
                      >
                        ✏️ Editar
                      </Link>

                      <Button
                        size="sm"
                        variant="outline-danger"
                        onClick={() => eliminarOrden(o.id)}
                      >
                        🗑️ Eliminar
                      </Button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );

  async function eliminarOrden(id: number) {
    const result = await Swal.fire({
      title: "¿Eliminar orden?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
    });

    if (!result.isConfirmed) return;

    try {
      await gymApi.delete(`/ordenes/${id}`);
      setOrdenes((prev) => prev.filter((x) => x.id !== id));
      Swal.fire("Eliminada", "La orden fue eliminada correctamente.", "success");
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudo eliminar la orden.", "error");
    }
  }
}
