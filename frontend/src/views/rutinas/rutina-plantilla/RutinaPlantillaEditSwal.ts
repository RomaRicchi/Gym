// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-rutina.css";

interface RutinaForm {
  nombre: string;
  objetivo: string;
}

export async function RutinaPlantillaEditSwal(id: string, onSuccess?: () => void) {
  try {
    // 🔹 Obtener datos existentes
    const { data: rutina } = await gymApi.get(`/rutinasplantilla/${id}`);

    const { value: formValues } = await Swal.fire<RutinaForm>({
      title: "✏️ Editar Rutina Plantilla",
      width: 600,
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      html: `
        <form id="form-editar-rutina" class="swal-form">
          <div class="swal-input-group">
            <label for="nombre" class="swal-label">Nombre de la rutina</label>
            <input id="nombre" type="text" class="swal2-input" placeholder="Ej: Full Body Intermedio"
              value="${rutina.nombre || ""}" />
          </div>

          <div class="swal-input-group" style="margin-top:0.8rem;">
            <label for="objetivo" class="swal-label">Objetivo</label>
            <textarea id="objetivo" rows="3" class="swal2-textarea" placeholder="Ej: Aumentar fuerza y resistencia...">${rutina.objetivo || ""}</textarea>
          </div>
        </form>
      `,
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement)?.value.trim();
        const objetivo = (document.getElementById("objetivo") as HTMLTextAreaElement)?.value.trim();

        if (!nombre) {
          Swal.showValidationMessage("El nombre es obligatorio");
          return;
        }
        return { nombre, objetivo };
      },
    });

    if (!formValues) return;

    // 🔹 Enviar actualización con ID incluido
    await gymApi.put(`/rutinasplantilla/${id}`, {
      id: Number(id),
      nombre: formValues.nombre.trim(),
      objetivo: formValues.objetivo?.trim() || null,
    });

    // 🔹 Confirmación visual
    await Swal.fire({
      icon: "success",
      title: "✅ Rutina actualizada",
      text: "Los cambios se guardaron correctamente.",
      timer: 1600,
      showConfirmButton: false,
      customClass: {
        popup: "swal2-card-style",
      },
    });

    if (onSuccess) onSuccess();
  } catch (error) {
    console.error("Error al editar rutina:", error);
    await Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudo editar la rutina. Verificá los datos o el servidor.",
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange",
      },
    });
  }
}
