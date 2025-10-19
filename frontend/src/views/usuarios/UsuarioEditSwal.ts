import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function UsuarioEditSwal(id: number | string, onSuccess?: () => void) {
  try {
    // 1️⃣ Obtener usuario y roles
    const [resUsuario, resRoles] = await Promise.all([
      gymApi.get(`/usuarios/${id}`),
      gymApi.get("/roles"),
    ]);

    const usuario = resUsuario.data;
    const roles = resRoles.data.items || resRoles.data;

    // 2️⃣ Mostrar formulario
    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Usuario",
      html: `
        <div class="swal2-card-style text-start">
          <div class="mb-3">
            <label class="form-label">Email</label>
            <input id="email" type="email" class="form-control" 
              value="${usuario.email || ""}" placeholder="correo@ejemplo.com" required />
          </div>

          <div class="mb-3">
            <label class="form-label">Alias</label>
            <input id="alias" type="text" class="form-control"
              value="${usuario.alias || ""}" placeholder="Nombre de usuario" required />
          </div>

          <div class="mb-3">
            <label class="form-label">Rol</label>
            <select id="rol_id" class="form-select">
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

          <div class="form-check mt-3">
            <input id="estado" type="checkbox" class="form-check-input" ${
              usuario.estado ? "checked" : ""
            } />
            <label class="form-check-label" for="estado">Activo</label>
          </div>
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,

      preConfirm: () => {
        const email = (document.getElementById("email") as HTMLInputElement).value.trim();
        const alias = (document.getElementById("alias") as HTMLInputElement).value.trim();
        const rolSelect = document.getElementById("rol_id") as HTMLSelectElement;
        const rolId = parseInt(rolSelect.value);
        const estado = (document.getElementById("estado") as HTMLInputElement).checked;

        if (!email || !alias || !rolId) {
          Swal.showValidationMessage("Debe completar todos los campos requeridos");
          return false;
        }

        return { email, alias, rolId, estado }; // 👈 Usa camelCase correcto
      },
    });

    if (!formValues) return;

    // 3️⃣ Enviar actualización
    await gymApi.put(`/usuarios/${id}`, formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Usuario actualizado",
      text: "Los datos del usuario se modificaron correctamente.",
      timer: 1500,
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
