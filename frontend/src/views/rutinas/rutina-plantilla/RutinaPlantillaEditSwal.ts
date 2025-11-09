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

    // 🧱 Formulario Swal
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
          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Nombre</label>
            <input id="nombre" type="text" class="form-control" value="${rutina.nombre || ""}">
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Objetivo</label>
            <textarea id="objetivo" class="form-control" placeholder="Objetivo de la rutina">${rutina.objetivo || ""}</textarea>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Grupo Muscular</label>
            <select id="grupoMuscularId" class="form-select">
              ${gruposOptions}
            </select>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Imagen actual</label><br>
            ${
              rutina.imagenUrl
                ? `<img src="${import.meta.env.VITE_API_BASE_URL.replace(/\/api$/, '')}/${rutina.imagenUrl}"
                        style="width:80px;height:80px;object-fit:cover;border-radius:8px;
                        border:2px solid #ff6600;margin-bottom:8px;"
                        onerror="this.src='/placeholder.png'">`
                : `<div class='text-muted small mb-2'>Sin imagen</div>`
            }
            <input id="imagen" type="file" class="form-control" accept="image/*">
          </div>
        </form>
      `,
      preConfirm: () => {
        const nombre = document.getElementById("nombre").value.trim();
        const objetivo = document.getElementById("objetivo").value.trim();
        const grupoMuscularId = parseInt(document.getElementById("grupoMuscularId").value);
        const imagen = document.getElementById("imagen").files?.[0];

        if (!nombre) {
          Swal.showValidationMessage("El nombre es obligatorio");
          return false;
        }
        if (isNaN(grupoMuscularId)) {
          Swal.showValidationMessage("Debe seleccionar un grupo muscular");
          return false;
        }

        return { nombre, objetivo, grupoMuscularId, imagen };
      },
    });

    if (!formValues) return;

    const { nombre, objetivo, grupoMuscularId, imagen } = formValues;

    // 📦 Crear FormData (multipart/form-data)
    const formData = new FormData();
    formData.append("Id", id.toString());
    formData.append("Nombre", nombre);
    formData.append("Objetivo", objetivo);
    formData.append("GrupoMuscularId", grupoMuscularId);
    if (imagen) {
      formData.append("Imagen", imagen);
    } else if (rutina.imagenUrl) {
      formData.append("ImagenUrl", rutina.imagenUrl);
    }

    // 📨 Enviar al backend
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
  } catch (error: any) {
    console.error(error);
    Swal.fire("Error", error.response?.data || "No se pudo actualizar la rutina", "error");
  }
}
