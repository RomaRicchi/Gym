// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function EjercicioEditSwal(id: number, onSuccess?: () => void) {
  try {
    // 🔹 Obtener datos actuales
    const { data: ejercicio } = await gymApi.get(`/ejercicios/${id}`);

    // 🔹 Obtener grupos musculares
    const { data: grupos } = await gymApi.get("/grupomuscular");
    const opcionesGrupo = (grupos.items || grupos)
      .map(
        (g: any) =>
          `<option value="${g.id}" ${
            ejercicio.grupoMuscularId === g.id ? "selected" : ""
          }>${g.nombre}</option>`
      )
      .join("");

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Ejercicio",
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
            <input id="nombre" class="swal-field" type="text" value="${
              ejercicio.nombre || ""
            }">
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Grupo muscular</label>
            <select id="grupo" class="swal-field">
              ${opcionesGrupo}
            </select>
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Tips</label>
            <textarea id="tips" class="swal-field" rows="2">${
              ejercicio.tips || ""
            }</textarea>
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Imagen (opcional)</label>
            <input id="imagen" type="file" accept="image/*" class="swal-field">
            ${
              ejercicio.mediaUrl
                ? `<div style="margin-top:6px">
                     <img src="${import.meta.env.VITE_API_URL}/${
                    ejercicio.mediaUrl
                  }" style="width:80px;height:80px;object-fit:cover;border-radius:8px;border:2px solid #ff6600">
                   </div>`
                : ""
            }
          </div>
        </form>
      `,
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement)
          .value;
        const grupo = (document.getElementById("grupo") as HTMLSelectElement)
          .value;
        const tips = (document.getElementById("tips") as HTMLTextAreaElement)
          .value;
        const imagen = (document.getElementById("imagen") as HTMLInputElement)
          .files?.[0];
        return { nombre, grupo, tips, imagen };
      },
    });

    if (!formValues) return;

    const { nombre, grupo, tips, imagen } = formValues;

    // 🔹 Crear FormData (multipart/form-data)
    const formData = new FormData();
    formData.append("Id", id.toString());
    formData.append("Nombre", nombre);
    formData.append("GrupoMuscularId", grupo);
    formData.append("Tips", tips || "");

    // Si el usuario subió nueva imagen
    if (imagen) {
      formData.append("Imagen", imagen);
    } else if (ejercicio.mediaUrl) {
      // mantener la anterior
      formData.append("MediaUrl", ejercicio.mediaUrl);
    }

    // 🔹 Enviar al backend
    await gymApi.put(`/ejercicios/${id}`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    Swal.fire({
      icon: "success",
      title: "Ejercicio actualizado",
      timer: 1000,
      showConfirmButton: false,
    });

    if (onSuccess) onSuccess();
  } catch (error) {
    console.error(error);
    Swal.fire("Error", "No se pudo actualizar el ejercicio", "error");
  }
}
