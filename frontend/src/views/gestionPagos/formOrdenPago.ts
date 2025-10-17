import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

/**
 * 💳 Muestra un formulario SweetAlert2 para generar una orden de pago
 * y, si se confirma, permite activar la suscripción automáticamente.
 */
export async function mostrarFormOrdenPago(socioId: number, nombre: string) {
  try {
    // 🔹 1️⃣ Obtener planes disponibles
    const resPlanes = await gymApi.get("/planes");
    const planes = resPlanes.data.items || resPlanes.data;

    // 🔹 2️⃣ Mostrar formulario SweetAlert2
    const { value: formValues } = await Swal.fire({
      title: "💳 Nueva Orden de Pago",
      html: `
        <div class="text-start">
          <p><strong>Socio:</strong> ${nombre}</p>
          <label class="form-label mt-2">Seleccionar plan</label>
          <select id="plan" class="swal2-input">
            ${planes
              .map(
                (p: any) =>
                  `<option value="${p.id}">${p.nombre} - $${p.precio}</option>`
              )
              .join("")}
          </select>
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "✅ Crear orden",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      preConfirm: () => {
        const planId = (document.getElementById("plan") as HTMLSelectElement)?.value;
        if (!planId) {
          Swal.showValidationMessage("Debes seleccionar un plan");
          return null;
        }
        return { planId };
      },
    });

    if (!formValues) return;

    const planId = formValues.planId;
    const planSeleccionado = planes.find((p: any) => p.id == planId);

    // 🔹 3️⃣ Crear la orden de pago (con nombres de campos correctos)
    Swal.fire({
      title: "Creando orden...",
      text: "Por favor, espera un momento.",
      allowOutsideClick: false,
      didOpen: () => Swal.showLoading(),
    });

    await gymApi.post("/ordenes", {
      SocioId: socioId,
      PlanId: planId,
      EstadoId: 1, // 1 = Pendiente
      Monto: planSeleccionado?.precio ?? 0,
    });

    // 🔹 4️⃣ Confirmación inicial
    Swal.close();
    const confirmSuscripcion = await Swal.fire({
      icon: "success",
      title: "Orden generada correctamente",
      text: `La orden para ${nombre} se creó con éxito. ¿Deseas activar la suscripción ahora?`,
      showCancelButton: true,
      confirmButtonText: "Sí, activar suscripción",
      cancelButtonText: "No, más tarde",
    });

    // 🔹 5️⃣ Si confirma → crear la suscripción activa
    if (confirmSuscripcion.isConfirmed) {
      Swal.fire({
        title: "Activando suscripción...",
        allowOutsideClick: false,
        didOpen: () => Swal.showLoading(),
      });

      const hoy = new Date();
      const fin = new Date();
      fin.setMonth(hoy.getMonth() + 1); // +1 mes

      await gymApi.post("/suscripciones", {
        SocioId: socioId,
        PlanId: planId,
        Inicio: hoy.toISOString(),
        Fin: fin.toISOString(),
        Estado: true,
      });

      Swal.fire({
        icon: "success",
        title: "✅ Suscripción activada",
        text: `${nombre} fue suscripto correctamente al plan ${planSeleccionado?.nombre}.`,
        timer: 2500,
        showConfirmButton: false,
      });
    } else {
      Swal.fire({
        icon: "info",
        title: "Orden pendiente",
        text: "Podrás activar la suscripción más adelante desde el panel de órdenes.",
        timer: 2000,
        showConfirmButton: false,
      });
    }
  } catch (error: any) {
    console.error("❌ Error al crear orden o suscripción:", error);
    Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudo crear la orden o la suscripción.",
      confirmButtonColor: "#ff7a00",
    });
  }
}
