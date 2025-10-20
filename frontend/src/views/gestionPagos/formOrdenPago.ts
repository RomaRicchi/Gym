// fileName: formOrdenPago.ts
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function crearOrdenDePago(socio: { id: number; nombre: string }) {
  try {
    // 1️⃣ Traer planes y estados
    const [{ data: planesResponse }, { data: estados }] = await Promise.all([
      gymApi.get("/planes"),
      gymApi.get("/estadoOrdenPago"),
    ]);

    const planes = planesResponse.items || planesResponse;

    // 2️⃣ Opciones para selects
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
            e.nombre.toLowerCase() === "pendiente" ? "selected" : ""
          }>${e.nombre}</option>`
      )
      .join("");

    // 3️⃣ Formulario visual con SweetAlert2
    const { value: formValues } = await Swal.fire({
      title: "🧾 Nueva orden de pago",
      html: `
        <div style="text-align:left;display:flex;flex-direction:column;gap:10px;max-width:100%;">
          <p style="margin:0;"><strong>Socio:</strong> ${socio.nombre}</p>
          <hr style="margin:4px 0 8px 0;">

          <label>Seleccionar plan</label>
          <select id="plan"
            style="background:#fff;color:#000;border:1px solid #ccc;border-radius:6px;padding:6px;width:100%;">
            ${opcionesPlanes}
          </select>

          <label>Fecha de inicio</label>
          <input type="date" id="inicio"
            style="background:#fff;color:#000;border:1px solid #ccc;border-radius:6px;padding:6px;width:100%;" />

          <label>Estado</label>
          <select id="estado"
            style="background:#fff;color:#000;border:1px solid #ccc;border-radius:6px;padding:6px;width:100%;">
            ${opcionesEstados}
          </select>

          <label>Comprobante (PDF o imagen)</label>
          <input type="file" id="comprobante" accept=".pdf,image/*"
            style="background:#fff;color:#000;border:1px solid #ccc;border-radius:6px;padding:6px;width:100%;" />

          <label>Notas</label>
          <textarea id="notas" placeholder="Observaciones opcionales..."
            style="background:#fff;color:#000;border:1px solid #ccc;border-radius:6px;padding:6px;width:100%;height:80px;resize:none;"></textarea>
        </div>
      `,
      focusConfirm: false,
      confirmButtonText: "Crear orden",
      confirmButtonColor: "#ff6600",
      showCancelButton: true,
      cancelButtonText: "Cancelar",
      width: 480,
      preConfirm: () => {
        const planId = (document.getElementById("plan") as HTMLSelectElement)?.value;
        const inicio = (document.getElementById("inicio") as HTMLInputElement)?.value;
        const estadoId = (document.getElementById("estado") as HTMLSelectElement)?.value;
        const notas = (document.getElementById("notas") as HTMLTextAreaElement)?.value;
        const file = (document.getElementById("comprobante") as HTMLInputElement)?.files?.[0];

        if (!planId || !inicio || !estadoId) {
          Swal.showValidationMessage("⚠️ Debe completar los campos obligatorios.");
          return false;
        }

        return { planId: Number(planId), inicio, estadoId: Number(estadoId), notas, file };
      },
    });

    if (!formValues) return;

    const { planId, inicio, estadoId, notas, file } = formValues;

    // 4️⃣ Construir FormData para enviar al backend
    const formData = new FormData();
    formData.append("SocioId", socio.id.toString());
    formData.append("PlanId", planId.toString());
    formData.append("EstadoId", estadoId.toString());
    formData.append("Notas", notas || "");

    // ✅ Fecha en formato ISO para ASP.NET (importante)
    const fechaInicioISO = new Date(inicio).toISOString();
    formData.append("FechaInicio", fechaInicioISO);

    if (file) {
      formData.append("file", file);
    }

    // 5️⃣ Enviar al endpoint correcto (una sola request)
    const { data: orden } = await gymApi.post("/ordenes", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    // 6️⃣ Mostrar confirmación
    const plan = planes.find((p: any) => p.id === planId);
    const estado = estados.find((e: any) => e.id === estadoId);
    const comprobanteUrl = orden?.comprobanteId ? `uploads/comprobantes/${orden.comprobanteId}` : null;

    Swal.fire({
      icon: "success",
      title: "Orden creada correctamente",
      html: `
        <p><strong>Socio:</strong> ${socio.nombre}</p>
        <p><strong>Plan:</strong> ${plan?.nombre}</p>
        <p><strong>Monto:</strong> 💰 $${plan?.precio}</p>
        <p><strong>Estado:</strong> ${estado?.nombre}</p>
        ${
          comprobanteUrl
            ? `<p><strong>Comprobante:</strong> <a href="http://localhost:5144/${comprobanteUrl}" target="_blank">Ver archivo</a></p>`
            : `<p><em>Pago sin comprobante (efectivo)</em></p>`
        }
      `,
      confirmButtonColor: "#ff6600",
    });
  } catch (err) {
    console.error("Error al crear orden:", err);
    Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudo crear la orden de pago.",
      confirmButtonColor: "#d33",
    });
  }
}
