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

export default function ComprobantesList() {
  const { id } = useParams(); // id de la orden
  const navigate = useNavigate();
  const [comprobantes, setComprobantes] = useState<Comprobante[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchComprobantes = async () => {
    try {
      const res = await gymApi.get(`/ordenes/${id}/comprobantes`);
      console.log("Comprobantes:", res.data);
      setComprobantes(Array.isArray(res.data) ? res.data : []);
    } catch {
      Swal.fire("Error", "No se pudieron cargar los comprobantes", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchComprobantes();
  }, [id]);

  const handleDelete = async (cid: number) => {
    const confirm = await Swal.fire({
      title: "¿Eliminar comprobante?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
    });

    if (confirm.isConfirmed) {
      try {
        await gymApi.delete(`/comprobantes/${cid}`);
        Swal.fire("Eliminado", "Comprobante eliminado correctamente", "success");
        fetchComprobantes();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el comprobante", "error");
      }
    }
  };

  if (loading) return <p>Cargando comprobantes...</p>;

  return (
    <div className="container mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        COMPROBANTES
      </h1>

      <button
        onClick={() => navigate(`/ordenes/${id}/subir-comprobante`)}
        className="btn btn-success mb-3"
      >
        ➕ Subir nuevo comprobante
      </button>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>Archivo</th>
            <th>Tipo</th>
            <th>Subido En</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {comprobantes.length > 0 ? (
            comprobantes.map((f) => (
              <tr key={f.id}>
                <td>
                  <a href={`/${f.fileUrl}`} target="_blank" rel="noopener noreferrer">
                    Ver archivo
                  </a>
                </td>
                <td>{f.mimeType}</td>
                <td>{new Date(f.subidoEn).toLocaleString()}</td>
                <td>
                  <button
                    className="btn btn-sm btn-outline-danger"
                    onClick={() => handleDelete(f.id)}
                  >
                    🗑️ Eliminar
                  </button>
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan={5} className="text-center text-muted">
                No hay comprobantes cargados
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}
