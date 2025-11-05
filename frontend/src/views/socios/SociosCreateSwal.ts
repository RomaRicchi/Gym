import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-socio.css";

interface SocioForm {
  dni: string;
  nombre: string;
  email: string;
  telefono: string;
  activo: boolean;
}

export async function mostrarFormNuevoSocio(): Promise<boolean> {
  const { value: formValues } = await Swal.fire<SocioForm>({
    title:
      '<h2 class="fw-bold mb-3" style="font-size:1.6rem"><i class="fa-solid fa-user-plus me-2"></i>Nuevo Socio</h2>',
    html: `
      <form class="swal-form-socio">
        <div>
          <label class="swal-label">DNI</label>
          <input id="dni" type="text" placeholder="Ingrese DNI">
        </div>
        <div>
          <label class="swal-label">Nombre</label>
          <input id="nombre" type="text" placeholder="Ingrese nombre">
        </div>
        <div>
          <label class="swal-label">Email</label>
          <input id="email" type="email" placeholder="Ingrese email">
        </div>
        <div>
          <label class="swal-label">Teléfono</label>
          <input id="telefono" type="text" placeholder="Ingrese teléfono">
        </div>
        <div class="checkbox-group">
          <input type="checkbox" id="activo" checked class="swal-checkbox">
          <label for="activo" class="swal-label">Activo</label>
        </div>
      </form>
    `,
    showCancelButton: true,
    confirmButtonText: "Guardar",
    cancelButtonText: "Cancelar",
    focusConfirm: false,
    customClass: {
      popup: "swal2-card-socio",
      confirmButton: "btn btn-orange",
      cancelButton: "btn btn-secondary",
    },
    buttonsStyling: false,

    preConfirm: () => {
      const dni = (document.getElementById("dni") as HTMLInputElement)?.value.trim();
      const nombre = (document.getElementById("nombre") as HTMLInputElement)?.value.trim();
      const email = (document.getElementById("email") as HTMLInputElement)?.value.trim();
      const telefono = (document.getElementById("telefono") as HTMLInputElement)?.value.trim();
      const activo = (document.getElementById("activo") as HTMLInputElement)?.checked ?? true;

      if (!dni || !nombre || !email) {
        Swal.showValidationMessage("DNI, Nombre y Email son obligatorios");
        return false;
      }

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
