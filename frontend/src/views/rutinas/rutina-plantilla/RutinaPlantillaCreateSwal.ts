// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function RutinaPlantillaCreateSwal(onSuccess?: () => void) {
  try {
    // 🔹 Cargar grupos musculares
    const { data: grupos } = await gymApi.get("/grupomuscular");
    const gruposOptions = (grupos.items || grupos)
      .map((g: any) => `<option value="${g.id}">${g.nombre}</option>`)
      .join("");

    const { value: formValues } = await Swal.fire({
      title: "🧩 Nueva Rutina",
      width: 650,
      showCancelButton: true,
      confirmButtonText: "Guardar",
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
            <input id="nombre" type="text" class="swal-field" placeholder="Nombre de la rutina">
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Objetivo</label>
            <textarea id="objetivo" class="swal-field" rows="2" placeholder="Objetivo de la rutina"></textarea>
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Grupo Muscular</label>
            <select id="grupo" class="swal-field">
              <option value="">Seleccionar...</option>
              ${gruposOptions}
            </select>
          </div>

          <div class="swal-input-group">
            <label class="swal-label">Imagen</label>
            <input id="imagen" type="file" accept="image/*" class="swal-field">
          </div>
        </form>
      `,
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value;
        const objetivo = (document.getElementById("objetivo") as HTMLTextAreaElement).value;
        const grupoMuscularId = (document.getElementById("grupo") as HTMLSelectElement).value;
        const imagen = (document.getElementById("imagen") as HTMLInputElement).files?.[0];

        if (!nombre || !grupoMuscularId) {
          Swal.showValidationMessage("⚠️ Nombre y Grupo Muscular son obligatorios");
          return false;
        }
        return { nombre, objetivo, grupoMuscularId, imagen };
      },
    });

    if (!formValues) return;

    const { nombre, objetivo, grupoMuscularId, imagen } = formValues;

    // Crear FormData
    const formData = new FormData();
    formData.append("nombre", nombre);
    formData.append("objetivo", objetivo);
    formData.append("grupoMuscularId", grupoMuscularId);
    if (imagen) formData.append("imagen", imagen);

    await gymApi.post("/rutinasplantilla", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    Swal.fire({
      icon: "success",
      title: "Rutina creada",
      timer: 1000,
      showConfirmButton: false,
    });

    if (onSuccess) onSuccess();
  } catch (error) {
    console.error(error);
    Swal.fire("Error", "No se pudo crear la rutina", "error");
  }
}
