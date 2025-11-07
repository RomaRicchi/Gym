import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function GrupoMuscularEditSwal(id: string, onSuccess?: () => void) {
  try {
    const { data: grupo } = await gymApi.get(`/grupomuscular/${id}`);

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Grupo Muscular",
      html: `
        <div class="mb-2 text-start">
          <label class="form-label fw-semibold">Nombre</label>
          <input id="nombre" class="form-control" value="${grupo.nombre || ""}" />
        </div>
        <div class="mb-2 text-start">
          <label class="form-label fw-semibold">Descripción</label>
          <textarea id="descripcion" class="form-control" rows="2">${grupo.descripcion || ""}</textarea>
        </div>
        <div class="mb-2 text-start">
          <label class="form-label fw-semibold">Imagen URL</label>
          <input id="imagenUrl" class="form-control" value="${grupo.imagenUrl || ""}" />
        </div>
      `,
      confirmButtonText: "Guardar cambios",
      showCancelButton: true,
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#ff6600",
      focusConfirm: false,
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
        const descripcion = (document.getElementById("descripcion") as HTMLTextAreaElement).value.trim();
        const imagenUrl = (document.getElementById("imagenUrl") as HTMLInputElement).value.trim();

        if (!nombre) {
          Swal.showValidationMessage("⚠️ El nombre es obligatorio.");
          return false;
        }

        return { id: grupo.id, nombre, descripcion, imagenUrl };
      },
    });

    if (formValues) {
      await gymApi.put(`/grupomuscular/${id}`, formValues);
      Swal.fire("✅ Actualizado", "Grupo muscular editado correctamente.", "success");
      onSuccess?.();
    }
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudo editar el grupo muscular.", "error");
  }
}
