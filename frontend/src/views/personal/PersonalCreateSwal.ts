import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function PersonalCreateSwal(onSuccess?: () => void) {
  // 🔹 Cargar roles disponibles
  let roles: { id: number; nombre: string }[] = [];
  try {
    const res = await gymApi.get("/roles");
    roles = res.data.items || res.data;
  } catch {
    console.warn("⚠️ No se pudieron cargar los roles");
  }

  const { value: formValues } = await Swal.fire({
    title: "➕ Nuevo Personal",
    html: `
      <div class="swal2-card-style">
        <h6 class="text-start mb-2 fw-bold text-primary">Datos personales</h6>
        <div class="row mb-3">
          <div class="col-md-6">
            <label class="form-label">Nombre</label>
            <input id="nombre" type="text" class="form-control" placeholder="Nombre completo" required />
          </div>
          <div class="col-md-6">
            <label class="form-label">Teléfono</label>
            <input id="telefono" type="text" class="form-control" placeholder="Ej: 2664123456" />
          </div>
        </div>

        <div class="row mb-3">
          <div class="col-md-6">
            <label class="form-label">Especialidad</label>
            <input id="especialidad" type="text" class="form-control" placeholder="Ej: Yoga, Spinning..." />
          </div>
          <div class="col-md-6">
            <label class="form-label">Dirección</label>
            <input id="direccion" type="text" class="form-control" placeholder="Ej: Av. Mitre 1234" />
          </div>
        </div>

        <div class="form-check text-start mb-3">
          <input id="activo" type="checkbox" class="form-check-input" checked />
          <label class="form-check-label" for="activo">Activo</label>
        </div>

        <hr class="my-3" />
        <h6 class="text-start mb-2 fw-bold text-success">Datos de usuario (opcional)</h6>
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
                .map((r) => `<option value="${r.id}">${r.nombre}</option>`)
                .join("")}
            </select>
          </div>
        </div>
      </div>
    `,
    showCancelButton: true,
    confirmButtonText: "Guardar",
    cancelButtonText: "Cancelar",
    focusConfirm: false,

    preConfirm: () => {
      const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
      const telefono = (document.getElementById("telefono") as HTMLInputElement).value.trim();
      const especialidad = (document.getElementById("especialidad") as HTMLInputElement).value.trim();
      const direccion = (document.getElementById("direccion") as HTMLInputElement).value.trim();
      const activo = (document.getElementById("activo") as HTMLInputElement).checked;
      const alias = (document.getElementById("alias") as HTMLInputElement).value.trim();
      const email = (document.getElementById("email") as HTMLInputElement).value.trim();
      const rol_id = (document.getElementById("rol_id") as HTMLSelectElement).value;

      if (!nombre) {
        Swal.showValidationMessage("El nombre es obligatorio");
        return;
      }

      return { nombre, telefono, especialidad, direccion, activo, alias, email, rol_id };
    },
  });

  if (!formValues) return;

  try {
    // 1️⃣ Crear el registro de personal (el backend crea usuario si hay email y rol)
    const { nombre, telefono, especialidad, direccion, activo, email, rol_id } = formValues;

    const res = await gymApi.post("/personal", {
      nombre,
      telefono,
      especialidad,
      direccion,
      activo,
      email,
      rolId: rol_id ? parseInt(rol_id) : null,
    });

    console.log("✅ Personal creado:", res.data);

    await Swal.fire({
      icon: "success",
      title: "Registro creado",
      text: "El personal fue registrado correctamente.",
      timer: 1600,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err) {
    console.error(err);
    Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudo guardar el registro. Verifique los datos.",
    });
  }
}
