import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-ejercicio.css"; // 🧡 mismo estilo global

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
      <form class="swal-form-ejercicio">
        <h6 class="fw-bold" style="color:#000;">Datos personales</h6>

        <input 
          id="nombre" 
          type="text" 
          placeholder="Nombre completo" 
          required
        >
        <input 
          id="telefono" 
          type="text" 
          placeholder="Ej: 2664123456"
        >
        <input 
          id="especialidad" 
          type="text" 
          placeholder="Especialidad (Yoga, Spinning...)"
        >
        <input 
          id="direccion" 
          type="text" 
          placeholder="Dirección (Ej: Av. Mitre 1234)"
        >

        <div class="checkbox-group">
          <input 
            id="activo" 
            type="checkbox" 
            class="swal-checkbox" 
            checked
          >
          <label for="activo" class="swal-label">Activo</label>
        </div>

        <hr style="margin: 1rem 0; border-color:#fff3;">

        <h6 class="fw-bold" style="color:#000;">Datos de usuario (opcional)</h6>

        <input 
          id="alias" 
          type="text" 
          placeholder="Nombre de usuario"
        >
        <input 
          id="email" 
          type="email" 
          placeholder="correo@ejemplo.com"
        >
        <select id="rol_id">
          <option value="">Seleccionar rol...</option>
          ${roles.map((r) => `<option value="${r.id}">${r.nombre}</option>`).join("")}
        </select>
      </form>
    `,
    showCancelButton: true,
    confirmButtonText: "💾 Guardar",
    cancelButtonText: "Cancelar",
    focusConfirm: false,
    customClass: {
      popup: "swal2-card-ejercicio",      // 🧡 fondo naranja suave
      confirmButton: "btn btn-orange",    // 🟠 botón principal
      cancelButton: "btn btn-secondary",  // ⚪ botón secundario
    },
    buttonsStyling: false,

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
    // 🧩 Crear registro en backend (el backend crea usuario si hay email y rol)
    const { nombre, telefono, especialidad, direccion, activo, email, rol_id } = formValues;

    await gymApi.post("/personal", {
      nombre,
      telefono,
      especialidad,
      direccion,
      activo,
      email,
      rolId: rol_id ? parseInt(rol_id) : null,
    });

    await Swal.fire({
      icon: "success",
      title: "✅ Registro creado",
      text: "El personal fue registrado correctamente.",
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err) {
    console.error(err);
    await Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudo guardar el registro. Verifique los datos.",
    });
  }
}
