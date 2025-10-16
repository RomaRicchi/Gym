// @ts-nocheck
import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { useNavigate } from "react-router-dom";
import "select2/dist/css/select2.min.css";
import "select2/dist/js/select2.full.min.js";

export default function OrdenPagoCreate() {
  const navigate = useNavigate();

  const [socios, setSocios] = useState<any[]>([]);
  const [planes, setPlanes] = useState<any[]>([]);
  const [estados, setEstados] = useState<any[]>([]);
  const [form, setForm] = useState({
    socioId: "",
    planId: "",
    monto: "",
    estadoId: "",
    notas: "",
  });

  // 🔹 Función segura para obtener arrays del backend
  const safeArray = (res: any) => {
    if (Array.isArray(res)) return res;
    if (Array.isArray(res?.data)) return res.data;
    if (Array.isArray(res?.result)) return res.result;
    if (Array.isArray(res?.data?.data)) return res.data.data;
    return [];
  };

  // 🔹 Cargar datos desde la API
  useEffect(() => {
    const cargarDatos = async () => {
      try {
        const [sociosRes, planesRes, estadosRes] = await Promise.all([
          gymApi.get("/socios"),
          gymApi.get("/planes"),
          gymApi.get("/estados"),
        ]);

        setSocios(safeArray(sociosRes.data));
        setPlanes(safeArray(planesRes.data));
        setEstados(safeArray(estadosRes.data));
      } catch (err) {
        console.error(err);
        Swal.fire("Error", "No se pudieron cargar los datos.", "error");
      }
    };

    cargarDatos();
  }, []);

  // 🔹 Inicializar Select2 cuando los datos estén listos
  useEffect(() => {
    if (!socios.length || !planes.length || !estados.length) return;

    const $ = (window as any).$; // usar la instancia global de jQuery cargada en main.tsx

    const initSelect2 = () => {
      const $socio = $("#socioSelect");
      const $plan = $("#planSelect");
      const $estado = $("#estadoSelect");

      // Destruir instancias previas
      if ($socio.hasClass("select2-hidden-accessible")) $socio.select2("destroy");
      if ($plan.hasClass("select2-hidden-accessible")) $plan.select2("destroy");
      if ($estado.hasClass("select2-hidden-accessible")) $estado.select2("destroy");

      // Inicializar Select2 con buscador
      $socio
        .select2({
          placeholder: "Seleccione un socio",
          allowClear: true,
          width: "100%",
        })
        .on("change", (e: any) =>
          setForm((prev) => ({ ...prev, socioId: e.target.value }))
        );

      $plan
        .select2({
          placeholder: "Seleccione un plan",
          allowClear: true,
          width: "100%",
        })
        .on("change", (e: any) => {
          const planId = e.target.value;
          const planSel = planes.find((p: any) => p.id == planId);
          setForm((prev) => ({
            ...prev,
            planId,
            monto: planSel ? planSel.precio : "",
          }));
        });

      $estado
        .select2({
          placeholder: "Seleccione un estado",
          allowClear: true,
          width: "100%",
        })
        .on("change", (e: any) =>
          setForm((prev) => ({ ...prev, estadoId: e.target.value }))
        );
    };

    // ⚙️ Ejecutar cuando React haya renderizado completamente el DOM
    const timeout = setTimeout(initSelect2, 500);
    return () => clearTimeout(timeout);
  }, [socios, planes, estados]);

  // 🔹 Guardar orden
  const handleSubmit = async (e: any) => {
    e.preventDefault();
    try {
      await gymApi.post("/ordenes", {
        socioId: Number(form.socioId),
        planId: Number(form.planId),
        monto: Number(form.monto),
        estadoId: Number(form.estadoId),
        notas: form.notas,
      });

      Swal.fire("Éxito", "Orden de pago creada correctamente", "success");
      navigate("/ordenes");
    } catch (error) {
      console.error(error);
      Swal.fire("Error", "No se pudo crear la orden.", "error");
    }
  };

  return (
    <div className="container mt-4">
      <h3>Nueva Orden de Pago</h3>
      <form onSubmit={handleSubmit}>
        {/* SOCIO */}
        <div className="mb-3">
          <label>Socio</label>
          <select id="socioSelect" className="form-select">
            <option value="">Seleccione un socio</option>
            {socios.map((s) => (
              <option key={s.id} value={s.id}>
                {s.nombre} ({s.email})
              </option>
            ))}
          </select>
        </div>

        {/* PLAN */}
        <div className="mb-3">
          <label>Plan</label>
          <select id="planSelect" className="form-select">
            <option value="">Seleccione un plan</option>
            {planes.map((p) => (
              <option key={p.id} value={p.id}>
                {p.nombre} - ${p.precio}
              </option>
            ))}
          </select>
        </div>

        {/* MONTO */}
        <div className="mb-3">
          <label>Monto</label>
          <input
            type="number"
            className="form-control"
            value={form.monto}
            readOnly
          />
        </div>

        {/* ESTADO */}
        <div className="mb-3">
          <label>Estado</label>
          <select id="estadoSelect" className="form-select">
            <option value="">Seleccione un estado</option>
            {estados.map((e) => (
              <option key={e.id} value={e.id}>
                {e.nombre}
              </option>
            ))}
          </select>
        </div>

        {/* NOTAS */}
        <div className="mb-3">
          <label>Notas</label>
          <textarea
            className="form-control"
            value={form.notas}
            onChange={(e) =>
              setForm((prev) => ({ ...prev, notas: e.target.value }))
            }
          ></textarea>
        </div>

        {/* BOTONES */}
        <button className="btn btn-primary" type="submit">
          Guardar
        </button>
        <button
          className="btn btn-secondary ms-2"
          type="button"
          onClick={() => navigate("/ordenes")}
        >
          Cancelar
        </button>
      </form>
    </div>
  );
}
