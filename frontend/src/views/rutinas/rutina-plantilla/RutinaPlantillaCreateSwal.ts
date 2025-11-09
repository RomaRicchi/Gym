// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function RutinaPlantillaCreateSwal(onSuccess?: () => void) {
  try {
    // 🔹 Obtener grupos musculares
    const { data: grupos } = await gymApi.get("/grupomuscular");
    const gruposOptions = (grupos.items || grupos)
      .map((g: any) => `<option value="${g.id}">${g.nombre}</option>`)
      .join("");

    // 🧱 Construcción del formulario
    const { value: formValues } = await Swal.fire({
      title: "🧩 Nueva Rutina",
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
        <form class="swal-form">
          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Nombre</label>
            <input id="nombre" type="text" class="form-control" placeholder="Nombre de la rutina">
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Objetivo</label>
            <textarea id="objetivo" class="form-control" placeholder="Ej: fuerza, tonificación, resistencia..."></textarea>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Grupo Muscular</label>
            <select id="grupoMuscularId" class="form-select">
              <option value="">Seleccione un grupo</option>
              ${gruposOptions}
            </select>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Imagen (opcional)</label>
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

    // 🚫 Cancelado
    if (!formValues) return;

    // 📦 Crear FormData (multipart/form-data)
    const formData = new FormData();
    formData.append("Nombre", formValues.nombre);
    formData.append("Objetivo", formValues.objetivo);
    formData.append("GrupoMuscularId", formValues.grupoMuscularId);
    if (formValues.imagen) {
      formData.append("Imagen", formValues.imagen);
    }

    // 📨 Enviar al backend
    await gymApi.post("/rutinasplantilla", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    Swal.fire({
      icon: "success",
      title: "Rutina creada",
      timer: 1200,
      showConfirmButton: false,
    });

    if (onSuccess) onSuccess();
  } catch (error: any) {
    console.error(error);
    const msg =
      error.response?.data || "No se pudo crear la rutina. Verificá los datos.";
    Swal.fire("Error", msg, "error");
  }
}
