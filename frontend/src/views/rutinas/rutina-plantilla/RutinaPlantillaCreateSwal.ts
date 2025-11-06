import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-rutina.css";

export async function RutinaPlantillaCreateSwal(onSuccess?: () => void) {
  const { value: formValues } = await Swal.fire({
    title: "➕ Nueva Rutina",
    html: `
      <form class="swal-form-ejercicio">
        <input 
          id="nombre" 
          type="text" 
          placeholder="Nombre de la rutina"
        >
        <textarea 
          id="objetivo" 
          rows="3" 
          placeholder="Objetivo o enfoque (opcional)"></textarea>
      </form>
    `,
    showCancelButton: true,
    confirmButtonText: "💾 Guardar",
    cancelButtonText: "Cancelar",
    focusConfirm: false,
    customClass: {
      popup: "swal2-card-ejercicio",       // 🧡 fondo naranja suave
      confirmButton: "btn btn-orange",     // 🟠 botón principal
      cancelButton: "btn btn-secondary",   // ⚪ botón secundario
    },
    buttonsStyling: false,

    preConfirm: () => {
      const nombre = (document.getElementById("nombre") as HTMLInputElement)?.value.trim();
      const objetivo = (document.getElementById("objetivo") as HTMLTextAreaElement)?.value.trim();

      if (!nombre) {
        Swal.showValidationMessage("El nombre de la rutina es obligatorio");
        return false;
      }

      return { nombre, objetivo };
    },
  });

  if (!formValues) return;

  try {
    await gymApi.post("/rutinasplantilla", formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Rutina creada",
      text: "La rutina fue registrada correctamente.",
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (error) {
    console.error(error);
    await Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudo crear la rutina.",
    });
  }
}
