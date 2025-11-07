import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import Pagination from "@/components/Pagination";
import { RutinaPlantillaCreateSwal } from "@/views/rutinas/rutina-plantilla/RutinaPlantillaCreateSwal";
import { RutinaPlantillaEditSwal } from "@/views/rutinas/rutina-plantilla/RutinaPlantillaEditSwal";

interface RutinaPlantilla {
  id: number;
  nombre: string;
  objetivo?: string;
  grupoMuscularId: number;
  grupoMuscularNombre?: string;
  imagenUrl?: string;
}

export default function RutinaPlantillaList() {
  const [rutinas, setRutinas] = useState<RutinaPlantilla[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalItems, setTotalItems] = useState(0);
  const [search, setSearch] = useState("");

  const fetchRutinas = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get(`/rutinasplantilla?page=${page}&pageSize=${pageSize}&q=${search}`);
      const data = res.data;
      const lista = Array.isArray(data.items) ? data.items : [];
      setRutinas(lista);
      setTotalItems(data.totalItems || 0);
      setError(null);
    } catch (err) {
      console.error(err);
      setError("Error al cargar las rutinas");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const delay = setTimeout(() => {
      if (search.length >= 3 || search.length === 0) {
        fetchRutinas();
      }
    }, 400);
    return () => clearTimeout(delay);
  }, [page, search]);

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearch(e.target.value);
    setPage(1);
  };

  const handleDelete = async (id: number) => {
    const result = await Swal.fire({
      title: "¿Eliminar rutina?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/rutinasplantilla/${id}`);
        Swal.fire("Eliminado", "Rutina eliminada correctamente", "success");
        fetchRutinas();
      } catch {
        Swal.fire("Error", "No se pudo eliminar la rutina", "error");
      }
    }
  };

  if (loading) return <p>Cargando rutinas...</p>;
  if (error) return <p className="text-danger">{error}</p>;

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.3rem", letterSpacing: "2px" }}
      >
        💪 RUTINAS PLANTILLA
      </h1>

      {/* 🔸 Barra superior */}
      <div className="d-flex justify-content-between align-items-center mb-3">
        <div className="flex-grow-1 d-flex justify-content-start">
          <input
            type="text"
            placeholder="Buscar por nombre, objetivo o grupo muscular..."
            value={search}
            onChange={handleSearch}
            className="form-control"
            style={{ width: "50%" }}
          />
        </div>

        <button
          className="btn btn-success ms-3"
          onClick={() => RutinaPlantillaCreateSwal(fetchRutinas)}
        >
          ➕ Nueva Rutina
        </button>
      </div>

      {/* 🔸 Tabla */}
      <div className="table-responsive">
        <table className="table table-striped table-hover align-middle">
          <thead className="table-dark">
            <tr>
              <th>Nombre</th>
              <th>Objetivo</th>
              <th>Grupo Muscular</th>
              <th>Imagen</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {rutinas.map((r) => (
              <tr key={r.id}>
                <td>{r.nombre}</td>
                <td>{r.objetivo || "-"}</td>
                <td>{r.grupoMuscularNombre || "—"}</td>
                <td>
                  {r.imagenUrl ? (
                    <img
                      src={r.imagenUrl}
                      alt={r.nombre}
                      style={{ width: "60px", height: "60px", objectFit: "cover", borderRadius: "8px" }}
                    />
                  ) : (
                    "—"
                  )}
                </td>
                <td>
                  <button
                    className="btn btn-sm btn-warning me-2"
                    onClick={() => RutinaPlantillaEditSwal(r.id.toString(), fetchRutinas)}
                  >
                    ✏️
                  </button>
                  <button
                    className="btn btn-sm btn-danger"
                    onClick={() => handleDelete(r.id)}
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
