import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Comprobante {
  id: number;
  file_url: string;
  mime_type: string;
  subido_en: string;
}

export default function ComprobantesList() {
  const { id } = useParams(); // ID de la orden de pago
  const [files, setFiles] = useState<Comprobante[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchFiles = async () => {
    try {
      const res = await gymApi.get(`/ordenes/${id}/comprobantes`);
      setFiles(res.data.items || res.data);
    } catch {
      Swal.fire("Error", "No se pudieron cargar los comprobantes", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchFiles();
  }, [id]);

  const handleDelete = async (compId: number) => {
    const confirm = await Swal.fire({
      title: "¿Eliminar comprobante?",
      text: "Este archivo será eliminado permanentemente.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (confirm.isConfirmed) {
      try {
        await gymApi.delete(`/comprobantes/${compId}`);
        Swal.fire("Eliminado", "Comprobante eliminado correctamente", "success");
        fetchFiles();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el comprobante", "error");
      }
    }
  };

  if (loading) return <p>Cargando comprobantes...</p>;

  return (
    <div className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2>📑 Comprobantes de la Orden #{id}</h2>
        <Link to={`/ordenes/${id}/subir-comprobante`} className="btn btn-success">
          ➕ Subir Comprobante
        </Link>
      </div>

      {files.length === 0 ? (
        <p>No hay comprobantes asociados a esta orden.</p>
      ) : (
        <table className="table table-hover">
          <thead className="table-dark">
            <tr>
              <th>ID</th>
              <th>Archivo</th>
              <th>Tipo</th>
              <th>Fecha</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {files.map((f) => (
              <tr key={f.id}>
                <td>{f.id}</td>
                <td>
                  <a href={f.file_url} target="_blank" rel="noopener noreferrer">
                    Ver archivo
                  </a>
                </td>
                <td>{f.mime_type}</td>
                <td>{new Date(f.subido_en).toLocaleString()}</td>
                <td>
                  <button
                    className="btn btn-sm btn-outline-danger"
                    onClick={() => handleDelete(f.id)}
                  >
                    🗑️ Eliminar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
