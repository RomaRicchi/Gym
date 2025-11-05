import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/orden.css";

export async function editarOrden(id: number) {
  try {
    // 1️⃣ Obtener los estados disponibles
    const { data: estados } = await gymApi.get("/estadoOrdenPago");

    if (!Array.isArray(estados) || estados.length === 0) {
      await Swal.fire({
        icon: "info",
        title: "Sin estados disponibles",
        text: "No se encontraron estados de orden de pago.",
        customClass: { popup: "swal2-card-style" },
      });
      return false;
    }

    // 2️⃣ Armar opciones del select
    const opciones: Record<string, string> = {};
    estados.forEach((e: any) => {
      opciones[String(e.id)] = e.nombre;
    });

    // 3️⃣ Mostrar selector de estado
    const { value: nuevoEstadoId } = await Swal.fire({
      title:
        '<h2 class="fw-bold mb-3" style="font-size:1.6rem"><i class="fa-solid fa-pen me-2"></i>Cambiar estado</h2>',
      input: "select",
      inputOptions: opciones,
      inputPlaceholder: "Seleccione un estado",
      showCancelButton: true,
      confirmButtonText: "Guardar",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      buttonsStyling: false,
      inputValidator: (value) => (!value ? "Debe seleccionar un estado" : null),
    });

    if (!nuevoEstadoId) return false;

    // 4️⃣ Actualizar estado de la orden
    await gymApi.put(`/ordenes/${id}/estado/simple`, {
      estadoId: Number(nuevoEstadoId),
    });

    // 5️⃣ Mostrar confirmación
    await Swal.fire({
      icon: "success",
      title: "✅ Estado actualizado",
      text: "El estado de la orden fue actualizado correctamente.",
      timer: 1500,
      showConfirmButton: false,
      customClass: { popup: "swal2-card-style swal-alert-simple" },
    });

    // 6️⃣ Verificar si el nuevo estado es ID = 3 (Verificado)
    const estadoSeleccionado = estados.find((e: any) => e.id === Number(nuevoEstadoId));
    console.log("🔍 Estado seleccionado:", estadoSeleccionado);

    if (estadoSeleccionado && estadoSeleccionado.id === 3) {
      console.log("✅ Estado es VERIFICADO (id=3), creando suscripción automática...");

      try {
        const { data: resultado } = await gymApi.post(`/suscripciones/crear-por-orden/${id}`);

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
            didOpen: (toast) => {
              toast.style.opacity = "0";
              setTimeout(() => (toast.style.opacity = "1"), 100);
              setTimeout(() => (toast.style.opacity = "0"), 2300);
            },
          });
        } else {
          console.warn("⚠️ No se creó la suscripción automáticamente:", resultado);
        }
      } catch (err) {
        console.error("❌ Error al crear suscripción automática:", err);
      }
    }

    return true;
  } catch (err) {
    console.error(err);
    await Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudo actualizar el estado de la orden.",
      customClass: { popup: "swal2-card-style swal-alert-simple" },
    });
    return false;
  }
}
