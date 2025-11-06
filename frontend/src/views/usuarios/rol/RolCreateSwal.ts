import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-usuario.css"; 

export async function RolCreateSwal(onSuccess?: () => void) {
  const { value: formValues } = await Swal.fire({
    title: "➕ Nuevo Rol",
    width: 500,
    customClass: {
      popup: "swal2-card-usuario",       // fondo naranja
      confirmButton: "btn btn-orange",   // botón naranja
      cancelButton: "btn btn-secondary", // botón blanco
    },
    buttonsStyling: false,
    html: `
      <form class="swal-form-usuario">
        <div>
          <label class="swal-label">Nombre del rol</label>
          <input id="nombre" type="text" placeholder="Ej: Administrador, Profesor..." />
        </div>
      </form>
    `,
    showCancelButton: true,
    confirmButtonText: "Guardar",
    cancelButtonText: "Cancelar",
    focusConfirm: false,

    preConfirm: () => {
      const nombre = (document.getElementById("nombre") as HTMLInputElement)?.value.trim();
      if (!nombre) {
        Swal.showValidationMessage("Debe ingresar un nombre para el rol");
        return;
      }
      return { nombre };
    },
  });

  if (!formValues) return;

  try {
    await gymApi.post("/roles", { nombre: formValues.nombre });

    await Swal.fire({
      icon: "success",
      title: "✅ Guardado",
      text: "Rol creado correctamente",
      timer: 1500,
      showConfirmButton: false,
      customClass: { popup: "swal2-card-usuario" },
    });

    onSuccess?.();
  } catch (err) {
    console.error(err);
    Swal.fire({
      icon: "error",
      title: "❌ Error",
      text: "No se pudo crear el rol",
      customClass: { popup: "swal2-card-usuario" },
    });
  }
}
