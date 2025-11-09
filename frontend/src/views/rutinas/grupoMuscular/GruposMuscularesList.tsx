// @ts-nocheck
import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import Pagination from "@/components/Pagination";
import { GrupoMuscularCreateSwal } from "@/views/rutinas/grupoMuscular/GrupoMuscularCreateSwal";
import { GrupoMuscularEditSwal } from "@/views/rutinas/grupoMuscular/GrupoMuscularEditSwal";

interface GrupoMuscular {
  id: number;
  nombre: string;
}

export default function GruposMuscularesList() {
  const [grupos, setGrupos] = useState<GrupoMuscular[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalItems, setTotalItems] = useState(0);

  const fetchGrupos = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get(`/grupomuscular?page=${page}&pageSize=${pageSize}`);
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
    fetchGrupos();
  }, [page]);

  const handleDelete = async (id: number, nombre: string) => {
    const result = await Swal.fire({
      title: `¿Eliminar "${nombre}"?`,
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
        Swal.fire({
          icon: "success",
          title: "Eliminado",
          text: "Grupo muscular eliminado correctamente.",
          timer: 1200,
          showConfirmButton: false,
        });
        fetchGrupos();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el grupo muscular.", "error");
      }
    }
  };

  if (loading)
    return <p className="text-center mt-5 text-light">Cargando grupos musculares...</p>;
  if (error)
    return <p className="text-danger text-center mt-4">{error}</p>;

  return (
    <div className="container mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.3rem", letterSpacing: "2px" }}
      >
        GRUPOS MUSCULARES
      </h1>

      {/* Botón superior */}
      <div className="d-flex justify-content-end mb-4">
        <button
          className="btn btn-success fw-semibold"
          onClick={() => GrupoMuscularCreateSwal(fetchGrupos)}
        >
          ➕ Nuevo Grupo
        </button>
      </div>

      {/* Tabla */}
      <div className="table-responsive">
        <table className="table table-striped align-middle text-center">
          <thead className="table-dark">
            <tr>
              <th>Nombre del Grupo Muscular</th>
              <th style={{ width: "140px" }}>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {grupos.length === 0 ? (
              <tr>
                <td colSpan={2} className="text-muted py-4">
                  No hay grupos musculares registrados.
                </td>
              </tr>
            ) : (
              grupos.map((g) => (
                <tr key={g.id}>
                  <td className="fw-semibold text-start ps-4">{g.nombre}</td>
                  <td>
                    <div className="d-flex justify-content-center gap-2">
                      <button
                        className="btn btn-sm btn-warning fw-semibold"
                        onClick={() => GrupoMuscularEditSwal(g.id.toString(), fetchGrupos)}
                      >
                        ✏️
                      </button>
                      <button
                        className="btn btn-sm btn-danger fw-semibold"
                        onClick={() => handleDelete(g.id, g.nombre)}
                      >
                        🗑️
                      </button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Paginación */}
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
