import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export default function ComprobanteUpload() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [file, setFile] = useState<File | null>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!file) {
      Swal.fire("Error", "Debe seleccionar un archivo", "error");
      return;
    }

    const formData = new FormData();
    formData.append("archivo", file);        // ✅ debe coincidir con [FromForm] IFormFile archivo
    formData.append("ordenPagoId", id || ""); // ✅ debe coincidir con [FromForm] int ordenPagoId

    try {
      await gymApi.post("/comprobantes", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      Swal.fire("Subido", "Comprobante cargado correctamente", "success");
      navigate(`/ordenes/${id}/comprobantes`);
    } catch {
      Swal.fire("Error", "No se pudo subir el comprobante", "error");
    }
  };

  return (
    <div className="container mt-5">
      <h2>📤 Subir Comprobante - Orden #{id}</h2>
      <form onSubmit={handleSubmit} className="mt-4">
        <div className="mb-3">
          <input type="file" className="form-control" onChange={handleFileChange} accept=".pdf,.jpg,.jpeg,.png" />
        </div>

        <button type="submit" className="btn btn-primary">
          Subir archivo
        </button>

        <button
          type="button"
          className="btn btn-secondary ms-3"
          onClick={() => navigate(`/ordenes/${id}/comprobantes`)}
        >
          Volver
        </button>
      </form>
    </div>
  );
}
