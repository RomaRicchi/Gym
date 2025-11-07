import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import Pagination from "@/components/Pagination";
import { EjercicioCreateSwal } from "@/views/rutinas/ejercicios/EjercicioCreateSwal";
import { EjercicioEditSwal } from "@/views/rutinas/ejercicios/EjercicioEditSwal";

interface Ejercicio {
  id: number;
  nombre: string;
  tips?: string;
  mediaUrl?: string;
  grupoMuscularId: number;
  grupoMuscularNombre?: string;
}

export default function EjerciciosList() {
  const [ejercicios, setEjercicios] = useState<Ejercicio[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalItems, setTotalItems] = useState(0);
  const [search, setSearch] = useState("");

  const fetchEjercicios = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get(`/ejercicios?page=${page}&pageSize=${pageSize}&q=${search}`);
      const data = res.data;

      const lista = Array.isArray(data.items)
        ? data.items
        : Array.isArray(data)
        ? data
        : [];

      setEjercicios(lista);
      setTotalItems(data.totalItems || data.total || 0);
      setError(null);
    } catch (err) {
      console.error(err);
      setError("Error al cargar los ejercicios");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const delay = setTimeout(() => {
      if (search.length >= 3 || search.length === 0) {
        fetchEjercicios();
      }
    }, 400);

    return () => clearTimeout(delay);
  }, [page, search]);

  const handleDelete = async (id: number, nombre: string) => {
    const result = await Swal.fire({
      title: `¿Eliminar ${nombre}?`,
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/ejercicios/${id}`);
        await Swal.fire("Eliminado", "Ejercicio eliminado correctamente", "success");
        fetchEjercicios();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el ejercicio", "error");
      }
    }
  };

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearch(e.target.value);
    setPage(1);
  };

  if (loading) return <p>Cargando ejercicios...</p>;
  if (error) return <p className="text-danger">{error}</p>;

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.3rem", letterSpacing: "2px" }}
      >
        EJERCICIOS
      </h1>

      {/* 🔸 Barra superior: filtro + botón nuevo */}
      <div className="d-flex justify-content-between align-items-center mb-3">
        <div className="flex-grow-1 d-flex justify-content-start">
          <input
            type="text"
            placeholder="Buscar por nombre o grupo muscular..."
            value={search}
            onChange={handleSearch}
            className="form-control"
            style={{ width: "50%" }}
          />
        </div>

        <button
          className="btn btn-success ms-3"
          onClick={() => EjercicioCreateSwal(fetchEjercicios)}
        >
          ➕ Nuevo Ejercicio
        </button>
      </div>

      {/* 🔸 Tabla */}
      <div className="table-responsive">
        <table className="table table-striped table-hover align-middle">
          <thead className="table-dark">
            <tr>
              <th>Nombre</th>
              <th>Grupo Muscular</th>
              <th>Tips</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {ejercicios.map((e) => (
              <tr key={e.id}>
                <td>{e.nombre}</td>
                <td>{e.grupoMuscularNombre || "—"}</td>
                <td style={{ maxWidth: "400px", whiteSpace: "pre-wrap" }}>
                  {e.tips || "-"}
                </td>
                <td>
                  <button
                    className="btn btn-sm btn-warning me-2"
                    onClick={() => EjercicioEditSwal(e.id.toString(), fetchEjercicios)}
                  >
                    ✏️
                  </button>
                  <button
                    className="btn btn-sm btn-danger"
                    onClick={() => handleDelete(e.id, e.nombre)}
                  >
                    🗑️
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* 🔸 Paginación */}
      <Pagination
        currentPage={page}
        totalPages={Math.ceil(totalItems / pageSize)}
        totalItems={totalItems}
        pageSize={pageSize}
        onPageChange={(newPage) => setPage(newPage)}
      />
    </div>
  );
}
