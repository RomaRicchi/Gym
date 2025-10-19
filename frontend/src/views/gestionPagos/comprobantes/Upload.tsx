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
      Swal.fire("⚠️ Error", "Debe seleccionar un archivo", "warning");
      return;
    }

    const formData = new FormData();
    formData.append("file", file); // ✅ mismo nombre que el backend
    formData.append("ordenPagoId", id || "");

    try {
      const { data } = await gymApi.post("/comprobantes", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      await Swal.fire({
        icon: "success",
        title: "✅ Comprobante subido",
        html: `
          <p>El comprobante fue cargado correctamente.</p>
          <a href="${import.meta.env.VITE_API_URL}/${data.fileUrl}" target="_blank" style="color:#ff6b00;font-weight:bold;">
            Ver archivo 📄
          </a>
        `,
        confirmButtonText: "Aceptar",
        confirmButtonColor: "#ff6b00",
      });

      navigate(`/ordenes/${id}/comprobantes`);
    } catch (err: any) {
      console.error(err);
      Swal.fire("❌ Error", err.response?.data || "No se pudo subir el comprobante", "error");
    }
  };

  return (
    <div
      className="d-flex align-items-center justify-content-center vh-100"
      style={{ backgroundColor: "#f8f9fa" }}
    >
      <div
        className="card shadow-lg p-4 text-center"
        style={{
          width: "100%",
          maxWidth: 500,
          borderRadius: "1rem",
          border: "none",
        }}
      >
        <h3 className="fw-bold text-orange mb-3">📤 Subir Comprobante</h3>
        <p className="text-muted">Orden #{id}</p>

        <form onSubmit={handleSubmit}>
          <div className="mb-3 text-start">
            <label className="form-label fw-semibold">Archivo</label>
            <input
              type="file"
              className="form-control"
              onChange={handleFileChange}
              accept=".pdf,.jpg,.jpeg,.png"
            />
          </div>

          <button
            type="submit"
            className="btn w-100 fw-bold"
            style={{ backgroundColor: "#ff6b00", color: "white" }}
          >
            Subir Archivo
          </button>

          <button
            type="button"
            className="btn btn-outline-secondary w-100 mt-2"
            onClick={() => navigate(`/ordenes/${id}/comprobantes`)}
          >
            Volver
          </button>
        </form>
      </div>
    </div>
  );
}
