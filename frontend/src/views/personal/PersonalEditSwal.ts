import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function PersonalEditSwal(id: string, onSuccess?: () => void) {
  try {
    const res = await gymApi.get(`/personal/${id}`);
    const data = res.data;

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Personal",
      html: `
        <div class="swal2-card-style">
          <div class="row mb-3">
            <div class="col-md-6">
              <label class="form-label">Nombre</label>
              <input id="nombre" type="text" class="form-control" value="${data.nombre || ""}" required />
            </div>
            <div class="col-md-6">
              <label class="form-label">Email</label>
              <input id="email" type="email" class="form-control" value="${data.email || ""}" required />
            </div>
          </div>
          <div class="row mb-3">
            <div class="col-md-6">
              <label class="form-label">Teléfono</label>
              <input id="telefono" type="text" class="form-control" value="${data.telefono || ""}" />
            </div>
            <div class="col-md-6">
              <label class="form-label">Rol</label>
              <input id="rol" type="text" class="form-control" value="${data.rol || ""}" />
            </div>
          </div>
          <div class="form-check mb-3 text-start">
            <input id="activo" type="checkbox" class="form-check-input" ${data.activo ? "checked" : ""} />
            <label class="form-check-label" for="activo">Activo</label>
          </div>
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "Guardar cambios",
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
      await gymApi.put(`/personal/${id}`, formValues);
      Swal.fire("Actualizado", "Registro modificado correctamente", "success");
      onSuccess?.();
    }
  } catch {
    Swal.fire("Error", "No se pudo cargar o actualizar el registro", "error");
  }
}
