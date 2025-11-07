// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function EjercicioCreateSwal(onSuccess?: () => void) {
  try {
    // 🔹 Cargar grupos musculares
    const { data } = await gymApi.get("/grupomuscular");
    const grupos = data.items || data;

    const opcionesGrupo = grupos
      .map((g: any) => `<option value="${g.id}">${g.nombre}</option>`)
      .join("");

    const { value: formValues } = await Swal.fire({
      title: "🏋️‍♂️ Nuevo Ejercicio",
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
            <input id="nombreInput" type="text" class="swal2-input" placeholder="Ej: Press de banca">
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Grupo muscular</label>
            <select id="grupoSelect" class="swal2-input form-select">
              <option value="">Seleccionar...</option>
              ${opcionesGrupo}
            </select>
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Tips o recomendaciones</label>
            <textarea id="tipsInput" rows="3" class="swal2-textarea" placeholder="Mantener la espalda recta..."></textarea>
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Imagen (opcional)</label>
            <input id="imagenInput" type="file" accept=".jpg,.jpeg,.png" class="form-control">
          </div>
        </form>
      `,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar",
      cancelButtonText: "Cancelar",
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

    // 🔹 Crear FormData para enviar imagen
    const formData = new FormData();
    formData.append("nombre", formValues.nombre);
    formData.append("grupoMuscularId", formValues.grupoId);
    formData.append("tips", formValues.tips || "");
    if (formValues.file) formData.append("image", formValues.file);

    await gymApi.post("/ejercicios", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    Swal.fire({
      icon: "success",
      title: "✅ Ejercicio agregado correctamente",
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err: any) {
    Swal.fire("Error", err.response?.data || "No se pudo crear el ejercicio", "error");
  }
}
