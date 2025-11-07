// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function RutinaPlantillaCreateSwal(onSuccess?: () => void) {
  try {
    const { data: gruposMusculares } = await gymApi.get("/grupomuscular");

    const { value: formValues } = await Swal.fire({
      title: "➕ Nueva Rutina",
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
        <div class="swal-form">
          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Nombre</label>
            <input id="nombre" type="text" class="form-control" placeholder="Nombre de la rutina">
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Objetivo</label>
            <textarea id="objetivo" class="form-control" placeholder="Objetivo o descripción (opcional)"></textarea>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">Grupo Muscular</label>
            <select id="grupoMuscularId" class="form-select">
              <option value="">Seleccione un grupo</option>
              ${gruposMusculares.map((g: any) => `<option value="${g.id}">${g.nombre}</option>`).join("")}
            </select>
          </div>

          <div class="mb-3 text-start">
            <label class="form-label fw-semibold">URL de Imagen (opcional)</label>
            <input id="imagenUrl" type="text" class="form-control" placeholder="uploads/rutinas/pecho.png">
          </div>
        </div>
      `,
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value;
        const objetivo = (document.getElementById("objetivo") as HTMLTextAreaElement).value;
        const grupoMuscularId = parseInt((document.getElementById("grupoMuscularId") as HTMLSelectElement).value);
        const imagenUrl = (document.getElementById("imagenUrl") as HTMLInputElement).value;

        if (!nombre.trim()) {
          Swal.showValidationMessage("El nombre es obligatorio");
          return false;
        }
        if (isNaN(grupoMuscularId)) {
          Swal.showValidationMessage("Debe seleccionar un grupo muscular");
          return false;
        }
        return { nombre, objetivo, grupoMuscularId, imagenUrl };
      },
    });

    if (!formValues) return;

    await gymApi.post("/rutinasplantilla", formValues);

    Swal.fire({
      icon: "success",
      title: "Rutina creada correctamente",
      timer: 1200,
      showConfirmButton: false,
    });

    if (onSuccess) onSuccess();
  } catch (error) {
    console.error(error);
    Swal.fire("Error", "No se pudo crear la rutina", "error");
  }
}
