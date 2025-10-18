import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function PersonalCreateSwal(onSuccess?: () => void) {
  try {
    // 🔹 Cargar roles disponibles desde el backend
    const resRoles = await gymApi.get("/roles");
    const roles = resRoles.data.items || resRoles.data;

    const { value: formValues } = await Swal.fire({
      title: "➕ Nuevo Personal",
      html: `
        <div class="swal2-card-style">
          <div class="row mb-3">
            <div class="col-md-12">
              <label class="form-label">Nombre</label>
              <input id="nombre" type="text" class="form-control" placeholder="Nombre completo" required />
            </div>
            <div class="col-md-12">
              <label class="form-label">Email</label>
              <input id="email" type="email" class="form-control" placeholder="correo@ejemplo.com" required />
            </div>
          </div>

          <div class="row mb-3">
            <div class="col-md-6">
              <label class="form-label">Teléfono</label>
              <input id="telefono" type="text" class="form-control" placeholder="Ej: 2664123456" />
            </div>
            <div class="col-md-6">
              <label class="form-label">Rol</label>
              <select id="rol" class="form-select">
                <option value="">Seleccionar rol...</option>
                ${roles
                  .map((r: any) => `<option value="${r.id}">${r.nombre}</option>`)
                  .join("")}
              </select>
            </div>
          </div>

          <div class="row mb-3">
            <div class="col-md-12">
              <label class="form-label">Especialidad</label>
              <input id="especialidad" type="text" class="form-control" placeholder="Ej: Crossfit, Yoga, Pilates" />
            </div>
          </div>

          <div class="form-check mb-3 text-start">
            <input id="activo" type="checkbox" class="form-check-input" checked />
            <label class="form-check-label" for="activo">Activo</label>
          </div>
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar",
      cancelButtonText: "Cancelar",
      focusConfirm: false,

      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
        const email = (document.getElementById("email") as HTMLInputElement).value.trim();
        const telefono = (document.getElementById("telefono") as HTMLInputElement).value.trim();
        const especialidad = (document.getElementById("especialidad") as HTMLInputElement).value.trim();
        const rol_id = (document.getElementById("rol") as HTMLSelectElement).value;
        const activo = (document.getElementById("activo") as HTMLInputElement).checked;

        if (!nombre || !email || !rol_id) {
          Swal.showValidationMessage("Nombre, correo y rol son obligatorios");
          return false;
        }

        return { nombre, email, telefono, especialidad, rol_id: Number(rol_id), activo };
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
            "#nombre, #email, #telefono, #especialidad, #rol, #activo"
          )
          .forEach((input) => input.classList.remove("swal2-input"));
      },
    });

    // 🔹 Enviar al backend si el usuario completó el formulario
    if (formValues) {
      await gymApi.post("/personal", formValues);
      await Swal.fire({
        icon: "success",
        title: "✅ Personal creado",
        text: "El registro se creó correctamente.",
        timer: 1600,
        showConfirmButton: false,
      });
      onSuccess?.();
    }
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudo crear el registro. Verifique los datos.", "error");
  }
}
