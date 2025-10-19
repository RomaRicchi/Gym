import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { mostrarFormEditarSuscripcion } from "@/views/suscripciones/SuscripcionEdit";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Suscripcion {
  id: number;
  socio: string;
  plan: string;
  inicio: string;
  fin: string;
  estado: boolean | number;
  creado_en: string;
  orden_pago_id?: number;
}

export default function SuscripcionesList() {
  const [suscripciones, setSuscripciones] = useState<Suscripcion[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  const fetchSuscripciones = async () => {
    try {
      const res = await gymApi.get("/suscripciones");
      const data = res.data.items || res.data;

      // 🔁 Convertir tinyint(1) → boolean (por si llega 0/1)
      const parsed = data.map((s: any) => ({
        ...s,
        estado: Boolean(s.estado),
      }));
      setSuscripciones(parsed);
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudieron cargar las suscripciones", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSuscripciones();
  }, []);

  const handleDelete = async (id: number) => {
    const result = await Swal.fire({
      title: "¿Eliminar suscripción?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/suscripciones/${id}`);
        Swal.fire("Eliminada", "Suscripción eliminada correctamente", "success");
        fetchSuscripciones();
      } catch {
        Swal.fire("Error", "No se pudo eliminar la suscripción", "error");
      }
    }
  };

  if (loading) return <p>Cargando suscripciones...</p>;

  return (
    <div className="mt-4">
        <h1
          className="text-center fw-bold mb-4"
          style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
        >
          SUSCRIPCIONES
        </h1>
     

      <table className="table table-striped table-hover align-middle text-center">
        <thead className="table-dark">
          <tr>
            <th>Socio</th>
            <th>Plan</th>
            <th>Inicio</th>
            <th>Fin</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {suscripciones.map((s) => (
            <tr key={s.id}>
              <td>{s.socio}</td>
              <td>{s.plan}</td>
              <td>{new Date(s.inicio).toLocaleDateString()}</td>
              <td>{new Date(s.fin).toLocaleDateString()}</td>
              <td>
                {s.estado ? (
                  <span className="text-success fw-bold">✅</span>
                ) : (
                  <span className="text-danger fw-bold">❌</span>
                )}
              </td>
              <td>
                <button
                  className="btn btn-sm btn-warning"
                  onClick={async () => {
                    const ok = await mostrarFormEditarSuscripcion(s.id);
                    if (ok) fetchSuscripciones(); 
                  }}
                >
                  ✏️ 
                </button>
                <button
                  className="btn btn-sm btn-danger"
                  onClick={() => handleDelete(s.id)}
                >
                  🗑️ 
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
