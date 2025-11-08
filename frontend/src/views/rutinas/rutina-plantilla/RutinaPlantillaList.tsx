// @ts-nocheck
import { useEffect, useState } from "react";
import { Button, Spinner } from "react-bootstrap";
import Swal from "sweetalert2";
import Select from "react-select";
import Pagination from "@/components/Pagination";
import gymApi from "@/api/gymApi";
import { RutinaPlantillaCreateSwal } from "./RutinaPlantillaCreateSwal";
import { RutinaPlantillaEditSwal } from "./RutinaPlantillaEditSwal";
import "@/styles/swal-card.css";

export default function RutinaPlantillaList() {
  const [rutinas, setRutinas] = useState<any[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(8);
  const [totalItems, setTotalItems] = useState(0);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState<string | null>(null);
  const [options, setOptions] = useState<any[]>([]);

  // === 🔹 Cargar listado de rutinas ===
  const cargarDatos = async (p = 1, q = "") => {
    setLoading(true);
    try {
      const { data } = await gymApi.get(
        `/rutinasplantilla?page=${p}&pageSize=${pageSize}&q=${q}`
      );
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

  // === 🔍 Cargar opciones del buscador (React-Select) ===
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
    } catch (err) {
      console.error("⚠️ Error cargando opciones:", err);
      return [];
    }
  };

  // === 🗑️ Eliminar rutina ===
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
    } catch {
      Swal.fire("Error", "No se pudo eliminar la rutina", "error");
    }
  };

  // === 🎨 Estilos del Select ===
  const selectStyles = {
    control: (base: any) => ({
      ...base,
      borderColor: "#ff6600",
      boxShadow: "none",
      "&:hover": { borderColor: "#ff6600" },
    }),
    option: (base: any, state: any) => ({
      ...base,
      backgroundColor: state.isFocused ? "#ffe0cc" : "white",
      color: "#333",
    }),
  };

  // === 🎯 Render ===
  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
          <h1
            className="text-center fw-bold mb-4"
            style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
          >
            RUTINAS
          </h1>
        <Button
          className="btn btn-success fw-semibold"
          onClick={() => RutinaPlantillaCreateSwal(() => cargarDatos(page, search ?? ""))}
        >
          ➕ Nueva
        </Button>
      </div>

      {/* 🔍 Buscador con React-Select */}
      <div className="mb-3" style={{ maxWidth: "400px" }}>
        <Select
          placeholder="Buscar rutina o grupo muscular..."
          isClearable
          onInputChange={(value) => fetchOptions(value)}
          onChange={(opt) => {
            setSearch(opt ? opt.label.split(" (")[0] : null);
            setPage(1);
          }}
          options={options}
          styles={selectStyles}
        />
      </div>

      {loading ? (
        <div className="text-center mt-5">
          <Spinner animation="border" variant="warning" />
        </div>
      ) : (
        <>
          <div className="table-responsive">
            <table className="table table-striped align-middle text-center">
              <thead className="table-dark">
                <tr>
                  <th style={{ width: "90px" }}>Imagen</th>
                  <th>Nombre</th>
                  <th>Objetivo</th>
                  <th>Grupo Muscular</th>
                  <th style={{ width: "140px" }}>Acciones</th>
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
                            src={`${import.meta.env.VITE_API_BASE_URL}/${r.imagenUrl}`}
                            alt={r.nombre}
                            style={{
                              width: "70px",
                              height: "70px",
                              objectFit: "cover",
                              borderRadius: "8px",
                              border: "2px solid #ff6600",
                            }}
                          />
                        ) : (
                          <span className="text-muted">Sin imagen</span>
                        )}
                      </td>
                      <td>{r.nombre}</td>
                      <td className="text-muted small" style={{ maxWidth: "280px" }}>
                        {r.objetivo || "—"}
                      </td>
                      <td>{r.grupoMuscularNombre || "—"}</td>
                      <td>
                        <div className="d-flex justify-content-center gap-2">
                          <Button
                            size="sm"
                            variant="warning"
                            className="fw-semibold"
                            onClick={() =>
                              RutinaPlantillaEditSwal(r.id, () => cargarDatos(page, search ?? ""))
                            }
                          >
                            <i className="fa fa-edit"></i>
                          </Button>
                          <Button
                            size="sm"
                            variant="danger"
                            className="fw-semibold"
                            onClick={() => eliminarRutina(r.id, r.nombre)}
                          >
                            <i className="fa fa-trash"></i>
                          </Button>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          {/* 📄 Paginación */}
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
