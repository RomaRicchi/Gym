// @ts-nocheck
import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { useNavigate, useParams } from "react-router-dom";
import $ from "jquery";

(globalThis as any).$ = $;
(globalThis as any).jQuery = $;

import "select2/dist/css/select2.min.css";
import "select2/dist/js/select2.full.min.js";

export default function OrdenPagoEdit() {
  const navigate = useNavigate();
  const { id } = useParams();

  const [orden, setOrden] = useState(null);
  const [socios, setSocios] = useState([]);
  const [planes, setPlanes] = useState([]);
  const [estados, setEstados] = useState([]);

  // ✅ Cargar datos iniciales
  useEffect(() => {
    const fetchData = async () => {
      try {
        const [ordenRes, sociosRes, planesRes, estadosRes] = await Promise.all([
          gymApi.get(`/ordenes/${id}`),
          gymApi.get("/socios"),
          gymApi.get("/planes"),
          gymApi.get("/estados"),
        ]);

        setOrden(ordenRes.data);
        setSocios(sociosRes.data);
        setPlanes(planesRes.data);
        setEstados(estadosRes.data);

        // Inicializar Select2 una vez cargados los datos
        setTimeout(() => {
          $("#socioSelect").select2({
            placeholder: "Seleccione un socio",
            width: "100%",
          }).val(ordenRes.data.socioId).trigger("change");
        }, 200);
      } catch (err) {
        console.error(err);
        Swal.fire("Error", "No se pudieron cargar los datos de la orden.", "error");
      }
    };

    fetchData();
  }, [id]);

  // ✅ Manejar cambios
  const handleChange = (e) => {
    const { name, value } = e.target;
    setOrden({ ...orden, [name]: value });
  };

  // ✅ Guardar cambios
  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await gymApi.put(`/ordenes/${id}`, {
        socioId: $("#socioSelect").val(),
        planId: orden.planId,
        monto: orden.monto,
        venceEn: orden.venceEn,
        estadoId: orden.estadoId,
      });

      Swal.fire("Éxito", "Orden de pago actualizada correctamente", "success");
      navigate("/ordenes");
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudo actualizar la orden.", "error");
    }
  };

  if (!orden) return <p>Cargando...</p>;

  return (
    <div className="container mt-4">
      <h3>Editar Orden de Pago</h3>
      <form onSubmit={handleSubmit} className="card p-4 mt-3 shadow-sm">

        {/* Socio */}
        <div className="mb-3">
          <label className="form-label fw-bold">Socio</label>
          <select id="socioSelect" name="socioId" defaultValue={orden.socioId} className="form-select">
            <option value="">Seleccione un socio...</option>
            {socios.map((s) => (
              <option key={s.id} value={s.id}>{s.nombre}</option>
            ))}
          </select>
        </div>

        {/* Plan */}
        <div className="mb-3">
          <label className="form-label fw-bold">Plan</label>
          <select
            className="form-select"
            name="planId"
            value={orden.planId}
            onChange={handleChange}
          >
            <option value="">Seleccione un plan...</option>
            {planes.map((p) => (
              <option key={p.id} value={p.id}>{p.nombre}</option>
            ))}
          </select>
        </div>

        {/* Monto */}
        <div className="mb-3">
          <label className="form-label fw-bold">Monto</label>
          <input
            type="number"
            name="monto"
            className="form-control"
            value={orden.monto}
            onChange={handleChange}
          />
        </div>

        {/* Fecha de vencimiento */}
        <div className="mb-3">
          <label className="form-label fw-bold">Vence</label>
          <input
            type="date"
            name="venceEn"
            className="form-control"
            value={orden.venceEn?.split("T")[0] || ""}
            onChange={handleChange}
          />
        </div>

        {/* Estado */}
        <div className="mb-3">
          <label className="form-label fw-bold">Estado</label>
          <select
            className="form-select"
            name="estadoId"
            value={orden.estadoId}
            onChange={handleChange}
          >
            {estados.map((e) => (
              <option key={e.id} value={e.id}>{e.nombre}</option>
            ))}
          </select>
        </div>

        <button type="submit" className="btn btn-primary w-100">
          Guardar Cambios
        </button>
      </form>
    </div>
  );
}
