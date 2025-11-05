import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";  // estilos de formularios (naranja)
import "@/styles/orden.css";   // limpia inputs/selects fantasmas en alertas simples

const ESTADO_VERIFICADO_ID = 3; // id del estado "Verificado"

export async function crearOrdenDePago(socio: { id: number; nombre: string }) {
  try {
    // 1) Traer planes y estados
    const [{ data: planesResponse }, { data: estados }] = await Promise.all([
      gymApi.get("/planes"),
      gymApi.get("/estadoOrdenPago"),
    ]);

    const planes = planesResponse.items || planesResponse;

    // 2) Opciones para selects
    const opcionesPlanes = planes
      .map(
        (p: any) =>
          `<option value="${p.id}">
            ${p.nombre} (${p.diasPorSemana}x semana) — 💰 $${p.precio.toLocaleString()}
          </option>`
      )
      .join("");

    const opcionesEstados = estados
      .map(
        (e: any) =>
          `<option value="${e.id}" ${
            String(e.nombre).toLowerCase() === "pendiente" ? "selected" : ""
          }>${e.nombre}</option>`
      )
      .join("");

    // 3) Formulario (usamos clases propias .swal-field / .swal-select / .swal-textarea)
    const { value: formValues } = await Swal.fire<{
      planId: number;
      inicio: string;
      estadoId: number;
      notas: string;
      file?: File;
    }>({
      title:
        '<h2 class="fw-bold mb-3" style="font-size:1.6rem"><i class="fa-solid fa-file-invoice me-2"></i>Nueva orden de pago</h2>',
      html: `
        <div class="swal-form">
          <p><strong>Socio:</strong> ${socio.nombre}</p>
          <hr style="margin:4px 0 8px 0;">

          <label class="swal-label">Seleccionar plan</label>
          <select id="plan" class="swal-select">${opcionesPlanes}</select>

          <label class="swal-label">Fecha de inicio</label>
          <input id="inicio" type="date" class="swal-field"/>

          <label class="swal-label">Estado</label>
          <select id="estado" class="swal-select">${opcionesEstados}</select>

          <label class="swal-label">Comprobante (PDF o imagen)</label>
          <input id="comprobante" type="file" accept=".pdf,image/*" class="swal-field"/>

          <label class="swal-label">Notas</label>
          <textarea id="notas" class="swal-textarea" placeholder="Observaciones opcionales..."></textarea>
        </div>
      `,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: "Crear orden",
      cancelButtonText: "Cancelar",
      customClass: {
        popup: "swal2-card-style has-custom-form",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      buttonsStyling: false,
      preConfirm: () => {
        const planId = Number((document.getElementById("plan") as HTMLSelectElement)?.value);
        const inicio = (document.getElementById("inicio") as HTMLInputElement)?.value;
        const estadoId = Number((document.getElementById("estado") as HTMLSelectElement)?.value);
        const notas = (document.getElementById("notas") as HTMLTextAreaElement)?.value || "";
        const file = (document.getElementById("comprobante") as HTMLInputElement)?.files?.[0];

        if (!planId || !inicio || !estadoId) {
          Swal.showValidationMessage("⚠️ Debe completar Plan, Fecha de inicio y Estado.");
          return false;
        }
        return { planId, inicio, estadoId, notas, file };
      },
    });

    if (!formValues) return;

    const { planId, inicio, estadoId, notas, file } = formValues;

    // 4) FormData (fecha ISO para .NET)
    const formData = new FormData();
    formData.append("SocioId", String(socio.id));
    formData.append("PlanId", String(planId));
    formData.append("EstadoId", String(estadoId));
    formData.append("Notas", notas);
    formData.append("FechaInicio", new Date(inicio).toISOString());
    if (file) formData.append("file", file);

    // 5) Crear orden
    const { data: orden } = await gymApi.post("/ordenes", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    // 6) Confirmación con preview
    const planSel = planes.find((p: any) => p.id === planId);
    const estadoSel = estados.find((e: any) => e.id === estadoId);
    const comprobanteUrl = orden?.comprobante?.fileUrl || null;
    const baseUrl = (import.meta.env.VITE_API_URL || "").replace("/api", "") || "http://localhost:5144";
    const fullUrl = comprobanteUrl ? `${baseUrl}/${comprobanteUrl}` : null;

    let contenidoHtml = `
      <p><strong>Socio:</strong> ${socio.nombre}</p>
      <p><strong>Plan:</strong> ${planSel?.nombre}</p>
      <p><strong>Monto:</strong> 💰 $${planSel?.precio?.toLocaleString?.() ?? planSel?.precio}</p>
      <p><strong>Estado:</strong> ${estadoSel?.nombre}</p>
    `;

    if (fullUrl) {
      if (fullUrl.toLowerCase().endsWith(".pdf")) {
        contenidoHtml += `
          <hr>
          <p><strong>Comprobante (PDF):</strong></p>
          <iframe src="${fullUrl}" width="100%" height="400" style="border-radius:8px;"></iframe>
        `;
      } else {
        contenidoHtml += `
          <hr>
          <p><strong>Comprobante (imagen):</strong></p>
          <img src="${fullUrl}" alt="Comprobante" style="max-width:100%;border-radius:8px;">
        `;
      }
    } else {
      contenidoHtml += `<p><em>Pago sin comprobante (efectivo)</em></p>`;
    }

    await Swal.fire({
      icon: "success",
      title: "Orden creada correctamente",
      html: contenidoHtml,
      width: fullUrl ? "80%" : "40%",
      confirmButtonText: "Cerrar",
      customClass: { popup: "swal2-card-style swal-alert-simple" },
    });

    // 7) Crear suscripción automática si el estado es VERIFICADO (id=3)
    if (estadoId === ESTADO_VERIFICADO_ID) {
      try {
        const { data: resultado } = await gymApi.post(`/suscripciones/crear-por-orden/${orden.id}`);
        if (resultado?.exito) {
          await Swal.fire({
            toast: true,
            position: "top-end",
            icon: "success",
            title: "Suscripción activa creada",
            showConfirmButton: false,
            timer: 2500,
            timerProgressBar: true,
            background: "#198754",
            color: "#fff",
            customClass: { popup: "swal-alert-simple" },
          });
        }
      } catch (e) {
        console.warn("⚠️ No se pudo crear la suscripción automáticamente:", e);
      }
    }
  } catch (err) {
    console.error("Error al crear orden:", err);
    await Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudo crear la orden de pago.",
      customClass: { popup: "swal2-card-style swal-alert-simple" },
    });
  }
}
