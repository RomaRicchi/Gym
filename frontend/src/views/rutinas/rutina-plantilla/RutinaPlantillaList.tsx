// @ts-nocheck
import { useEffect, useState } from "react";
import { Button, Spinner } from "react-bootstrap";
import Swal from "sweetalert2";
import Select from "react-select";
import Pagination from "@/components/Pagination";
import gymApi from "@/api/gymApi";
import { RutinaPlantillaCreateSwal } from "./RutinaPlantillaCreateSwal";
import { RutinaPlantillaEditSwal } from "./RutinaPlantillaEditSwal";
import "@/styles/swal-ejercicio.css"; // usa el mismo estilo naranja moderno

export default function RutinaPlantillaList() {
  const [rutinas, setRutinas] = useState<any[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(8);
  const [totalItems, setTotalItems] = useState(0);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState<string | null>(null);
  const [options, setOptions] = useState<any[]>([]);

  // === 🔹 Cargar listado ===
  const cargarDatos = async (p = 1, q = "") => {
    setLoading(true);
    try {
      const { data } = await gymApi.get(`/rutinasplantilla?page=${p}&pageSize=${pageSize}&q=${q}`);
      setRutinas(data.items || []);
      setTotalItems(data.totalItems || 0);
    } catch (error) {
      console.error(error);
      Swal.fire("Error", "No se pudieron cargar las rutinas", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarDatos(page, search ?? "");
  }, [page, search]);

  // === 🔍 Buscador dinámico ===
  const fetchOptions = async (inputValue: string) => {
    try {
      const { data } = await gymApi.get(
        `/rutinasplantilla?page=1&pageSize=10&q=${encodeURIComponent(inputValue)}`
      );
      const opts =
        (data.items || []).map((r: any) => ({
          value: r.id,
          label: `${r.nombre} (${r.grupoMuscularNombre || "-"})`,
        })) ?? [];
      setOptions(opts);
      return opts;
    } catch {
      return [];
    }
  };

  const handleInputChange = (value: string) => {
    fetchOptions(value);
    return value;
  };

  // === 🗑️ Eliminar ===
  const eliminarRutina = async (id: number, nombre: string) => {
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
      await gymApi.delete(`/rutinasplantilla/${id}`);
      Swal.fire({
        icon: "success",
        title: "Eliminado",
        timer: 1000,
        showConfirmButton: false,
      });
      cargarDatos(page, search ?? "");
    } catch (err) {
      console.error("❌ Error al eliminar:", err.response || err);
      const msg = err.response?.data || "No se pudo eliminar la rutina";
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

  // === 🎯 Render ===
  return (
    <div className="container mt-4">
      <h1 className="titulo-modulo">RUTINAS</h1>

      {/* 🔍 Buscador y Botón alineados */}
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
        <div style={{ flex: "1 1 400px", maxWidth: "450px" }}>
          <Select
            placeholder="Buscar rutina o grupo muscular..."
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
          onClick={() => RutinaPlantillaCreateSwal(() => cargarDatos(page, search ?? ""))}
        >
          ➕ Nueva
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
                  <th>OBJETIVO</th>
                  <th>GRUPO MUSCULAR</th>
                  <th>ACCIONES</th>
                </tr>
              </thead>
              <tbody>
                {rutinas.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="text-muted py-4">
                      No hay rutinas registradas.
                    </td>
                  </tr>
                ) : (
                  rutinas.map((r) => (
                    <tr key={r.id}>
                      <td>
                        {r.imagenUrl ? (
                          <img
                            src={`${import.meta.env.VITE_API_BASE_URL.replace(/\/api$/, "")}/${r.imagenUrl}`}
                            alt={r.nombre}
                            className="miniatura-ejercicio"
                            onClick={() =>
                              Swal.fire({
                                imageUrl: `${import.meta.env.VITE_API_BASE_URL.replace(/\/api$/, "")}/${r.imagenUrl}`,
                                imageAlt: r.nombre,
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
                      <td className="fw-semibold">{r.nombre}</td>
                      <td className="text-muted small">{r.objetivo || "—"}</td>
                      <td>{r.grupoMuscularNombre || "—"}</td>
                      <td>
                        <div className="acciones-botones">
                          <Button
                            className="btn-accion btn-editar"
                            onClick={() => RutinaPlantillaEditSwal(r.id, () => cargarDatos(page, search ?? ""))}
                          >
                            ✏️
                          </Button>
                          <Button
                            className="btn-accion btn-eliminar"
                            onClick={() => eliminarRutina(r.id, r.nombre)}
                          >
                            🗑️
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
