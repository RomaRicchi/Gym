import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import Pagination from "@/components/Pagination";
import { RutinaPlantillaEjercicioCreateSwal } from "@/views/rutinas/rutina-ejercicios/RutinaPlantillaEjercicioCreateSwal";
import { RutinaPlantillaEjercicioEditSwal } from "@/views/rutinas/rutina-ejercicios/RutinaPlantillaEjercicioEditSwal";

interface RutinaPlantillaEjercicio {
  id: number;
  rutina: string;
  ejercicio: string;
  orden: number;
  series: number;
  repeticiones: number;
  descansoSeg: number;
}

export default function RutinaPlantillaEjercicioList() {
  const [items, setItems] = useState<RutinaPlantillaEjercicio[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalItems, setTotalItems] = useState(0);

  // 🔹 Cargar lista desde backend
  const fetchItems = async () => {
    setLoading(true);
    try {
      const res = await gymApi.get(
        `/rutinasplantillaejercicios?page=${page}&pageSize=${pageSize}&q=${search}`
      );
      const data = res.data;
      setItems(data.items || []);
      setTotalItems(data.totalItems || 0);
    } catch (err) {
      console.error("❌ Error cargando lista:", err);
      Swal.fire("Error", "No se pudieron cargar los ejercicios de rutina", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
  const delay = setTimeout(() => {
    if (search.length >= 3 || search.length === 0) {
      fetchItems();
    }
  }, 400); 
  return () => clearTimeout(delay);
}, [page, search]);


  // Eliminar registro
  const handleDelete = async (id: number) => {
    const result = await Swal.fire({
      title: "¿Eliminar registro?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/rutinasplantillaejercicios/${id}`);
        Swal.fire("Eliminado", "Registro eliminado correctamente", "success");
        fetchItems();
      } catch (err) {
        console.error(err);
        Swal.fire("Error", "No se pudo eliminar el registro", "error");
      }
    }
  };

  if (loading) return <p>Cargando ejercicios...</p>;

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.3rem", letterSpacing: "2px" }}
      >
        RUTINAS / EJERCICIOS
      </h1>

      {/* Buscador y botón */}
      <div className="d-flex justify-content-between align-items-center mb-3">
        <input
          type="text"
          className="form-control"
          placeholder="Buscar por rutina o ejercicio..."
          value={search}
          onChange={(e) => {
            setSearch(e.target.value);
            setPage(1);
          }}
          style={{ width: "50%" }}
        />
        <button
          className="btn btn-success ms-3"
          onClick={() => RutinaPlantillaEjercicioCreateSwal(fetchItems)}
        >
          ➕ Nuevo
        </button>
      </div>

      {/* 🔹 Tabla */}
      <div className="table-responsive">
        <table className="table table-striped table-hover align-middle">
          <thead className="table-dark">
            <tr>
              <th>Rutina</th>
              <th>Ejercicio</th>
              <th>Orden</th>
              <th>Series</th>
              <th>Reps</th>
              <th>Descanso (seg)</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {items.length === 0 ? (
              <tr>
                <td colSpan={7} className="text-center">
                  No se encontraron registros
                </td>
              </tr>
            ) : (
              items.map((r) => (
                <tr key={r.id}>
                  <td>{r.rutina}</td>
                  <td>{r.ejercicio}</td>
                  <td>{r.orden}</td>
                  <td>{r.series}</td>
                  <td>{r.repeticiones}</td>
                  <td>{r.descansoSeg}</td>
                  <td>
                    <button
                      className="btn btn-sm btn-warning me-2"
                      onClick={() =>
                        RutinaPlantillaEjercicioEditSwal(r.id.toString(), fetchItems)
                      }
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
              ))
            )}
          </tbody>
        </table>
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
