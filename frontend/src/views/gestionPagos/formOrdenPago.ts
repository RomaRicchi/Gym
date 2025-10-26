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

    // 5️⃣ Enviar al endpoint correcto
    const { data: orden } = await gymApi.post("/ordenes", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    // 6️⃣ Mostrar confirmación
    const plan = planes.find((p: any) => p.id === planId);
    const estado = estados.find((e: any) => e.id === estadoId);
    const comprobanteUrl = orden?.comprobante?.fileUrl || null;
    const baseUrl = import.meta.env.VITE_API_URL?.replace("/api", "") || "http://localhost:5144";
    const fullUrl = comprobanteUrl ? `${baseUrl}/${comprobanteUrl}` : null;

    // 🔹 Generar HTML dinámico
    let contenidoHtml = `
      <p><strong>Socio:</strong> ${socio.nombre}</p>
      <p><strong>Plan:</strong> ${plan?.nombre}</p>
      <p><strong>Monto:</strong> 💰 $${plan?.precio}</p>
      <p><strong>Estado:</strong> ${estado?.nombre}</p>
    `;

    if (fullUrl) {
      if (fullUrl.toLowerCase().endsWith(".pdf")) {
        contenidoHtml += `
          <hr>
          <p><strong>Comprobante (PDF):</strong></p>
          <iframe src="${fullUrl}" width="100%" height="400px"></iframe>
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

    // 🔹 Mostrar Swal final
    await Swal.fire({
      icon: "success",
      title: "Orden creada correctamente",
      html: contenidoHtml,
      width: fullUrl ? "80%" : "40%",
      confirmButtonColor: "#ff6600",
      confirmButtonText: "Cerrar",
    });

    // ✅ Actualizar plan del socio y mostrar notificación
    try {
      const { data: subs } = await gymApi.get(`/suscripciones?socioId=${socio.id}`);
      const ultima = subs?.length ? subs[0] : null;

      if (ultima?.plan?.nombre) {
        const fila = document.querySelector(`tr td:nth-child(2):contains("${socio.nombre}")`)?.parentElement;
        const celdaPlan = fila?.querySelector("td:nth-child(5)");
        if (celdaPlan) celdaPlan.textContent = ultima.plan.nombre;
      }

      // 🔔 Mini toast de confirmación
      Swal.fire({
        toast: true,
        position: "top-end",
        icon: "success",
        title: "Suscripción activa creada",
        showConfirmButton: false,
        timer: 2500,
        timerProgressBar: true,
        background: "#198754",
        color: "#fff",
        didOpen: (toast) => {
          toast.style.opacity = "0";
          setTimeout(() => (toast.style.opacity = "1"), 100);
          setTimeout(() => (toast.style.opacity = "0"), 2300);
        },
      });
    } catch (e) {
      console.warn("⚠️ No se pudo actualizar la vista del plan del socio:", e);
    }
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
