// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-ejercicio.css"; 

/**
 * @param id ID del usuario
 * @param context "perfil" → edición desde el perfil personal (sin editar estado)
 *                 "admin"  → edición completa desde administración
 * @param onSuccess callback opcional tras guardar
 */
export async function PersonalEditSwal(id: number| string, context: "perfil" | "admin" = "admin", onSuccess?: () => void) {
  try {
    const { data: personal } = await gymApi.get(`/personal/${id}`);

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Datos Personales",
      html: `
        <form class="swal-form-ejercicio">
          <input 
            id="nombre" 
            type="text"
            placeholder="Nombre completo"
            value="${personal.nombre || ""}"
          >
          <input 
            id="telefono" 
            type="text"
            placeholder="Teléfono de contacto"
            value="${personal.telefono || ""}"
          >
          <input 
            id="direccion" 
            type="text"
            placeholder="Dirección completa"
            value="${personal.direccion || ""}"
          >
          <input 
            id="especialidad" 
            type="text"
            placeholder="Área o especialidad"
            value="${personal.especialidad || ""}"
          >

          ${
            context === "admin"
              ? `
              <div class="checkbox-group">
                <input 
                  type="checkbox" 
                  id="estado" 
                  class="swal-checkbox"
                  ${personal.estado === 1 ? "checked" : ""}
                >
                <label for="estado" class="swal-label">Activo</label>
              </div>
            `
              : `
              <div class="swal-input-group">
                <label class="swal-label">Estado</label>
                <input 
                  type="text"
                  value="${personal.estado === 1 ? "Activo" : "Inactivo"}"
                  disabled
                  style="background:#eee;color:#555;"
                >
              </div>
            `
          }
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
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
        const telefono = (document.getElementById("telefono") as HTMLInputElement)?.value.trim();
        const direccion = (document.getElementById("direccion") as HTMLInputElement)?.value.trim();
        const especialidad = (document.getElementById("especialidad") as HTMLInputElement)?.value.trim();

        const estadoInput = document.getElementById("estado") as HTMLInputElement;
        const estado = context === "admin" ? (estadoInput?.checked ? 1 : 0) : personal.estado;

        if (!nombre) {
          Swal.showValidationMessage("El nombre es obligatorio");
          return;
        }

        return { nombre, telefono, direccion, especialidad, estado };
      },
    });

    if (!formValues) return;

    await gymApi.put(`/personal/${id}`, formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Datos actualizados",
      text: "Los cambios fueron guardados correctamente.",
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err) {
    console.error(err);
    await Swal.fire({
      icon: "error",
      title: "❌ Error",
      text: "No se pudo actualizar la información del personal.",
    });
  }
}
