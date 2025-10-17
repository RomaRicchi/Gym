import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { SalaCreateSwal } from "@/views/salas/SalaCreateSwal"; 
import { SalaEditSwal } from "@/views/salas/SalaEditSwal";   

interface Sala {
  id: number;
  nombre: string;
  capacidad: number;
  activa: boolean | number;
}

export default function SalasList() {
  const [salas, setSalas] = useState<Sala[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchSalas = async () => {
    try {
      const res = await gymApi.get("/salas");
      const data = res.data.items || res.data;
      const parsed = data.map((s: any) => ({ ...s, activa: Boolean(s.activa) }));
      setSalas(parsed);
    } catch {
      Swal.fire("Error", "No se pudieron cargar las salas", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSalas();
  }, []);

  const handleDelete = async (id: number) => {
    const confirm = await Swal.fire({
      title: "¿Eliminar sala?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (confirm.isConfirmed) {
      try {
        await gymApi.delete(`/salas/${id}`);
        Swal.fire("Eliminada", "Sala eliminada correctamente", "success");
        fetchSalas();
      } catch {
        Swal.fire("Error", "No se pudo eliminar la sala", "error");
      }
    }
  };

  if (loading) return <p>Cargando salas...</p>;

  return (
    <div className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2>Salas</h2>
        {/* 🟢 Botón crear usando el Swal importado */}
        <button onClick={() => SalaCreateSwal(fetchSalas)} className="btn btn-success">
          ➕ Nueva Sala
        </button>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Capacidad</th>
            <th>Activa</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {salas.map((s) => (
            <tr key={s.id}>
              <td>{s.id}</td>
              <td>{s.nombre}</td>
              <td>{s.capacidad}</td>
              <td>{s.activa ? "✅ Sí" : "❌ No"}</td>
              <td>
                {/* ✏️ Editar usando el Swal importado */}
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => SalaEditSwal(s.id.toString(), fetchSalas)}
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
