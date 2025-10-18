import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function PersonalEditSwal(id: string, onSuccess?: () => void) {
  try {
    // 🔹 Cargar datos del personal y roles disponibles
    const [resPersonal, resRoles] = await Promise.all([
      gymApi.get(`/personal/${id}`),
      gymApi.get("/roles"),
    ]);

    const data = resPersonal.data;
    const roles = resRoles.data.items || resRoles.data;

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Personal",
      html: `
        <div class="swal2-card-style">
          <div class="row mb-3">
            <div class="col-md-12">
              <label class="form-label">Nombre</label>
              <input id="nombre" type="text" class="form-control" 
                     value="${data.nombre || ""}" required />
            </div>
            <div class="col-md-12">
              <label class="form-label">Email</label>
              <input id="email" type="email" class="form-control" 
                     value="${data.email || ""}" required />
            </div>
          </div>

          <div class="row mb-3">
            <div class="col-md-6">
              <label class="form-label">Teléfono</label>
              <input id="telefono" type="text" class="form-control" 
                     value="${data.telefono || ""}" />
            </div>
            <div class="col-md-6">
              <label class="form-label">Rol</label>
              <select id="rol" class="form-select">
                <option value="">Seleccionar rol...</option>
                ${roles
                  .map(
                    (r: any) =>
                      `<option value="${r.id}" ${
                        r.id === data.rolId ? "selected" : ""
                      }>${r.nombre}</option>`
                  )
                  .join("")}
              </select>
            </div>
          </div>

          <div class="form-check mb-3 text-start">
            <input id="activo" type="checkbox" class="form-check-input" ${
              data.activo ? "checked" : ""
            } />
            <label class="form-check-label" for="activo">Activo</label>
          </div>
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,

      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
        const email = (document.getElementById("email") as HTMLInputElement).value.trim();
        const telefono = (document.getElementById("telefono") as HTMLInputElement).value.trim();
        const rol_id = (document.getElementById("rol") as HTMLSelectElement).value;
        const activo = (document.getElementById("activo") as HTMLInputElement).checked;

        if (!nombre || !email || !rol_id) {
          Swal.showValidationMessage("Nombre, correo y rol son obligatorios");
          return false;
        }

        return { nombre, email, telefono, rol_id: Number(rol_id), activo };
      },

      didOpen: () => {
        const popup = Swal.getPopup();
        if (popup) {
          popup.style.overflowX = "hidden";
          popup.style.maxWidth = "520px";
          popup.style.textAlign = "left";
        }

        document
          .querySelectorAll<HTMLInputElement | HTMLSelectElement>(
            "#nombre, #email, #telefono, #rol, #activo"
          )
          .forEach((input) => input.classList.remove("swal2-input"));
      },
    });

    // 🔹 Enviar actualización
    if (formValues) {
      await gymApi.put(`/personal/${id}`, formValues);
      await Swal.fire("✅ Actualizado", "Registro modificado correctamente", "success");
      onSuccess?.();
    }
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudo cargar o actualizar el registro", "error");
  }
}
