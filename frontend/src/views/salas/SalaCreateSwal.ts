import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-ejercicio.css"; // ✅ usamos el mismo estilo naranja global

export async function SalaCreateSwal(onSuccess?: () => void) {
  const { value: formValues } = await Swal.fire({
    title: "➕ Nueva Sala",
    html: `
      <form class="swal-form-ejercicio">
        <input 
          id="nombre" 
          type="text" 
          placeholder="Ej: Sala de pesas"
        >
        <input 
          id="cupo" 
          type="number" 
          min="1" 
          placeholder="Cupo máximo (ej: 10)"
        >
        <div class="checkbox-group">
          <input 
            type="checkbox" 
            id="activa" 
            class="swal-checkbox" 
            checked
          >
          <label for="activa" class="swal-label">Activa</label>
        </div>
      </form>
    `,
    showCancelButton: true,
    confirmButtonText: "💾 Guardar",
    cancelButtonText: "Cancelar",
    focusConfirm: false,
    customClass: {
      popup: "swal2-card-ejercicio",      // 🧡 fondo naranja suave
      confirmButton: "btn btn-orange",    // 🟠 botón principal
      cancelButton: "btn btn-secondary",  // ⚪ botón secundario
    },
    buttonsStyling: false,

    preConfirm: () => {
      const nombre = (document.getElementById("nombre") as HTMLInputElement)?.value.trim();
      const cupoStr = (document.getElementById("cupo") as HTMLInputElement)?.value;
      const activa = (document.getElementById("activa") as HTMLInputElement)?.checked;

      if (!nombre) {
        Swal.showValidationMessage("El nombre es obligatorio");
        return null;
      }

      const cupo = Number(cupoStr);
      if (!Number.isFinite(cupo) || cupo < 1) {
        Swal.showValidationMessage("El cupo debe ser un número mayor a 0");
        return null;
      }

      return { nombre, cupo, activa };
    },
  });

  if (!formValues) return;

  try {
    await gymApi.post("/salas", formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Sala creada",
      text: "La sala fue registrada correctamente.",
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err: any) {
    const message =
      err.response?.data?.message ||
      err.response?.data?.error ||
      "No se pudo crear la sala";
    await Swal.fire({
      icon: "error",
      title: "❌ Error",
      text: message,
    });
  }
}
