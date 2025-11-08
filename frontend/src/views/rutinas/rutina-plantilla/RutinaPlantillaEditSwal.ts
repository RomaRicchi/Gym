// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function RutinaPlantillaEditSwal(id: number, onSuccess?: () => void) {
  try {
    // 🔹 Obtener datos actuales
    const { data: rutina } = await gymApi.get(`/rutinasplantilla/${id}`);

    // 🔹 Cargar grupos musculares
    const { data: grupos } = await gymApi.get("/grupomuscular");
    const gruposOptions = (grupos.items || grupos)
      .map(
        (g: any) =>
          `<option value="${g.id}" ${
            g.id === rutina.grupoMuscularId ? "selected" : ""
          }>${g.nombre}</option>`
      )
      .join("");

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Rutina",
      width: 650,
      showCancelButton: true,
      confirmButtonText: "Guardar cambios",
      cancelButtonText: "Cancelar",
      buttonsStyling: false,
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      html: `
        <form class="swal-form">
          <div class="swal-input-group">
            <label class="swal-label">Nombre</label>
            <input id="nombre" type="text" class="swal-field" value="${rutina.nombre || ""}">
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Objetivo</label>
            <textarea id="objetivo" class="swal-field" rows="2">${
              rutina.objetivo || ""
            }</textarea>
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Grupo Muscular</label>
            <select id="grupo" class="swal-field">
              ${gruposOptions}
            </select>
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Imagen actual</label>
            ${
              rutina.imagenUrl
                ? `<img src="${import.meta.env.VITE_API_BASE_URL}/${rutina.imagenUrl}" 
                        style="width:80px;height:80px;object-fit:cover;border-radius:8px;
                        border:2px solid #ff6600;margin-bottom:8px;">`
                : `<div class="text-muted small">Sin imagen</div>`
            }
            <input id="imagen" type="file" accept="image/*" class="swal-field">
          </div>
        </form>
      `,
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value;
        const objetivo = (document.getElementById("objetivo") as HTMLTextAreaElement).value;
        const grupoMuscularId = (document.getElementById("grupo") as HTMLSelectElement).value;
        const imagen = (document.getElementById("imagen") as HTMLInputElement).files?.[0];
        return { nombre, objetivo, grupoMuscularId, imagen };
      },
    });

    if (!formValues) return;

    const { nombre, objetivo, grupoMuscularId, imagen } = formValues;

    // Crear FormData
    const formData = new FormData();
    formData.append("id", id.toString());
    formData.append("nombre", nombre);
    formData.append("objetivo", objetivo);
    formData.append("grupoMuscularId", grupoMuscularId);
    if (imagen) formData.append("imagen", imagen);
    else if (rutina.imagenUrl) formData.append("imagenUrl", rutina.imagenUrl);

    await gymApi.put(`/rutinasplantilla/${id}`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    Swal.fire({
      icon: "success",
      title: "Rutina actualizada",
      timer: 1000,
      showConfirmButton: false,
    });

    if (onSuccess) onSuccess();
  } catch (error) {
    console.error(error);
    Swal.fire("Error", "No se pudo actualizar la rutina", "error");
  }
}
