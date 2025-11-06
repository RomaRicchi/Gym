import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-usuario.css"; 

export async function RolEditSwal(id: string, onSuccess?: () => void) {
  try {
    const { data } = await gymApi.get(`/roles/${id}`);

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Rol",
      width: 500,
      customClass: {
        popup: "swal2-card-usuario",       // fondo naranja
        confirmButton: "btn btn-orange",   // botón guardar
        cancelButton: "btn btn-secondary", // botón cancelar
      },
      buttonsStyling: false,
      html: `
        <form class="swal-form-usuario">
          <div>
            <label class="swal-label">Nombre del rol</label>
            <input id="nombre" type="text" value="${data.nombre || ""}" placeholder="Ej: Profesor, Socio..." />
          </div>
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "Guardar cambios",
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

    await gymApi.put(`/roles/${id}`, { nombre: formValues.nombre });

    await Swal.fire({
      icon: "success",
      title: "✅ Actualizado",
      text: "Rol modificado correctamente",
      timer: 1500,
      showConfirmButton: false,
      customClass: { popup: "swal2-card-usuario" },
    });

    onSuccess?.();
  } catch (err) {
    console.error("Error al editar rol:", err);
    Swal.fire({
      icon: "error",
      title: "❌ Error",
      text: "No se pudo cargar o actualizar el rol",
      customClass: { popup: "swal2-card-usuario" },
    });
  }
}
