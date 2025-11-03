// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

/**
 * @param id ID del usuario
 * @param context "perfil" → edición desde el perfil personal (sin editar estado)
 *                 "admin"  → edición completa desde administración
 * @param onSuccess callback opcional tras guardar
 */
export async function PersonalEditSwal(id: number, context: "perfil" | "admin" = "admin", onSuccess?: () => void) {
  try {
    const { data: personal } = await gymApi.get(`/personal/${id}`);

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Datos Personales",
      html: `
        <form id="form-editar-personal" style="text-align:left;overflow-x:hidden;margin-top:0.5rem;">
          
          <div style="margin-bottom:0.8rem;">
            <label for="nombre" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Nombre</label>
            <input id="nombre" type="text"
              value="${personal.nombre || ""}"
              placeholder="Nombre completo"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="telefono" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Teléfono</label>
            <input id="telefono" type="text"
              value="${personal.telefono || ""}"
              placeholder="Teléfono de contacto"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="direccion" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Dirección</label>
            <input id="direccion" type="text"
              value="${personal.direccion || ""}"
              placeholder="Dirección completa"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="especialidad" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Especialidad</label>
            <input id="especialidad" type="text"
              value="${personal.especialidad || ""}"
              placeholder="Área o especialidad"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
          </div>

          ${
            context === "admin"
              ? `
                <div style="display:flex;align-items:center;gap:0.6rem;margin-top:0.8rem;white-space:nowrap;width:fit-content;">
                  <input id="estado" type="checkbox" ${personal.estado === 1 ? "checked" : ""}
                    style="transform:scale(1.3);accent-color:#ff6600;cursor:pointer;margin:0;">
                  <label for="estado" style="font-weight:600;color:#222;margin:0;line-height:1;">Activo</label>
                </div>
              `
              : `
                <div style="margin-top:0.8rem;">
                  <label style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">Estado</label>
                  <input type="text"
                    value="${personal.estado === 1 ? "Activo" : "Inactivo"}"
                    disabled
                    style="width:100%;background:#eee;color:#555;border:1px solid #ccc;border-radius:6px;
                           padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
                </div>
              `
          }
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      confirmButtonColor: "#ff6b00",

      didOpen: () => {
        const popup = Swal.getPopup();
        if (popup) {
          popup.style.overflowX = "hidden";
          popup.style.maxWidth = "520px";
          popup.style.textAlign = "left";
        }

        const htmlContainer = popup?.querySelector(".swal2-html-container") as HTMLElement;
        if (htmlContainer) {
          htmlContainer.style.width = "100%";
          htmlContainer.style.maxWidth = "none";
          htmlContainer.style.display = "block";
          htmlContainer.style.textAlign = "left";
        }

        const form = document.getElementById("form-editar-personal") as HTMLElement;
        if (form) {
          form.style.width = "100%";
          form.style.maxWidth = "480px";
          form.style.display = "block";
        }

        // Ajustar inputs
        document
          .querySelectorAll<HTMLInputElement>(
            "#form-editar-personal input"
          )
          .forEach((el) => {
            el.classList.remove("swal2-input");
            el.style.width = "100%";
            el.style.margin = "0.3rem 0";
            el.style.display = "block";
            el.style.boxSizing = "border-box";
            el.style.maxWidth = "none";
            el.style.fontSize = "1rem";
            el.style.padding = "0.7rem 1rem";
          });
      },

      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement)?.value.trim();
        const telefono = (document.getElementById("telefono") as HTMLInputElement)?.value.trim();
        const direccion = (document.getElementById("direccion") as HTMLInputElement)?.value.trim();
        const especialidad = (document.getElementById("especialidad") as HTMLInputElement)?.value.trim();

        // Si se abre desde perfil, no permitir editar estado
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
      confirmButtonColor: "#ff6b00",
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err) {
    console.error(err);
    Swal.fire("❌ Error", "No se pudo actualizar la información del personal", "error");
  }
}
