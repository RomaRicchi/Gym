import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export default function ComprobanteUpload() {
  const { id } = useParams(); // orden_pago_id
  const navigate = useNavigate();
  const [file, setFile] = useState<File | null>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = e.target.files?.[0] || null;
    setFile(selectedFile);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!file) {
      Swal.fire("Aviso", "Debe seleccionar un archivo", "info");
      return;
    }

    const formData = new FormData();
    formData.append("file", file);
    formData.append("orden_pago_id", id || "");

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
    <div className="container mt-4">
      <h2>📤 Subir Comprobante</h2>
      <p className="text-muted">Seleccioná un archivo (PDF, JPG, PNG, etc.)</p>

      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <input
            type="file"
            accept=".pdf,.jpg,.jpeg,.png"
            className="form-control"
            onChange={handleFileChange}
          />
        </div>

        <button type="submit" className="btn btn-primary">
          Subir Archivo
        </button>
      </form>
    </div>
  );
}
