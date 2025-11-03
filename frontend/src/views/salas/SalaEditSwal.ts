import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function SalaEditSwal(id: string, onSuccess: () => void) {
  try {
    // Obtener datos actuales de la sala
    const { data } = await gymApi.get(`/salas/${id}`);

    const { value: formValues } = await Swal.fire({
      title: "Editar Sala",
      html: `
        <input id="nombre" class="swal2-input" placeholder="Nombre" value="${data.nombre || ""}">
        <input id="cupo" type="number" class="swal2-input" placeholder="Cupo" value="${data.cupo || 0}">
        <select id="activa" class="swal2-input">
          <option value="true" ${data.activa ? "selected" : ""}>Activa</option>
          <option value="false" ${!data.activa ? "selected" : ""}>Inactiva</option>
        </select>
      `,
      confirmButtonText: "Guardar cambios",
      showCancelButton: true,
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
        const cupo = (document.getElementById("cupo") as HTMLInputElement).value;
        const activa = (document.getElementById("activa") as HTMLSelectElement).value;

        if (!nombre || !cupo) {
          Swal.showValidationMessage("Todos los campos son obligatorios");
          return false;
        }

        return { nombre, cupo, activa };
      },
    });

    if (!formValues) return; // usuario canceló

    // Conversión de tipos correcta para el backend
    const payload = {
      nombre: formValues.nombre,
      cupo: Number(formValues.cupo),
      activa: formValues.activa === "true" || formValues.activa === true,
    };

    // Enviar PUT al backend
    await gymApi.put(`/salas/${id}`, payload);

    Swal.fire("Actualizado", "La sala fue actualizada correctamente", "success");
    onSuccess();
  } catch (error) {
    console.error("Error al editar sala:", error);
    Swal.fire("Error", "No se pudo actualizar la sala", "error");
  }
}
