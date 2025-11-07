// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function RutinaPlantillaEditSwal(id: number, onSuccess?: () => void) {
  try {
    const [{ data: rutina }, { data: gruposRes }] = await Promise.all([
      gymApi.get(`/rutinasplantilla/${id}`),
      gymApi.get("/grupomuscular"),
    ]);

    const grupos = gruposRes.items || gruposRes;

    const opcionesGrupo = grupos
      .map((g: any) => {
        const selected = g.id === rutina.grupoMuscularId ? "selected" : "";
        return `<option value="${g.id}" ${selected}>${g.nombre}</option>`;
      })
      .join("");

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Rutina",
      width: 650,
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn-orange",
        cancelButton: "btn-secondary",
      },
      buttonsStyling: false,
      html: `
        <form class="swal-form">
          <div class="swal-input-group">
            <label class="swal-label">Nombre</label>
            <input id="nombreInput" type="text" class="swal2-input" value="${rutina.nombre || ""}">
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Objetivo</label>
            <input id="objetivoInput" type="text" class="swal2-input" value="${rutina.objetivo || ""}">
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Grupo muscular</label>
            <select id="grupoSelect" class="swal2-input form-select">
              ${opcionesGrupo}
            </select>
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Imagen (opcional)</label>
            <input id="imagenInput" type="file" class="form-control" accept=".jpg,.jpeg,.png">
            ${
              rutina.imagenUrl
                ? `<div style="margin-top:10px;text-align:center">
                     <img src="${import.meta.env.VITE_API_URL}/${rutina.imagenUrl}"
                          style="width:90px;height:90px;object-fit:cover;border-radius:8px;border:2px solid #ff6600;">
                   </div>`
                : ""
            }
          </div>
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      preConfirm: () => {
        const nombre = document.getElementById("nombreInput").value.trim();
        const objetivo = document.getElementById("objetivoInput").value.trim();
        const grupoId = document.getElementById("grupoSelect").value;
        const file = (document.getElementById("imagenInput") as HTMLInputElement).files?.[0];

        if (!nombre || !grupoId) {
          Swal.showValidationMessage("Completá los campos obligatorios");
          return false;
        }
        return { nombre, objetivo, grupoId, file };
      },
    });

    if (!formValues) return;

    const formData = new FormData();
    formData.append("id", id.toString());
    formData.append("nombre", formValues.nombre);
    formData.append("objetivo", formValues.objetivo || "");
    formData.append("grupoMuscularId", formValues.grupoId);
    if (formValues.file) formData.append("image", formValues.file);

    await gymApi.put(`/rutinasplantilla/${id}`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    Swal.fire({
      icon: "success",
      title: "✅ Cambios guardados",
      timer: 1200,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err: any) {
    Swal.fire("Error", err.response?.data || "No se pudo editar la rutina", "error");
  }
}
