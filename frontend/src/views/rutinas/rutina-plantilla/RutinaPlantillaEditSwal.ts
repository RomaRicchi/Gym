import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-ejercicio.css"; // ✅ reutilizamos el mismo estilo global

export async function RutinaPlantillaEditSwal(id: string, onSuccess?: () => void) {
  try {
    const { data } = await gymApi.get(`/rutinasplantilla/${id}`);

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Rutina",
      html: `
        <form class="swal-form-ejercicio">
          <input 
            id="nombre" 
            type="text" 
            placeholder="Nombre de la rutina"
            value="${data.nombre || ""}"
          >
          <textarea 
            id="objetivo" 
            rows="3"
            placeholder="Objetivo o descripción general">${data.objetivo || ""}</textarea>
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
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
        const objetivo = (document.getElementById("objetivo") as HTMLTextAreaElement)?.value.trim();

        if (!nombre) {
          Swal.showValidationMessage("El nombre es obligatorio");
          return false;
        }

        return { nombre, objetivo };
      },
    });

    if (!formValues) return;

    // 🔹 Guardar cambios
    await gymApi.put(`/rutinasplantilla/${id}`, formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Rutina actualizada",
      text: "Los cambios se guardaron correctamente.",
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (error) {
    console.error(error);
    await Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudo cargar o actualizar la rutina.",
    });
  }
}
