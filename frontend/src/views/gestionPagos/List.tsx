import { useEffect, useState, useRef } from "react";
import { Button } from "react-bootstrap";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { editarOrden } from "@/views/gestionPagos/OrdenPagoEdit";
import $ from "jquery";
import DataTable from "datatables.net-dt";
import "datatables.net-dt/css/dataTables.dataTables.css";
import "datatables.net-responsive-dt";
import "datatables.net-responsive-dt/css/responsive.dataTables.css";

export default function OrdenesList() {
  const [ordenes, setOrdenes] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [fechaInicio, setFechaInicio] = useState("");
  const [fechaFin, setFechaFin] = useState("");

  const tableRef = useRef<HTMLTableElement | null>(null);
  const dtInstance = useRef<any>(null);
  const fechaInicioRef = useRef("");
  const fechaFinRef = useRef("");

  useEffect(() => {
    fechaInicioRef.current = fechaInicio;
    fechaFinRef.current = fechaFin;
    if (dtInstance.current) dtInstance.current.draw();
  }, [fechaInicio, fechaFin]);

  // 🔹 Cargar datos
  const fetchOrdenes = async () => {
    try {
      const res = await gymApi.get("/ordenes");
      setOrdenes(res.data || []);
    } catch {
      Swal.fire("Error", "No se pudieron cargar las órdenes de pago", "error");
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => { fetchOrdenes(); }, []);

  // 🔹 Inicializar DataTable
  useEffect(() => {
    if (!loading && tableRef.current && !dtInstance.current) {
      dtInstance.current = new DataTable(tableRef.current, {
        responsive: true,
        pageLength: 10,
        destroy: true,
        order: [[6, "asc"]], // ordenar por venceISO
        language: {
          url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json",
        },
        columns: [
          { data: "socio" },
          { data: "plan" },
          { data: "monto" },
          { data: "vence" },
          { data: "estado" },
          { data: "acciones" },
          { data: "venceISO", visible: false, defaultContent: "" }, // ✅ fallback seguro
        ],
      });

      // 🔹 Filtro personalizado
      $.fn.dataTable.ext.search.push((settings: any, data: string[]) => {
        const desde = fechaInicioRef.current || "0000-01-01";
        const hasta = fechaFinRef.current || "9999-12-31";
        const venceISO = data[6] || ""; // usa columna oculta
        if (!venceISO) return true;
        return venceISO >= desde && venceISO <= hasta;
      });
    }

    return () => {
      $.fn.dataTable.ext.search.pop();
      if (dtInstance.current) {
        dtInstance.current.destroy(true);
        dtInstance.current = null;
      }
    };
  }, [loading]);

  // 🔹 Formateo seguro
  const parseFecha = (fecha: any) => {
    try {
      const d = new Date(fecha);
      if (isNaN(d.getTime())) return { iso: "", local: "" };
      const iso = d.toISOString().split("T")[0];
      const local = d.toLocaleDateString("es-AR", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
      });
      return { iso, local };
    } catch {
      return { iso: "", local: "" };
    }
  };

  //  Cargar / refrescar filas
  useEffect(() => {
    if (dtInstance.current) {
      const rows = ordenes.map((o) => {
        const { iso, local } = parseFecha(o.venceEn || o.venceISO || null);

        return {
          socio: o.socio?.nombre || "—",
          plan: o.plan?.nombre || "—",
          monto: `$${o.monto?.toFixed(2) || "0.00"}`,
          vence: local || "—",
          venceISO: iso || "", // ✅ garantizado siempre presente
          estado: `
            <span class="badge ${
              o.estado?.nombre?.toLowerCase() === "verificado"
                ? "bg-success"
                : o.estado?.nombre?.toLowerCase() === "pendiente"
                ? "bg-warning text-dark"
                : o.estado?.nombre?.toLowerCase() === "rechazado"
                ? "bg-danger"
                : "bg-secondary"
            }">${o.estado?.nombre || "Sin estado"}</span>
          `,
          acciones: `
              <div class="d-flex justify-content-center gap-2">
                <button class="btn ${
                  o.comprobante && o.comprobante.id
                    ? "btn-primary"
                    : "btn-secondary disabled"
                } btn-sm ver"
                  data-id="${o.id}"
                  ${!o.comprobante ? "disabled" : ""}
                  title="Comprobantes">
                  📎
                </button>
              <button class="btn btn-warning btn-sm editar" data-id="${o.id}" title="Cambiar estado">✏️</button>
              <button class="btn btn-danger btn-sm eliminar" data-id="${o.id}" title="Eliminar">🗑️</button>
            </div>
          `,
        };
      });

      dtInstance.current.clear();
      dtInstance.current.rows.add(rows).draw();

      $(tableRef.current!).off("click");

      $(tableRef.current!).on("click", ".editar", async function () {
        const id = $(this).data("id");
        const ok = await editarOrden(id);
        if (ok) fetchOrdenes();
      });

      $(tableRef.current!).on("click", ".eliminar", function () {
        const id = $(this).data("id");
        eliminarOrden(id);
      });

      $(tableRef.current!).on("click", ".ver", async function () {
        const id = $(this).data("id");

        try {
          const { data } = await gymApi.get(`/ordenes/${id}`);
          const fileUrl = data?.comprobante?.fileUrl;

          if (!fileUrl) {
            Swal.fire("Sin comprobante", "Esta orden no tiene archivo cargado.", "info");
            return;
          }

          // 🧩 Construimos la URL completa con el backend
          const baseUrl = import.meta.env.VITE_API_URL?.replace("/api", "") || "http://localhost:5144";
          const fullUrl = `${baseUrl}/${fileUrl}`; 
          if (fileUrl.toLowerCase().endsWith(".pdf")) {
            Swal.fire({
              title: "Comprobante PDF",
              html: `<iframe src="${fullUrl}" width="100%" height="500px"></iframe>`,
              width: "80%",
              confirmButtonText: "Cerrar",
            });
          } else {
            Swal.fire({
              title: "Comprobante",
              imageUrl: fullUrl,
              imageAlt: "Comprobante de pago",
              width: "60%",
              confirmButtonText: "Cerrar",
            });
          }
        } catch (err) {
          Swal.fire("Error", "No se pudo cargar el comprobante.", "error");
        }
      });


    }
  }, [ordenes]);

  if (loading)
    return (
      <div className="text-center mt-5">
        <div className="spinner-border text-primary" role="status"></div>
        <p className="mt-3">Cargando órdenes...</p>
      </div>
    );

  return (
    <div className="container mt-3">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        ORDENES DE PAGO
      </h1>

      {/* 🔹 Filtro */}
      <div className="d-flex flex-wrap align-items-end gap-3 mb-3">
        <div>
          <label className="fw-bold">Desde</label>
          <input
            type="date"
            className="form-control"
            value={fechaInicio}
            onChange={(e) => setFechaInicio(e.target.value)}
          />
        </div>
        <div>
          <label className="fw-bold">Hasta</label>
          <input
            type="date"
            className="form-control"
            value={fechaFin}
            onChange={(e) => setFechaFin(e.target.value)}
          />
        </div>
        <Button
          variant="secondary"
          onClick={() => {
            setFechaInicio("");
            setFechaFin("");
          }}
        >
          Limpiar
        </Button>
      </div>

      <table
        ref={tableRef}
        className="display table table-striped table-bordered align-middle text-center"
        style={{ width: "100%" }}
      >
        <thead className="table-dark">
          <tr>
            <th>Socio</th>
            <th>Plan</th>
            <th>Monto</th>
            <th>Vence</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
      </table>
    </div>
  );

  async function eliminarOrden(id: number) {
    const result = await Swal.fire({
      title: "¿Eliminar orden?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
    });
    if (!result.isConfirmed) return;

    try {
      await gymApi.delete(`/ordenes/${id}`);
      Swal.fire("Eliminada", "La orden fue eliminada correctamente.", "success");
      fetchOrdenes();
    } catch {
      Swal.fire("Error", "No se pudo eliminar la orden.", "error");
    }
  }
}
