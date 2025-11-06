// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-ejercicio.css";

/**
 * Editar datos personales del usuario logueado (perfil)
 * Llama a PATCH /api/perfil/{id}/personal
 */
export async function PerfilEditSwal(usuarioId: number, onSuccess?: () => void) {
  try {
    const { data: perfil } = await gymApi.get(`/perfil/${usuarioId}`);
    const personal = perfil.personal ?? {};

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Perfil",
      html: `
        <form class="swal-form-ejercicio">
          <input id="nombre" type="text" placeholder="Nombre completo" value="${personal.nombre || ""}">
          <input id="telefono" type="text" placeholder="Teléfono de contacto" value="${personal.telefono || ""}">
          <input id="direccion" type="text" placeholder="Dirección completa" value="${personal.direccion || ""}">
          <input id="especialidad" type="text" placeholder="Área o especialidad" value="${personal.especialidad || ""}">
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      customClass: {
        popup: "swal2-card-ejercicio",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      buttonsStyling: false,
      preConfirm: () => {
        const nombre = document.getElementById("nombre").value.trim();
        const telefono = document.getElementById("telefono").value.trim();
        const direccion = document.getElementById("direccion").value.trim();
        const especialidad = document.getElementById("especialidad").value.trim();

        if (!nombre) {
          Swal.showValidationMessage("El nombre es obligatorio");
          return;
        }

        return { nombre, telefono, direccion, especialidad };
      },
    });

    if (!formValues) return;

    // 🔹 PATCH actualizado con nueva ruta
    await gymApi.patch(`/perfil/${usuarioId}/personal`, formValues);

    await Swal.fire({
      icon: "success",
      title: "✅ Perfil actualizado",
      text: "Los cambios fueron guardados correctamente.",
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err) {
    console.error(err);
    await Swal.fire({
      icon: "error",
      title: "❌ Error",
      text: "No se pudo actualizar tu perfil.",
    });
  }
}
