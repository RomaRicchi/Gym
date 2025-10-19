import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Estado {
  id: number;
  nombre: string;
  descripcion: string;
}

export default function EstadosList() {
  const [estados, setEstados] = useState<Estado[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  const fetchEstados = async () => {
    try {
      const res = await gymApi.get("/estados"); // ✅ corregido
      setEstados(res.data.items || res.data);
    } catch {
      Swal.fire("Error", "No se pudieron cargar los estados", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchEstados();
  }, []);

  const handleDelete = async (id: number) => {
    const confirm = await Swal.fire({
      title: "¿Eliminar estado?",
      text: "Esto puede afectar órdenes existentes.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (confirm.isConfirmed) {
      try {
        await gymApi.delete(`/estados/${id}`); // ✅ corregido
        Swal.fire("Eliminado", "Estado eliminado correctamente", "success");
        fetchEstados();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el estado", "error");
      }
    }
  };

  if (loading) return <p>Cargando estados...</p>;

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        ESTADO DEL PAGO
      </h1>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <button onClick={() => navigate("/estados/nuevo")} className="btn btn-success">
          ➕ Nuevo Estado
        </button>
      </div>

      <table className="table table-striped table-hover">
        <thead className="table-dark">
          <tr>
            <th>Nombre</th>
            <th>Descripción</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {estados.map((e) => (
            <tr key={e.id}>
              <td>{e.nombre}</td>
              <td>{e.descripcion || "—"}</td>
              <td>
                <button
                  className="btn btn-sm btn-outline-primary me-2"
                  onClick={() => navigate(`/estados/editar/${e.id}`)}
                >
                  ✏️ Editar
                </button>
                <button
                  className="btn btn-sm btn-outline-danger"
                  onClick={() => handleDelete(e.id)}
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
