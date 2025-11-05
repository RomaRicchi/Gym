import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-ejercicio.css"; // ✅ importante

export async function EjercicioEditSwal(id: string, onSuccess?: () => void) {
  try {
    // 🔹 Cargar datos del ejercicio
    const { data: ejercicio } = await gymApi.get(`/ejercicios/${id}`);

    // 🔹 Modal con estilo unificado
    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Ejercicio",
      html: `
        <form class="swal-form-ejercicio">
          <input 
            id="nombre" 
            type="text" 
            placeholder="Nombre del ejercicio" 
            value="${ejercicio.nombre || ""}"
          >
          <input 
            id="grupo" 
            type="text" 
            placeholder="Grupo muscular (ej: Core, Piernas...)" 
            value="${ejercicio.grupo || ""}"
          >
          <textarea 
            id="tips" 
            rows="3" 
            placeholder="Consejos o indicaciones">${ejercicio.tips || ""}</textarea>
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      customClass: {
        popup: "swal2-card-ejercicio",    // 🔸 usa el fondo naranja suave
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

    // 🔹 Guardar los cambios
    await gymApi.put(`/ejercicios/${id}`, formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Ejercicio actualizado",
      text: "Los cambios se guardaron correctamente.",
      timer: 1600,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (error) {
    console.error(error);
    Swal.fire("Error", "No se pudo cargar o guardar el ejercicio", "error");
  }
}
