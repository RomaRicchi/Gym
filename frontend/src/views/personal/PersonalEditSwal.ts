import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

/**
 * 🧩 Modal reutilizable para editar datos personales o de usuario
 * @param id ID del personal o usuario logueado
 * @param modo "admin" → usa /personal y permite roles/usuarios, "perfil" → usa /perfil y solo datos personales
 * @param onSuccess función callback a ejecutar tras guardado exitoso
 */
export async function PersonalEditSwal(
  id: string,
  modo: "admin" | "perfil" = "admin",
  onSuccess?: () => void
) {
  try {
    // 🔹 Cargar datos del personal y roles (solo si es modo admin)
    const [resPersonal, resRoles] = await Promise.all([
      gymApi.get(modo === "perfil" ? `/perfil/${id}` : `/personal/${id}`),
      modo === "admin" ? gymApi.get("/roles") : Promise.resolve({ data: [] }),
    ]);

    const data = resPersonal.data;
    const roles = resRoles.data.items || resRoles.data;

    const personal = modo === "perfil" ? data.personal ?? data : data;
    const tieneUsuario = modo === "admin" && !!data.email;

    // 🧾 Modal principal
    const { value: formValues } = await Swal.fire({
      title: modo === "perfil" ? "✏️ Editar Perfil" : "✏️ Editar Personal",
      html: `
        <div class="swal2-card-style">
          <h6 class="text-start mb-2 fw-bold text-black">Datos personales</h6>

          <div class="row mb-3">
            <div class="col-md-12">
              <label class="form-label">Nombre</label>
              <input id="nombre" type="text" class="form-control"
                value="${personal.nombre || ""}" required />
            </div>
            <div class="col-md-6">
              <label class="form-label">Teléfono</label>
              <input id="telefono" type="text" class="form-control"
                value="${personal.telefono || ""}" />
            </div>
            <div class="col-md-6">
              <label class="form-label">Especialidad</label>
              <input id="especialidad" type="text" class="form-control"
                value="${personal.especialidad || ""}" />
            </div>
            <div class="col-md-12">
              <label class="form-label">Dirección</label>
              <input id="direccion" type="text" class="form-control"
                value="${personal.direccion || ""}" placeholder="Ej: Av. Mitre 1234" />
            </div>
          </div>

          <div class="form-check text-start mb-3">
            <input id="activo" type="checkbox" class="form-check-input" ${
              personal.estado || personal.activo ? "checked" : ""
            } />
            <label class="form-check-label" for="activo">Activo</label>
          </div>

          ${
            modo === "admin"
              ? tieneUsuario
                ? `
                <hr class="my-3" />
                <h6 class="text-start text-success fw-bold">Usuario asociado</h6>
                <div class="mb-2">
                  <p class="mb-0"><strong>Email:</strong> ${data.email}</p>
                  <p class="mb-0"><strong>Rol:</strong> ${data.rol}</p>
                </div>
                <small class="text-muted fst-italic">El usuario ya está vinculado a este personal.</small>
              `
                : `
                <hr class="my-3" />
                <h6 class="text-start text-success fw-bold">Crear usuario (opcional)</h6>
                <div class="row mb-3">
                  <div class="col-md-6">
                    <label class="form-label">Alias</label>
                    <input id="alias" type="text" class="form-control" placeholder="Nombre de usuario" />
                  </div>
                  <div class="col-md-6">
                    <label class="form-label">Email</label>
                    <input id="email" type="email" class="form-control" placeholder="correo@ejemplo.com" />
                  </div>
                </div>

                <div class="row mb-3">
                  <div class="col-md-6">
                    <label class="form-label">Rol</label>
                    <select id="rol_id" class="form-select">
                      <option value="">Seleccionar rol...</option>
                      ${roles
                        .map((r: any) => `<option value="${r.id}">${r.nombre}</option>`)
                        .join("")}
                    </select>
                  </div>
                </div>
              `
              : ""
          }
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,

      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
        const telefono = (document.getElementById("telefono") as HTMLInputElement).value.trim();
        const especialidad = (document.getElementById("especialidad") as HTMLInputElement).value.trim();
        const direccion = (document.getElementById("direccion") as HTMLInputElement).value.trim();
        const activo = (document.getElementById("activo") as HTMLInputElement).checked;

        const alias = (document.getElementById("alias") as HTMLInputElement)?.value.trim();
        const email = (document.getElementById("email") as HTMLInputElement)?.value.trim();
        const rol_id = (document.getElementById("rol_id") as HTMLSelectElement)?.value;

        if (!nombre) {
          Swal.showValidationMessage("El nombre es obligatorio");
          return false;
        }

        return { nombre, telefono, especialidad, direccion, activo, alias, email, rol_id };
      },
    });

    if (!formValues) return;

    // 🔹 Lógica diferenciada según el modo
    if (modo === "perfil") {
      await gymApi.put(`/perfil/${id}`, {
        nombre: formValues.nombre,
        telefono: formValues.telefono,
        especialidad: formValues.especialidad,
        direccion: formValues.direccion,
        estado: formValues.activo, // bool
      });
    } else {
      await gymApi.put(`/personal/${id}`, {
        nombre: formValues.nombre,
        telefono: formValues.telefono,
        especialidad: formValues.especialidad,
        direccion: formValues.direccion,
        activo: formValues.activo,
        email: formValues.email || null,
        rolId: formValues.rol_id ? parseInt(formValues.rol_id) : null,
      });
    }

    await Swal.fire({
      icon: "success",
      title: "Actualizado",
      text: modo === "perfil"
        ? "Tu perfil se actualizó correctamente."
        : "El registro fue modificado correctamente.",
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudo cargar o actualizar el registro", "error");
  }
}
