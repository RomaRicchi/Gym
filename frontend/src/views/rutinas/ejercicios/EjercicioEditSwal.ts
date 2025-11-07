// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function EjercicioEditSwal(id: number, onSuccess?: () => void) {
  try {
    // 🔹 Obtener ejercicio + grupos musculares
    const [{ data: ejercicio }, { data: gruposData }] = await Promise.all([
      gymApi.get(`/ejercicios/${id}`),
      gymApi.get(`/grupomuscular`),
    ]);

    const gruposMusculares = gruposData.items || gruposData || [];

    // 🧱 Formulario SweetAlert2
    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Ejercicio",
      width: 650,
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      buttonsStyling: false,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar",
      cancelButtonText: "Cancelar",
      html: `
        <form class="swal-form" enctype="multipart/form-data">
          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Nombre</label>
            <input id="nombre" type="text" class="form-control"
              value="${ejercicio.nombre || ""}" placeholder="Nombre del ejercicio">
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Tips</label>
            <textarea id="tips" class="form-control" 
              placeholder="Consejos o notas">${ejercicio.tips || ""}</textarea>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Grupo Muscular</label>
            <select id="grupoMuscularId" class="form-select">
              ${gruposMusculares
                .map(
                  (g: any) =>
                    `<option value="${g.id}" ${
                      g.id === ejercicio.grupoMuscularId ? "selected" : ""
                    }>${g.nombre}</option>`
                )
                .join("")}
            </select>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Imagen</label><br>
            ${
              ejercicio.mediaUrl
                ? `<img src="${import.meta.env.VITE_API_URL}/${ejercicio.mediaUrl}"
                     id="preview" alt="preview"
                     style="width:100px;height:100px;object-fit:cover;
                            border-radius:8px;border:2px solid #ff6600;margin-bottom:8px;">`
                : `<span class="text-muted d-block mb-2">Sin imagen actual</span>`
            }
            <input id="imagen" type="file" class="form-control" accept="image/*">
          </div>
        </form>
      `,
      preConfirm: async () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
        const tips = (document.getElementById("tips") as HTMLTextAreaElement).value.trim();
        const grupoMuscularId = parseInt(
          (document.getElementById("grupoMuscularId") as HTMLSelectElement).value
        );
        const imagenFile = (document.getElementById("imagen") as HTMLInputElement).files?.[0] || null;

        if (!nombre) {
          Swal.showValidationMessage("El nombre es obligatorio");
          return false;
        }

        // 📤 Si hay nueva imagen, subirla al backend
        let mediaUrl = ejercicio.mediaUrl;
        if (imagenFile) {
          const formData = new FormData();
          formData.append("file", imagenFile);

          const { data } = await gymApi.post("/ejercicios/upload", formData, {
            headers: { "Content-Type": "multipart/form-data" },
          });

          mediaUrl = data.url; // el backend devuelve { url: "Uploads/Ejercicios/archivo.jpg" }
        }

        return { nombre, tips, grupoMuscularId, mediaUrl };
      },
    });

    if (!formValues) return;

    // 📨 Actualizar ejercicio
    await gymApi.put(`/ejercicios/${id}`, {
       Id: id,
  Nombre: formValues.nombre,
  Tips: formValues.tips,
  GrupoMuscularId: formValues.grupoMuscularId,
  MediaUrl: formValues.mediaUrl ?? null
    });

    Swal.fire({
      icon: "success",
      title: "Ejercicio actualizado",
      timer: 1200,
      showConfirmButton: false,
    });

    if (onSuccess) onSuccess();
  } catch (error) {
    console.error(error);
    Swal.fire("Error", "No se pudo actualizar el ejercicio", "error");
  }
}
