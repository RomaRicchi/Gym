import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function RolEditSwal(id: string, onSuccess?: () => void) {
  try {
    const res = await gymApi.get(`/roles/${id}`);
    const data = res.data;

    const { value: nuevoNombre } = await Swal.fire({
      title: "✏️ Editar Rol",
      html: `
        <div class="swal2-card-style text-start">
          <label class="form-label fw-bold">Nombre del rol</label>
          <input id="nombre" type="text" class="form-control" value="${data.nombre}" />
        </div>
      `,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: "Guardar cambios",
      cancelButtonText: "Cancelar",

      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
        if (!nombre) {
          Swal.showValidationMessage("Debe ingresar un nombre");
          return false;
        }
        return nombre;
      },
    });

    if (!nuevoNombre) return;

    await gymApi.put(`/roles/${id}`, { nombre: nuevoNombre });
    await Swal.fire({
      icon: "success",
      title: "Actualizado",
      text: "Rol modificado correctamente",
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudo cargar o actualizar el rol", "error");
  }
}
