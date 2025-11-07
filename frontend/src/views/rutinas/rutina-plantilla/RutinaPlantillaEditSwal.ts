import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function RutinaPlantillaEditSwal(id: string, onSuccess?: () => void) {
  try {
    const [resRutina, resGrupos] = await Promise.all([
      gymApi.get(`/rutinasplantilla/${id}`),
      gymApi.get("/grupomuscular"),
    ]);

    const r = resRutina.data;
    const grupos = resGrupos.data.items || resGrupos.data;

    const options = grupos
      .map(
        (g: any) =>
          `<option value="${g.id}" ${g.id === r.grupoMuscularId ? "selected" : ""}>${g.nombre}</option>`
      )
      .join("");

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Rutina",
      html: `
        <div class="mb-2 text-start">
          <label class="form-label fw-semibold">Nombre</label>
          <input id="nombre" class="form-control" value="${r.nombre || ""}" />
        </div>
        <div class="mb-2 text-start">
          <label class="form-label fw-semibold">Objetivo</label>
          <input id="objetivo" class="form-control" value="${r.objetivo || ""}" />
        </div>
        <div class="mb-2 text-start">
          <label class="form-label fw-semibold">Grupo Muscular</label>
          <select id="grupoMuscularId" class="form-select">${options}</select>
        </div>
        <div class="mb-2 text-start">
          <label class="form-label fw-semibold">Imagen URL</label>
          <input id="imagenUrl" class="form-control" value="${r.imagenUrl || ""}" />
        </div>
      `,
      confirmButtonText: "Guardar cambios",
      showCancelButton: true,
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#ff6600",
      focusConfirm: false,
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
        const objetivo = (document.getElementById("objetivo") as HTMLInputElement).value.trim();
        const grupoMuscularId = (document.getElementById("grupoMuscularId") as HTMLSelectElement).value;
        const imagenUrl = (document.getElementById("imagenUrl") as HTMLInputElement).value.trim();

        if (!nombre || !grupoMuscularId) {
          Swal.showValidationMessage("⚠️ Nombre y grupo muscular son obligatorios.");
          return false;
        }

        return { id: r.id, nombre, objetivo, grupoMuscularId: Number(grupoMuscularId), imagenUrl };
      },
    });

    if (formValues) {
      await gymApi.put(`/rutinasplantilla/${id}`, formValues);
      Swal.fire("✅ Actualizado", "Rutina editada correctamente.", "success");
      onSuccess?.();
    }
  } catch (error) {
    console.error(error);
    Swal.fire("Error", "No se pudo editar la rutina.", "error");
  }
}
