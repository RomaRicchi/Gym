import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-ejercicio.css"; 

export async function EjercicioCreateSwal(onSuccess?: () => void) {
  const { value: formValues } = await Swal.fire({
    title: "➕ Nuevo Ejercicio",
    html: `
      <form class="swal-form-ejercicio">
        <input 
          id="nombre" 
          type="text" 
          placeholder="Nombre del ejercicio"
        >
        <input 
          id="grupo" 
          type="text" 
          placeholder="Grupo muscular (piernas, pecho, etc.)"
        >
        <textarea 
          id="tips" 
          rows="3" 
          placeholder="Consejos o tips de ejecución"></textarea>
      </form>
    `,
    showCancelButton: true,
    confirmButtonText: "💾 Guardar",
    cancelButtonText: "Cancelar",
    focusConfirm: false,
    customClass: {
      popup: "swal2-card-ejercicio",
      confirmButton: "btn btn-orange",
      cancelButton: "btn btn-secondary",
    },
    buttonsStyling: false,

    preConfirm: () => {
      const nombre = (document.getElementById("nombre") as HTMLInputElement)?.value.trim();
      const grupo = (document.getElementById("grupo") as HTMLInputElement)?.value.trim();
      const tips = (document.getElementById("tips") as HTMLTextAreaElement)?.value.trim();

      if (!nombre || !grupo) {
        Swal.showValidationMessage("Nombre y grupo son obligatorios");
        return false;
      }

      return { nombre, grupo, tips };
    },
  });

  if (!formValues) return;

  try {
    await gymApi.post("/ejercicios", formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Ejercicio creado",
      text: "El ejercicio fue registrado correctamente.",
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (error) {
    console.error(error);
    await Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudo crear el ejercicio.",
    });
  }
}
