import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

export async function RutinaPlantillaCreateSwal(onSuccess?: () => void) {
  try {
    const { data: grupos } = await gymApi.get("/grupomuscular");
    const lista = grupos.items || grupos;

    const options = lista
      .map((g: any) => `<option value="${g.id}">${g.nombre}</option>`)
      .join("");

    const { value: formValues } = await Swal.fire({
      title: "➕ Nueva Rutina Plantilla",
      html: `
        <div class="mb-2 text-start">
          <label class="form-label fw-semibold">Nombre</label>
          <input id="nombre" class="form-control" placeholder="Nombre de la rutina" required />
        </div>
        <div class="mb-2 text-start">
          <label class="form-label fw-semibold">Objetivo</label>
          <input id="objetivo" class="form-control" placeholder="Ej: Tonificar, Fuerza, Resistencia" />
        </div>
        <div class="mb-2 text-start">
          <label class="form-label fw-semibold">Grupo Muscular</label>
          <select id="grupoMuscularId" class="form-select">
            <option value="" disabled selected>Seleccionar...</option>
            ${options}
          </select>
        </div>
        <div class="mb-2 text-start">
          <label class="form-label fw-semibold">Imagen URL</label>
          <input id="imagenUrl" class="form-control" placeholder="/img/pecho.png" />
        </div>
      `,
      confirmButtonText: "Guardar",
      showCancelButton: true,
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#ff6600",
      focusConfirm: false,
      preConfirm: () => {
        const nombre = (document.getElementById("nombre") as HTMLInputElement).value.trim();
        const objetivo = (document.getElementById("objetivo") as HTMLInputElement).value.trim();
        const grupoMuscularId = (document.getElementById("grupoMuscularId") as HTMLSelectElement).value;
        const imagenUrl = (document.getElementById("imagenUrl") as HTMLInputElement).value.trim();

        if (!nombre || !grupoMuscularId) {
          Swal.showValidationMessage("⚠️ Nombre y grupo muscular son obligatorios.");
          return false;
        }

        return { nombre, objetivo, grupoMuscularId: Number(grupoMuscularId), imagenUrl };
      },
    });

    if (formValues) {
      await gymApi.post("/rutinasplantilla", formValues);
      Swal.fire("✅ Guardado", "Rutina creada correctamente.", "success");
      onSuccess?.();
    }
  } catch (error) {
    console.error(error);
    Swal.fire("Error", "No se pudo crear la rutina.", "error");
  }
}
