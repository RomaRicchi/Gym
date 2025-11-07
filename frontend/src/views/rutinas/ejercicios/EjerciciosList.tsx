// @ts-nocheck
import { useEffect, useState, useRef } from "react";
import { Button } from "react-bootstrap";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { EjercicioCreateSwal } from "./EjercicioCreateSwal";
import { EjercicioEditSwal } from "./EjercicioEditSwal";
import $ from "jquery";
import DataTable from "datatables.net-dt";
import "datatables.net-dt/css/dataTables.dataTables.css";
import "datatables.net-responsive-dt";
import "datatables.net-responsive-dt/css/responsive.dataTables.css";
import "@/styles/swal-card.css";

export default function EjerciciosList() {
  const [ejercicios, setEjercicios] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const tableRef = useRef<HTMLTableElement | null>(null);
  const dtInstance = useRef<any>(null);

  const cargarDatos = async () => {
    setLoading(true);
    try {
      const { data } = await gymApi.get("/ejercicios");
      setEjercicios(data.items || []);
    } catch (error) {
      console.error(error);
      Swal.fire("Error", "No se pudieron cargar los ejercicios", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarDatos();
  }, []);

useEffect(() => {
  if (loading || !tableRef.current) return;

  // Si hay instancia previa, destruirla primero
  if (dtInstance.current) {
    try {
      dtInstance.current.destroy();
    } catch {}
    dtInstance.current = null;
  }

  if (ejercicios.length > 0) {
    dtInstance.current = new DataTable(tableRef.current, {
      responsive: true,
      destroy: true,
      language: {
        url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json",
      },
    });
  }
}, [loading, ejercicios]);

useEffect(() => {
  return () => {
    if (dtInstance.current) {
      try {
        dtInstance.current.destroy();
      } catch {}
    }
  };
}, []);


  const eliminarEjercicio = async (id: number) => {
    const result = await Swal.fire({
      title: "¿Eliminar ejercicio?",
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

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/ejercicios/${id}`);
        Swal.fire({
          icon: "success",
          title: "Eliminado",
          timer: 1000,
          showConfirmButton: false,
        });
        cargarDatos();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el ejercicio", "error");
      }
    }
  };

  // 👇 asegurate de que este return esté dentro del componente (no fuera de la función)
  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h3 className="fw-bold text-orange mb-0">
          🏋️‍♂️ Gestión de Ejercicios
        </h3>
        <Button
          className="btn btn-orange fw-semibold"
          onClick={() => EjercicioCreateSwal(cargarDatos)}
        >
          ➕ Nuevo
        </Button>
      </div>

      <div className="table-responsive">
        <table
          className="table table-striped align-middle text-center"
          ref={tableRef}
          style={{ width: "100%" }}
        >
          <thead className="table-dark">
            <tr>
              <th style={{ width: "90px" }}>Imagen</th>
              <th>Nombre</th>
              <th>Grupo Muscular</th>
              <th>Tips</th>
              <th style={{ width: "140px" }}>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {ejercicios.map((e) => (
              <tr key={e.id}>
                <td>
                  {e.mediaUrl ? (
                    <img
                      src={`${import.meta.env.VITE_API_URL}/${e.mediaUrl}`}
                      alt={e.nombre}
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
                <td>{e.nombre}</td>
                <td>{e.grupoMuscularNombre || "—"}</td>
                <td className="text-muted small" style={{ maxWidth: "220px" }}>
                  {e.tips || "—"}
                </td>
                <td>
                  <div className="d-flex justify-content-center gap-2">
                    <Button
                      size="sm"
                      variant="warning"
                      className="fw-semibold"
                      onClick={() => EjercicioEditSwal(e.id, cargarDatos)}
                    >
                      <i className="fa fa-edit"></i>
                    </Button>
                    <Button
                      size="sm"
                      variant="danger"
                      className="fw-semibold"
                      onClick={() => eliminarEjercicio(e.id)}
                    >
                      <i className="fa fa-trash"></i>
                    </Button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
