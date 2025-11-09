// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function EjercicioCreateSwal(onSuccess?: () => void) {
  try {
    // 🔹 Cargar grupos musculares
    const { data: gruposMusculares } = await gymApi.get("/grupomuscular");

    // 🧱 Construir formulario
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
            <input id="nombre" type="text" class="form-control" placeholder="Nombre del ejercicio">
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Tips</label>
            <textarea id="tips" class="form-control" placeholder="Consejos o notas opcionales"></textarea>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Grupo Muscular</label>
            <select id="grupoMuscularId" class="form-select">
              <option value="">Seleccione un grupo</option>
              ${(gruposMusculares.items || gruposMusculares)
                .map((g: any) => `<option value="${g.id}">${g.nombre}</option>`)
                .join("")}
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
        const tips = document.getElementById("tips").value.trim();
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

        return { nombre, tips, grupoMuscularId, imagen };
      },
    });

    // 🚫 Cancelado
    if (!formValues) return;

    // 📦 Crear FormData (multipart/form-data)
    const formData = new FormData();
    formData.append("Nombre", formValues.nombre);
    formData.append("Tips", formValues.tips);
    formData.append("GrupoMuscularId", formValues.grupoMuscularId);
    if (formValues.imagen) {
      formData.append("Imagen", formValues.imagen);
    }

    // 📨 Enviar al backend
    await gymApi.post("/ejercicios", formData, {
      headers: { "Content-Type": "multipart/form-data" },
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
    const msg =
      error.response?.data || "No se pudo crear el ejercicio";
    Swal.fire("Error", msg, "error");
  }
}
