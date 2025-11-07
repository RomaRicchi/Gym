// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function RutinaPlantillaEditSwal(id: number, onSuccess?: () => void) {
  try {
    const [{ data: rutina }, { data: gruposMusculares }] = await Promise.all([
      gymApi.get(`/rutinasplantilla/${id}`),
      gymApi.get(`/grupomuscular`),
    ]);

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Rutina",
      width: 650,
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      buttonsStyling: false,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      html: `
        <div class="swal-form">
          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Nombre</label>
            <input id="nombre" type="text" class="form-control" value="${rutina.nombre || ""}">
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Objetivo</label>
            <textarea id="objetivo" class="form-control">${rutina.objetivo || ""}</textarea>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Grupo Muscular</label>
            <select id="grupoMuscularId" class="form-select">
              ${gruposMusculares
                .map(
                  (g: any) =>
                    `<option value="${g.id}" ${
                      g.id === rutina.grupoMuscularId ? "selected" : ""
                    }>${g.nombre}</option>`
                )
                .join("")}
            </select>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Imagen actual</label><br>
            ${
              rutina.imagenUrl
                ? `<img src="${import.meta.env.VITE_API_URL}/${rutina.imagenUrl}" 
                    style="width:100px;height:100px;object-fit:cover;border-radius:8px;border:2px solid #ff6600;">`
                : `<span class="text-muted">Sin imagen</span>`
            }
          </div>
        </div>
      `,
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value;
        const objetivo = (document.getElementById("objetivo") as HTMLTextAreaElement).value;
        const grupoMuscularId = parseInt((document.getElementById("grupoMuscularId") as HTMLSelectElement).value);

        if (!nombre.trim()) {
          Swal.showValidationMessage("El nombre es obligatorio");
          return false;
        }

        return { nombre, objetivo, grupoMuscularId };
      },
    });

    if (!formValues) return;

    await gymApi.put(`/rutinasplantilla/${id}`, {
      id,
      ...formValues,
      imagenUrl: rutina.imagenUrl || null,
    });

    Swal.fire({
      icon: "success",
      title: "Rutina actualizada correctamente",
      timer: 1200,
      showConfirmButton: false,
    });

    if (onSuccess) onSuccess();
  } catch (error) {
    console.error(error);
    Swal.fire("Error", "No se pudo actualizar la rutina", "error");
  }
}
