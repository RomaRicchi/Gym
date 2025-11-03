import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface UsuarioForm {
  email: string;
  alias: string;
  rolId: string;
  estado: boolean;
}

export async function UsuarioEditSwal(id: number | string, onSuccess?: () => void) {
  try {
    const [resUsuario, resRoles] = await Promise.all([
      gymApi.get(`/usuarios/${id}`),
      gymApi.get("/roles"),
    ]);

    const usuario = resUsuario.data;
    const roles = resRoles.data.items || resRoles.data;

    const { value: formValues } = await Swal.fire<UsuarioForm>({
      title: "✏️ Editar Usuario",
      html: `
        <form id="form-editar-usuario" style="text-align:left;overflow-x:hidden;margin-top:0.5rem;">

          <div style="margin-bottom:0.8rem;">
            <label for="email" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">
              Correo electrónico
            </label>
            <input id="email" type="email"
              value="${usuario.email || ""}"
              placeholder="correo@ejemplo.com"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="alias" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">
              Alias
            </label>
            <input id="alias" type="text"
              value="${usuario.alias || ""}"
              placeholder="Nombre de usuario"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
          </div>

          <div style="margin-bottom:0.8rem;">
            <label for="rol_id" style="display:block;font-weight:600;color:#222;margin-bottom:0.3rem;">
              Rol
            </label>
            <select id="rol_id"
              style="width:100%;background:#fff;color:#222;border:1px solid #ccc;border-radius:6px;
                     padding:0.7rem 1rem;font-size:1rem;box-sizing:border-box;">
              <option value="">Seleccionar rol...</option>
              ${roles
                .map(
                  (r: any) =>
                    `<option value="${r.id}" ${
                      usuario.rolId === r.id || usuario.rol_id === r.id ? "selected" : ""
                    }>${r.nombre}</option>`
                )
                .join("")}
            </select>
          </div>

          <div
            style="display:flex;align-items:center;gap:0.6rem;margin-top:0.8rem;
                   white-space:nowrap;width:fit-content;">
            <input id="estado" type="checkbox" ${usuario.estado ? "checked" : ""}
              style="transform:scale(1.3);accent-color:#ff6600;cursor:pointer;margin:0;">
            <label for="estado" style="font-weight:600;color:#222;margin:0;line-height:1;">
              Activo
            </label>
          </div>
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,

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

        const form = document.getElementById("form-editar-usuario") as HTMLElement;
        if (form) {
          form.style.width = "100%";
          form.style.maxWidth = "480px";
          form.style.display = "block";
        }

        document
          .querySelectorAll<HTMLInputElement | HTMLSelectElement>(
            "#form-editar-usuario input, #form-editar-usuario select"
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
        const email = (document.getElementById("email") as HTMLInputElement)?.value.trim();
        const alias = (document.getElementById("alias") as HTMLInputElement)?.value.trim();
        const rolId = (document.getElementById("rol_id") as HTMLSelectElement)?.value;
        const estado = (document.getElementById("estado") as HTMLInputElement)?.checked ?? false;

        if (!email || !alias || !rolId) {
          Swal.showValidationMessage("Todos los campos son obligatorios");
          return;
        }

        return { email, alias, rolId, estado };
      },
    });

    if (!formValues) return;

    await gymApi.put(`/usuarios/${id}`, formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Usuario actualizado",
      text: "Los datos fueron guardados correctamente.",
      timer: 1600,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err: any) {
    console.error(err);
    Swal.fire(
      "Error",
      err.response?.data?.message || "No se pudo cargar o actualizar el usuario",
      "error"
    );
  }
}
