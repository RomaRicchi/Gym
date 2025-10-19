// @ts-nocheck
import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Comprobante {
  id: number;
  fileUrl: string;
  mimeType: string;
  subidoEn: string;
}

export default function OrdenComprobantesList() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [comprobantes, setComprobantes] = useState<Comprobante[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchComprobantes = async () => {
    try {
      const res = await gymApi.get(`/comprobantes/orden/${id}`);
      setComprobantes(res.data.items || res.data);
    } catch (err) {
      console.error(err);
      Swal.fire("❌ Error", "No se pudieron cargar los comprobantes.", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchComprobantes();
  }, [id]);

  // 🗑️ Eliminar comprobante
  const handleDelete = async (comprobanteId: number) => {
    const confirm = await Swal.fire({
      title: "¿Eliminar comprobante?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (!confirm.isConfirmed) return;

    try {
      await gymApi.delete(`/comprobantes/${comprobanteId}`);
      setComprobantes((prev) => prev.filter((c) => c.id !== comprobanteId));
      Swal.fire("✅ Eliminado", "El comprobante fue eliminado correctamente.", "success");
    } catch (err) {
      console.error(err);
      Swal.fire("❌ Error", "No se pudo eliminar el comprobante.", "error");
    }
  };

  if (loading)
    return (
      <div className="text-center mt-5">
        <div className="spinner-border text-warning" role="status"></div>
        <p className="mt-3 text-muted">Cargando comprobantes...</p>
      </div>
    );

  return (
    <div className="container mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.3rem", letterSpacing: "2px" }}
      >
        📄 COMPROBANTES DE LA ORDEN #{id}
      </h1>

      <div className="d-flex justify-content-between align-items-center mb-3">
        <button
          className="btn btn-outline-secondary fw-bold"
          onClick={() => navigate("/ordenes")}
        >
          ⬅️ Volver a Órdenes
        </button>
      </div>

      {comprobantes.length === 0 ? (
        <div className="alert alert-warning text-center">
          No hay comprobantes cargados para esta orden.
        </div>
      ) : (
        <div className="table-responsive">
          <table className="table table-striped align-middle text-center">
            <thead className="table-dark">
              <tr>
                <th>ID</th>
                <th>Archivo</th>
                <th>Tipo</th>
                <th>Subido</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {comprobantes.map((c) => (
                <tr key={c.id}>
                  <td>{c.id}</td>
                  <td>
                    {c.fileUrl?.endsWith(".pdf") ? (
                      <span>📕 PDF</span>
                    ) : (
                      <img
                        src={`${import.meta.env.VITE_API_URL}/${c.fileUrl}`}
                        alt="Comprobante"
                        style={{
                          width: "70px",
                          height: "70px",
                          objectFit: "cover",
                          borderRadius: "8px",
                        }}
                      />
                    )}
                  </td>
                  <td>{c.mimeType}</td>
                  <td>{new Date(c.subidoEn).toLocaleString()}</td>
                  <td>
                    <div className="d-flex justify-content-center gap-2">
                      <a
                        href={`${import.meta.env.VITE_API_URL}/${c.fileUrl}`}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="btn btn-sm btn-primary fw-bold"
                      >
                        👁️ Ver
                      </a>
                      <button
                        className="btn btn-sm btn-danger fw-bold"
                        onClick={() => handleDelete(c.id)}
                      >
                        🗑️
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
