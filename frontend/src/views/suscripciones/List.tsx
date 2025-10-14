import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Suscripcion {
  id: number;
  socio_id: number;
  plan_id: number;
  inicio: string;
  fin: string;
  estado: boolean | number;
  creado_en: string;
}

export default function SuscripcionesList() {
  const [suscripciones, setSuscripciones] = useState<Suscripcion[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  const fetchSuscripciones = async () => {
    try {
      const res = await gymApi.get("/suscripciones");
      const data = res.data.items || res.data;

      // 🔁 Convertir tinyint(1) → boolean para mostrar correctamente
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
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2>Suscripciones</h2>
        <Link to="/suscripciones/nueva" className="btn btn-success">
          ➕ Nueva Suscripción
        </Link>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>ID</th>
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
              <td>{s.id}</td>
              <td>{s.socio_id}</td>
              <td>{s.plan_id}</td>
              <td>{new Date(s.inicio).toLocaleDateString()}</td>
              <td>{new Date(s.fin).toLocaleDateString()}</td>
              <td>{s.estado ? "✅ Activa" : "❌ Inactiva"}</td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => navigate(`/suscripciones/editar/${s.id}`)}
                >
                  ✏️ Editar
                </button>
                <button
                  className="btn btn-sm btn-outline-danger"
                  onClick={() => handleDelete(s.id)}
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
