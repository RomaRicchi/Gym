// @ts-nocheck
import { useEffect, useState } from "react";
import { Button, Spinner } from "react-bootstrap";
import Swal from "sweetalert2";
import Select from "react-select";
import Pagination from "@/components/Pagination";
import gymApi from "@/api/gymApi";
import { EjercicioCreateSwal } from "./EjercicioCreateSwal";
import { EjercicioEditSwal } from "./EjercicioEditSwal";
import "@/styles/swal-ejercicio.css";


export default function EjerciciosList() {
  const [ejercicios, setEjercicios] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(8);
  const [totalItems, setTotalItems] = useState(0);
  const [search, setSearch] = useState<string | null>(null);
  const [options, setOptions] = useState<any[]>([]);

  // === Cargar ejercicios ===
  const cargarDatos = async (p = 1, q = "") => {
    setLoading(true);
    try {
      const { data } = await gymApi.get(`/ejercicios?page=${p}&pageSize=${pageSize}&q=${q}`);
      setEjercicios(data.items || []);
      setTotalItems(data.totalItems || 0);
    } catch (error) {
      console.error(error);
      Swal.fire("Error", "No se pudieron cargar los ejercicios", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarDatos(page, search ?? "");
  }, [page, search]);

  // === Buscador ===
  const fetchOptions = async (inputValue: string) => {
    try {
      const { data } = await gymApi.get(
        `/ejercicios?page=1&pageSize=10&q=${encodeURIComponent(inputValue)}`
      );
      const opts =
        (data.items || []).map((e: any) => ({
          value: e.id,
          label: `${e.nombre} (${e.grupoMuscularNombre || "-"})`,
        })) ?? [];
      setOptions(opts);
      return opts;
    } catch {
      return [];
    }
  };

  // === Eliminar ===
  const eliminarEjercicio = async (id: number, nombre: string) => {
    const result = await Swal.fire({
      title: `¿Eliminar "${nombre}"?`,
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn-orange",
        cancelButton: "btn-secondary",
      },
      buttonsStyling: false,
    });

    if (!result.isConfirmed) return;

    try {
      await gymApi.delete(`/ejercicios/${id}`);
      Swal.fire({
        icon: "success",
        title: "Eliminado",
        timer: 1000,
        showConfirmButton: false,
      });
      cargarDatos(page, search ?? "");
    } catch (err) {
      console.error("❌ Error al eliminar:", err.response || err);
      const msg = err.response?.data || "No se pudo eliminar el ejercicio";
      Swal.fire("Error", msg, "error");
    }
  };

  const selectStyles = {
    control: (base: any) => ({
      ...base,
      borderColor: "#ff6600",
      boxShadow: "none",
      "&:hover": { borderColor: "#ff6600" },
    }),
  };

  const handleInputChange = (value: string) => {
    fetchOptions(value);
    return value;
  };

  return (
    <div className="container mt-4">
      <h1 className="titulo-modulo">EJERCICIOS</h1>

      {/* 🔍 Buscador y Botón alineados */}
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
        <div style={{ flex: "1 1 400px", maxWidth: "450px" }}>
          <Select
            placeholder="Buscar ejercicio o grupo muscular..."
            isClearable
            onInputChange={handleInputChange}
            onChange={(opt) => {
              setSearch(opt ? opt.label.split(" (")[0] : null);
              setPage(1);
            }}
            options={options}
            styles={selectStyles}
          />
        </div>

        <Button
          className="btn btn-success"
          onClick={() => EjercicioCreateSwal(() => cargarDatos(page, search ?? ""))}
        >
          ➕ Nuevo
        </Button>
      </div>

      {loading ? (
        <div className="text-center mt-5">
          <Spinner animation="border" variant="warning" />
        </div>
      ) : (
        <>
          <div className="table-responsive">
            <table className="tabla-gestion table align-middle text-center">
              <thead>
                <tr>
                  <th>IMAGEN</th>
                  <th>NOMBRE</th>
                  <th>GRUPO MUSCULAR</th>
                  <th>TIPS</th>
                  <th>ACCIONES</th>
                </tr>
              </thead>
              <tbody>
                {ejercicios.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="text-muted py-4">
                      No hay ejercicios registrados.
                    </td>
                  </tr>
                ) : (
                  ejercicios.map((e) => (
                    <tr key={e.id}>
                      <td>
                        {e.mediaUrl ? (
                          <img
                            src={`${import.meta.env.VITE_API_BASE_URL.replace(/\/api$/, "")}/${e.mediaUrl}`}
                            alt={e.nombre}
                            className="miniatura-ejercicio"
                            onClick={() =>
                              Swal.fire({
                                imageUrl: `${import.meta.env.VITE_API_BASE_URL.replace(/\/api$/, "")}/${e.mediaUrl}`,
                                imageAlt: e.nombre,
                                background: "#000",
                                showConfirmButton: false,
                                showCloseButton: true,
                                width: "auto",
                                padding: "1rem",
                              })
                            }
                            onError={(ev) => (ev.currentTarget.src = "/placeholder.png")}
                          />
                        ) : (
                          <span className="text-muted">Sin imagen</span>
                        )}
                      </td>
                      <td className="fw-semibold">{e.nombre}</td>
                      <td>{e.grupoMuscularNombre || "—"}</td>
                      <td className="text-muted small">{e.tips || "—"}</td>
                      <td>
                        <div className="acciones-botones">
                          <Button
                            className="btn-accion btn-editar"
                            onClick={() => EjercicioEditSwal(e.id, () => cargarDatos(page, search ?? ""))}
                          >✏️ 
                            
                          </Button>
                          <Button
                            className="btn-accion btn-eliminar"
                            onClick={() => eliminarEjercicio(e.id, e.nombre)}
                          >🗑️ 
                          
                          </Button>
                        </div>
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
        </>
      )}
    </div>
  );
}
