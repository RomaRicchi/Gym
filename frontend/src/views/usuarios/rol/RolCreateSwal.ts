import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function RolCreateSwal(onSuccess?: () => void) {
  const { value: nombre } = await Swal.fire({
    title: "➕ Nuevo Rol",
    html: `
      <div class="swal2-card-style text-start">
        <label class="form-label fw-bold">Nombre del rol</label>
        <input id="nombre" type="text" class="form-control" placeholder="Ej: Administrador, Profesor..." />
      </div>
    `,
    focusConfirm: false,
    showCancelButton: true,
    confirmButtonText: "Guardar",
    cancelButtonText: "Cancelar",

    preConfirm: () => {
      const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
      if (!nombre) {
        Swal.showValidationMessage("Debe ingresar un nombre para el rol");
        return false;
      }
      return nombre;
    },
  });

  if (!nombre) return;

  try {
    await gymApi.post("/roles", { nombre });
    await Swal.fire({
      icon: "success",
      title: "Guardado",
      text: "Rol creado correctamente",
      timer: 1500,
      showConfirmButton: false,
    });
    onSuccess?.();
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudo crear el rol", "error");
  }
}
