import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function PersonalCreateSwal(onSuccess?: () => void) {
  const { value: formValues } = await Swal.fire({
    title: "➕ Nuevo Personal",
    html: `
      <div class="swal2-card-style">
        <div class="row mb-3">
          <div class="col-md-6">
            <label class="form-label">Nombre</label>
            <input id="nombre" type="text" class="form-control" placeholder="Nombre completo" required />
          </div>
          <div class="col-md-6">
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
            <input id="rol" type="text" class="form-control" placeholder="Ej: Profesor, Recepción" />
          </div>
        </div>
        <div class="form-check mb-3 text-start">
          <input id="activo" type="checkbox" class="form-check-input" checked />
          <label class="form-check-label" for="activo">Activo</label>
        </div>
      </div>
    `,
    showCancelButton: true,
    confirmButtonText: "Guardar",
    preConfirm: () => {
      const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
      const email = (document.getElementById("email") as HTMLInputElement).value.trim();
      const telefono = (document.getElementById("telefono") as HTMLInputElement).value.trim();
      const rol = (document.getElementById("rol") as HTMLInputElement).value.trim();
      const activo = (document.getElementById("activo") as HTMLInputElement).checked;

      if (!nombre || !email) {
        Swal.showValidationMessage("Nombre y correo son obligatorios");
        return false;
      }

      return { nombre, email, telefono, rol, activo };
    },
  });

  if (formValues) {
    try {
      await gymApi.post("/personal", formValues);
      Swal.fire("Guardado", "Personal creado correctamente", "success");
      onSuccess?.();
    } catch {
      Swal.fire("Error", "No se pudo crear el registro", "error");
    }
  }
}
