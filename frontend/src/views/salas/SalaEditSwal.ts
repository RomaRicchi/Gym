import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function SalaEditSwal(id: string, onSuccess?: () => void) {
  try {
    const res = await gymApi.get(`/salas/${id}`);
    const data = res.data;

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Sala",
      html: `
        <div class="swal2-card-style">
          <div class="mb-3 text-start">
            <label class="form-label">Nombre</label>
            <input id="nombre" type="text" class="form-control" value="${data.nombre || ""}"/>
          </div>
          <div class="mb-3 text-start">
            <label class="form-label">Cupo</label>
            <input id="cupo" type="number" class="form-control" value="${data.cupo || 1}" min="1"/>
          </div>
          <div class="form-check mb-3 text-start">
            <input id="activa" type="checkbox" class="form-check-input" ${data.activa ? "checked" : ""}/>
            <label class="form-check-label" for="activa">Activa</label>
          </div>
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "Guardar cambios",
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
        const cupo = (document.getElementById("cupo") as HTMLInputElement).value;
        const activa = (document.getElementById("activa") as HTMLInputElement).checked;

        if (!nombre) {
          Swal.showValidationMessage("El nombre es obligatorio");
          return false;
        }

        return { nombre, cupo, activa };
      },
    });

    if (formValues) {
      await gymApi.put(`/salas/${id}`, {
        ...formValues,
        activa: formValues.activa ? 1 : 0,
      });
      Swal.fire("Actualizada", "Sala modificada correctamente", "success");
      onSuccess?.();
    }
  } catch {
    Swal.fire("Error", "No se pudo cargar o actualizar la sala", "error");
  }
}
