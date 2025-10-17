import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

// 🧱 Interfaz para los datos del socio
interface SocioForm {
  dni: string;
  nombre: string;
  email: string;
  telefono: string;
  activo: boolean;
}

/**
 * 🧡 Muestra un formulario SweetAlert2 para crear un nuevo socio.
 * Retorna true si se creó correctamente.
 */
export async function mostrarFormNuevoSocio(): Promise<boolean> {
  const { value: formValues } = await Swal.fire<SocioForm>({
    title: "🧍 Nuevo Socio",
    html: `
      <div class="swal2-card-style text-start">
        <label class="form-label mt-2">DNI</label>
        <input id="dni" class="swal2-input" placeholder="Ingrese DNI">

        <label class="form-label mt-2">Nombre</label>
        <input id="nombre" class="swal2-input" placeholder="Ingrese nombre">

        <label class="form-label mt-2">Email</label>
        <input id="email" type="email" class="swal2-input" placeholder="Ingrese email">

        <label class="form-label mt-2">Teléfono</label>
        <input id="telefono" class="swal2-input" placeholder="Ingrese teléfono">

        <div class="form-check mt-3">
          <input type="checkbox" id="activo" class="form-check-input" checked>
          <label class="form-check-label" for="activo">Activo</label>
        </div>
      </div>
    `,
    focusConfirm: false,
    showCancelButton: true,
    confirmButtonText: "💾 Guardar",
    cancelButtonText: "Cancelar",
    preConfirm: () => {
      const dni = (document.getElementById("dni") as HTMLInputElement)?.value.trim();
      const nombre = (document.getElementById("nombre") as HTMLInputElement)?.value.trim();
      const email = (document.getElementById("email") as HTMLInputElement)?.value.trim();
      const telefono = (document.getElementById("telefono") as HTMLInputElement)?.value.trim();
      const activo = (document.getElementById("activo") as HTMLInputElement)?.checked ?? true;

      if (!dni || !nombre || !email)
        Swal.showValidationMessage("DNI, Nombre y Email son obligatorios");

      return { dni, nombre, email, telefono, activo };
    },
  });

  if (!formValues) return false;

  try {
    await gymApi.post("/socios", formValues);
    await Swal.fire({
      icon: "success",
      title: "✅ Socio creado",
      text: "El socio fue registrado correctamente.",
      timer: 1600,
      showConfirmButton: false,
    });
    return true;
  } catch (err) {
    console.error(err);
    await Swal.fire({
      icon: "error",
      title: "Error",
      text: "No se pudo registrar el socio. Verifique los datos.",
    });
    return false;
  }
}
