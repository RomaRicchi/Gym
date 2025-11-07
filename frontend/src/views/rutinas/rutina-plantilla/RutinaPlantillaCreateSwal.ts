// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function RutinaPlantillaCreateSwal(onSuccess?: () => void) {
  try {
    const { data } = await gymApi.get("/grupomuscular");
    const grupos = data.items || data;

    const opcionesGrupo = grupos
      .map((g: any) => `<option value="${g.id}">${g.nombre}</option>`)
      .join("");

    const { value: formValues } = await Swal.fire({
      title: "🏋️‍♀️ Nueva Rutina",
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
            <input id="nombreInput" type="text" class="swal2-input" placeholder="Ej: Full Body Avanzada">
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Objetivo</label>
            <input id="objetivoInput" type="text" class="swal2-input" placeholder="Ej: Ganar masa muscular">
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Grupo muscular</label>
            <select id="grupoSelect" class="swal2-input form-select">
              <option value="">Seleccionar...</option>
              ${opcionesGrupo}
            </select>
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Imagen (opcional)</label>
            <input id="imagenInput" type="file" class="form-control" accept=".jpg,.jpeg,.png">
          </div>
        </form>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar",
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
    formData.append("nombre", formValues.nombre);
    formData.append("objetivo", formValues.objetivo || "");
    formData.append("grupoMuscularId", formValues.grupoId);
    if (formValues.file) formData.append("image", formValues.file);

    await gymApi.post("/rutinasplantilla", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    Swal.fire({
      icon: "success",
      title: "✅ Rutina creada correctamente",
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err: any) {
    Swal.fire("Error", err.response?.data || "No se pudo crear la rutina", "error");
  }
}
