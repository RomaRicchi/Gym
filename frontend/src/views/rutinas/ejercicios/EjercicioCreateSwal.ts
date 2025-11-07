// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function EjercicioCreateSwal(onSuccess?: () => void) {
  try {
    // 🔹 Cargar grupos musculares para el selector
    const { data: gruposMusculares } = await gymApi.get("/grupomuscular");

    // 🧱 Construcción del formulario HTML
    const { value: formValues } = await Swal.fire({
      title: "➕ Nuevo Ejercicio",
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
            <input id="nombre" type="text" class="form-control" 
              placeholder="Nombre del ejercicio">
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Tips</label>
            <textarea id="tips" class="form-control" 
              placeholder="Consejos o notas opcionales"></textarea>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Grupo Muscular</label>
            <select id="grupoMuscularId" class="form-select">
              <option value="">Seleccione un grupo</option>
              ${gruposMusculares
                .map(
                  (g: any) =>
                    `<option value="${g.id}">${g.nombre}</option>`
                )
                .join("")}
            </select>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">URL de Imagen (opcional)</label>
            <input id="mediaUrl" type="text" class="form-control" 
              placeholder="ej: uploads/ejercicios/press-banca.jpg">
          </div>
        </form>
      `,
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement)
          .value;
        const tips = (document.getElementById("tips") as HTMLTextAreaElement)
          .value;
        const grupoMuscularId = parseInt(
          (document.getElementById("grupoMuscularId") as HTMLSelectElement).value
        );
        const mediaUrl = (
          document.getElementById("mediaUrl") as HTMLInputElement
        ).value;

        if (!nombre.trim()) {
          Swal.showValidationMessage("El nombre es obligatorio");
          return false;
        }

        if (isNaN(grupoMuscularId)) {
          Swal.showValidationMessage("Debe seleccionar un grupo muscular");
          return false;
        }

        return { nombre, tips, grupoMuscularId, mediaUrl };
      },
    });

    // 🚫 Cancelado
    if (!formValues) return;

    // 📨 Enviar creación
    await gymApi.post("/ejercicios", {
      nombre: formValues.nombre,
      tips: formValues.tips,
      grupoMuscularId: formValues.grupoMuscularId,
      mediaUrl: formValues.mediaUrl || null,
    });

    Swal.fire({
      icon: "success",
      title: "Ejercicio creado",
      timer: 1200,
      showConfirmButton: false,
    });

    if (onSuccess) onSuccess();
  } catch (error: any) {
    console.error(error);
    Swal.fire("Error", "No se pudo crear el ejercicio", "error");
  }
}
