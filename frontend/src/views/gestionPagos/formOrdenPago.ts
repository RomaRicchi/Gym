// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import $ from "jquery";
import "select2/dist/css/select2.min.css";
import "select2/dist/js/select2.full.min.js";

export async function mostrarFormularioOrdenPago() {
  try {
    // 🔹 Cargar datos antes de mostrar el modal
    const [sociosRes, planesRes, estadosRes] = await Promise.all([
      gymApi.get("/socios"),
      gymApi.get("/planes"),
      gymApi.get("/estados"),
    ]);

    const safeArray = (res) => {
      if (Array.isArray(res)) return res;
      if (Array.isArray(res?.data)) return res.data;
      if (Array.isArray(res?.result)) return res.result;
      if (Array.isArray(res?.data?.data)) return res.data.data;
      return [];
    };

    const socios = safeArray(sociosRes.data);
    const planes = safeArray(planesRes.data);
    const estados = safeArray(estadosRes.data);

    // 🔹 Construir HTML del formulario
    const html = `
      <form id="formOrdenPago" class="swal2-card-style text-start">
        <div class="mb-3">
          <label class="form-label">Socio</label>
          <select id="socioSelect" class="form-select">
            <option value="">Seleccione un socio</option>
            ${socios
              .map(
                (s) =>
                  `<option value="${s.id}">${s.nombre} (${s.email || ""})</option>`
              )
              .join("")}
          </select>
        </div>

        <div class="mb-3">
          <label class="form-label">Plan</label>
          <select id="planSelect" class="form-select">
            <option value="">Seleccione un plan</option>
            ${planes
              .map(
                (p) =>
                  `<option value="${p.id}">${p.nombre} - $${p.precio}</option>`
              )
              .join("")}
          </select>
        </div>

        <div class="mb-3">
          <label class="form-label">Monto</label>
          <input type="number" id="montoInput" class="form-control" readonly />
        </div>

        <div class="mb-3">
          <label class="form-label">Estado</label>
          <select id="estadoSelect" class="form-select">
            <option value="">Seleccione un estado</option>
            ${estados
              .map((e) => `<option value="${e.id}">${e.nombre}</option>`)
              .join("")}
          </select>
        </div>

        <div class="mb-3">
          <label class="form-label">Notas</label>
          <textarea id="notasInput" class="form-control"></textarea>
        </div>
      </form>
    `;

    // 🔹 Mostrar el SweetAlert2
    const { isConfirmed } = await Swal.fire({
      title: "Nueva Orden de Pago",
      html,
      showCancelButton: true,
      confirmButtonText: "Guardar",
      cancelButtonText: "Cancelar",
      width: 600,
      focusConfirm: false,
      didOpen: () => {
  const $ = (window as any).$;

  if (!$.fn || !$.fn.select2) {
    console.error("❌ Select2 no está disponible en window.$");
    return;
  }

  // Destruir instancias previas
  $("#socioSelect").select2("destroy");
  $("#planSelect").select2("destroy");
  $("#estadoSelect").select2("destroy");

  // Inicializar nuevamente
  $("#socioSelect").select2({ placeholder: "Seleccione un socio", width: "100%" });
  $("#planSelect").select2({ placeholder: "Seleccione un plan", width: "100%" });
  $("#estadoSelect").select2({ placeholder: "Seleccione un estado", width: "100%" });

  // Actualizar monto al seleccionar plan
  $("#planSelect").on("change", (e: any) => {
    const planId = e.target.value;
    const planSel = planes.find((p) => p.id == planId);
    $("#montoInput").val(planSel ? planSel.precio : "");
  });

  console.log("✅ Select2 aplicado dentro del modal");
},


      preConfirm: () => {
        const socioId = $("#socioSelect").val();
        const planId = $("#planSelect").val();
        const estadoId = $("#estadoSelect").val();
        const monto = $("#montoInput").val();
        const notas = $("#notasInput").val();

        if (!socioId || !planId || !estadoId)
          return Swal.showValidationMessage("Debe completar todos los campos");

        return { socioId, planId, estadoId, monto, notas };
      },
    });

    // 🔹 Si se confirmó, enviar al backend
    if (isConfirmed) {
      const data = Swal.getPopup().querySelector("#formOrdenPago");

      const payload = {
        socioId: Number($("#socioSelect").val()),
        planId: Number($("#planSelect").val()),
        monto: Number($("#montoInput").val()),
        estadoId: Number($("#estadoSelect").val()),
        notas: $("#notasInput").val(),
      };

      await gymApi.post("/ordenes", payload);
      Swal.fire("Éxito", "Orden de pago creada correctamente", "success");
    }
  } catch (error) {
    console.error(error);
    Swal.fire("Error", "No se pudo crear la orden.", "error");
  }
}
