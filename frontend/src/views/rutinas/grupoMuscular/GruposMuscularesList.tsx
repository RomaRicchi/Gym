import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import Pagination from "@/components/Pagination";
import { GrupoMuscularCreateSwal } from "@/views/rutinas/grupoMuscular/GrupoMuscularCreateSwal";
import { GrupoMuscularEditSwal } from "@/views/rutinas/grupoMuscular/GrupoMuscularEditSwal";

interface GrupoMuscular {
  id: number;
  nombre: string;
  descripcion?: string;
  imagenUrl?: string;
}

export default function GruposMuscularesList() {
  const [grupos, setGrupos] = useState<GrupoMuscular[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalItems, setTotalItems] = useState(0);
  const [search, setSearch] = useState("");

  const fetchGrupos = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get(`/grupomuscular?page=${page}&pageSize=${pageSize}&q=${search}`);
      const data = res.data;
      const lista = data.items || data;
      setGrupos(lista);
      setTotalItems(data.totalItems || lista.length);
    } catch (err) {
      console.error(err);
      setError("Error al cargar los grupos musculares");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const delay = setTimeout(() => {
      if (search.length >= 3 || search.length === 0) {
        fetchGrupos();
      }
    }, 400);
    return () => clearTimeout(delay);
  }, [page, search]);

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearch(e.target.value);
    setPage(1);
  };

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
        await gymApi.delete(`/grupomuscular/${id}`);
        Swal.fire("Eliminado", "Grupo muscular eliminado correctamente.", "success");
        fetchGrupos();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el grupo muscular.", "error");
      }
    }
  };

  if (loading) return <p>Cargando grupos musculares...</p>;
  if (error) return <p className="text-danger">{error}</p>;

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.3rem", letterSpacing: "2px" }}
      >
        🧠 GRUPOS MUSCULARES
      </h1>

      {/* 🔸 Barra superior */}
      <div className="d-flex justify-content-between align-items-center mb-3">
        <div className="flex-grow-1 d-flex justify-content-start">
          <input
            type="text"
            placeholder="Buscar grupo muscular..."
            value={search}
            onChange={handleSearch}
            className="form-control"
            style={{ width: "50%" }}
          />
        </div>

        <button
          className="btn btn-success ms-3"
          onClick={() => GrupoMuscularCreateSwal(fetchGrupos)}
        >
          ➕ Nuevo Grupo
        </button>
      </div>

      {/* 🔸 Cards de grupos */}
      <div className="row g-3">
        {grupos.map((g) => (
          <div key={g.id} className="col-md-4 col-lg-3">
            <div className="card shadow-sm text-center">
              {g.imagenUrl ? (
                <img
                  src={g.imagenUrl}
                  alt={g.nombre}
                  className="card-img-top"
                  style={{ height: "150px", objectFit: "cover" }}
                />
              ) : (
                <div
                  className="bg-light d-flex align-items-center justify-content-center"
                  style={{ height: "150px" }}
                >
                  <span className="text-muted">Sin imagen</span>
                </div>
              )}
              <div className="card-body">
                <h5 className="fw-bold">{g.nombre}</h5>
                <p className="text-muted small">{g.descripcion || "—"}</p>
                <div className="d-flex justify-content-center gap-2">
                  <button
                    className="btn btn-sm btn-warning"
                    onClick={() => GrupoMuscularEditSwal(g.id.toString(), fetchGrupos)}
                  >
                    ✏️
                  </button>
                  <button
                    className="btn btn-sm btn-danger"
                    onClick={() => handleDelete(g.id, g.nombre)}
                  >
                    🗑️
                  </button>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

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
