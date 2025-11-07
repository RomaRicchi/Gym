import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function EjercicioEditSwal(id: string, onSuccess?: () => void) {
  try {
    const [resEjercicio, resGrupos] = await Promise.all([
      gymApi.get(`/ejercicios/${id}`),
      gymApi.get("/grupomuscular"),
    ]);

    const e = resEjercicio.data;
    const grupos = resGrupos.data.items || resGrupos.data;

    const options = grupos
      .map(
        (g: any) =>
          `<option value="${g.id}" ${g.id === e.grupoMuscularId ? "selected" : ""}>
             ${g.nombre}
           </option>`
      )
      .join("");

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Ejercicio",
      html: `
        <form id="form-ejercicio-edit" class="swal-form">
          <div class="mb-2 text-start">
            <label class="form-label fw-semibold">Nombre</label>
            <input id="nombre" class="form-control" value="${e.nombre || ""}" />
          </div>

          <div class="mb-2 text-start">
            <label class="form-label fw-semibold">Grupo Muscular</label>
            <select id="grupoMuscularId" class="form-select">
              ${options}
            </select>
          </div>

          <div class="mb-2 text-start">
            <label class="form-label fw-semibold">Tips</label>
            <textarea id="tips" class="form-control" rows="2">${e.tips || ""}</textarea>
          </div>

          <div class="mb-2 text-start">
            <label class="form-label fw-semibold">Media URL</label>
            <input id="mediaUrl" class="form-control" value="${e.mediaUrl || ""}" />
          </div>
        </form>
      `,
      focusConfirm: false,
      confirmButtonText: "Guardar cambios",
      showCancelButton: true,
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#ff6600",
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement)?.value.trim();
        const grupoMuscularId = (document.getElementById("grupoMuscularId") as HTMLSelectElement)?.value;
        const tips = (document.getElementById("tips") as HTMLTextAreaElement)?.value.trim();
        const mediaUrl = (document.getElementById("mediaUrl") as HTMLInputElement)?.value.trim();

        if (!nombre || !grupoMuscularId) {
          Swal.showValidationMessage("⚠️ El nombre y el grupo muscular son obligatorios.");
          return false;
        }

        return { id: e.id, nombre, grupoMuscularId: Number(grupoMuscularId), tips, mediaUrl };
      },
    });

    if (formValues) {
      await gymApi.put(`/ejercicios/${id}`, formValues);
      Swal.fire("✅ Actualizado", "Ejercicio editado correctamente.", "success");
      onSuccess?.();
    }
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudo editar el ejercicio.", "error");
  }
}
