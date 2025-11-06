import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-usuario.css"; // ✅ Importá este CSS

export async function UsuarioEditSwal(id: number | string, onSuccess?: () => void) {
  try {
    const [resUsuario, resRoles] = await Promise.all([
      gymApi.get(`/usuarios/${id}`),
      gymApi.get("/roles"),
    ]);

    const usuario = resUsuario.data;
    const roles = resRoles.data.items || resRoles.data;

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Usuario",
      customClass: {
        popup: "swal2-card-usuario",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      buttonsStyling: false,
      html: `
        <form class="swal-form-usuario">
          <div>
            <label class="swal-label">Correo electrónico</label>
            <input id="email" type="email" value="${usuario.email || ""}" placeholder="correo@ejemplo.com">
          </div>

          <div>
            <label class="swal-label">Alias</label>
            <input id="alias" type="text" value="${usuario.alias || ""}" placeholder="Nombre de usuario">
          </div>

          <div>
            <label class="swal-label">Rol</label>
            <select id="rol_id">
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

          <div class="checkbox-group">
            <input id="estado" type="checkbox" class="swal-checkbox" ${
              usuario.estado ? "checked" : ""
            }>
            <label for="estado" class="swal-label" style="margin-left:4px;">Activo</label>
          </div>

        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
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
      customClass: { popup: "swal2-card-usuario" },
    });

    onSuccess?.();
  } catch (err: any) {
    console.error(err);
    Swal.fire({
      icon: "error",
      title: "❌ Error",
      text: err.response?.data?.message || "No se pudo actualizar el usuario.",
      customClass: { popup: "swal2-card-usuario" },
    });
  }
}
