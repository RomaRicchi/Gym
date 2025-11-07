// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function EjercicioEditSwal(id: number, onSuccess?: () => void) {
  try {
    const [{ data: ejercicio }, { data: gruposRes }] = await Promise.all([
      gymApi.get(`/ejercicios/${id}`),
      gymApi.get("/grupomuscular"),
    ]);

    const grupos = gruposRes.items || gruposRes;

    const opcionesGrupo = grupos
      .map((g: any) => {
        const selected = g.id === ejercicio.grupoMuscularId ? "selected" : "";
        return `<option value="${g.id}" ${selected}>${g.nombre}</option>`;
      })
      .join("");

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Ejercicio",
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
            <input id="nombreInput" type="text" class="swal2-input" value="${ejercicio.nombre || ""}">
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Grupo muscular</label>
            <select id="grupoSelect" class="swal2-input form-select">
              ${opcionesGrupo}
            </select>
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Tips o recomendaciones</label>
            <textarea id="tipsInput" rows="3" class="swal2-textarea">${ejercicio.tips || ""}</textarea>
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Imagen (opcional)</label>
            <input id="imagenInput" type="file" accept=".jpg,.jpeg,.png" class="form-control">
            ${
              ejercicio.imagenUrl
                ? `<div style="margin-top:10px;text-align:center">
                     <img src="${import.meta.env.VITE_API_URL}/${ejercicio.imagenUrl}" 
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
        const nombre = (document.getElementById("nombreInput") as HTMLInputElement).value.trim();
        const grupoId = (document.getElementById("grupoSelect") as HTMLSelectElement).value;
        const tips = (document.getElementById("tipsInput") as HTMLTextAreaElement).value;
        const file = (document.getElementById("imagenInput") as HTMLInputElement).files?.[0];

        if (!nombre || !grupoId) {
          Swal.showValidationMessage("Completá los campos obligatorios");
          return false;
        }
        return { nombre, grupoId, tips, file };
      },
    });

    if (!formValues) return;

    // 🔹 Crear FormData
    const formData = new FormData();
    formData.append("nombre", formValues.nombre);
    formData.append("grupoMuscularId", formValues.grupoId);
    formData.append("tips", formValues.tips || "");
    if (formValues.file) formData.append("image", formValues.file);

    await gymApi.put(`/ejercicios/${id}`, formData, {
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
    Swal.fire("Error", err.response?.data || "No se pudo editar el ejercicio", "error");
  }
}
