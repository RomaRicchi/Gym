// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";

/**
 * Formulario para agregar un ejercicio a una rutina específica.
 * Si se pasa rutinaPreseleccionada, la rutina se fija y no se puede cambiar.
 */
export async function RutinaPlantillaEjercicioCreateSwal(onSuccess?: () => void, rutinaPreseleccionada?: { id: number, nombre: string }) {
  try {
    // 🔹 Cargar rutinas y ejercicios
    const [{ data: rutinasRes }, { data: ejerciciosRes }] = await Promise.all([
      gymApi.get("/rutinasplantilla?page=1&pageSize=100"),
      gymApi.get("/ejercicios?page=1&pageSize=100"),
    ]);

    const rutinas = rutinasRes.items || rutinasRes;
    const ejercicios = ejerciciosRes.items || ejerciciosRes;

    // === HTML dinámico ===
    const { value: formValues } = await Swal.fire({
      title: "➕ Agregar Ejercicio a Rutina",
      width: 650,
      html: `
        <div class="container-fluid text-start">
          
          <!-- Rutina -->
          <div class="mb-3">
            <label class="form-label fw-semibold">Rutina</label>
            <select id="rutina" class="form-select" ${rutinaPreseleccionada ? "disabled" : ""}>
              <option value="">Seleccionar rutina...</option>
              ${rutinas
                .map(
                  (r: any) =>
                    `<option value="${r.id}" ${rutinaPreseleccionada?.id === r.id ? "selected" : ""}>${r.nombre}</option>`
                )
                .join("")}
            </select>
          </div>

          <!-- Ejercicio -->
          <div class="mb-3">
            <label class="form-label fw-semibold">Ejercicio</label>
            <select id="ejercicio" class="form-select">
              <option value="">Seleccionar ejercicio...</option>
              ${ejercicios.map((e: any) => `<option value="${e.id}">${e.nombre}</option>`).join("")}
            </select>
          </div>

          <div class="row g-3">
            <div class="col-md-6">
              <label class="form-label fw-semibold">Orden</label>
              <input id="orden" type="number" min="1" class="form-control" placeholder="Ej: 1">
            </div>
            <div class="col-md-6">
              <label class="form-label fw-semibold">Series</label>
              <input id="series" type="number" min="1" class="form-control" placeholder="Ej: 3">
            </div>
            <div class="col-md-6">
              <label class="form-label fw-semibold">Repeticiones</label>
              <input id="reps" type="number" min="1" class="form-control" placeholder="Ej: 12">
            </div>
            <div class="col-md-6">
              <label class="form-label fw-semibold">Descanso (seg)</label>
              <input id="descanso" type="number" min="0" class="form-control" placeholder="Ej: 60">
            </div>
          </div>
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "Guardar",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      buttonsStyling: false,
      preConfirm: () => {
        const rutinaId = rutinaPreseleccionada
          ? rutinaPreseleccionada.id
          : (document.getElementById("rutina") as HTMLSelectElement).value;
        const ejercicioId = (document.getElementById("ejercicio") as HTMLSelectElement).value;
        const orden = (document.getElementById("orden") as HTMLInputElement).value;
        const series = (document.getElementById("series") as HTMLInputElement).value;
        const repeticiones = (document.getElementById("reps") as HTMLInputElement).value;
        const descansoSeg = (document.getElementById("descanso") as HTMLInputElement).value;

        if (!rutinaId || !ejercicioId) {
          Swal.showValidationMessage("Seleccioná una rutina y un ejercicio");
          return false;
        }

        return { rutinaId, ejercicioId, orden, series, repeticiones, descansoSeg };
      },
    });

    if (!formValues) return;

    // === Enviar datos ===
    await gymApi.post("/rutinasplantillaejercicios", {
      rutinaId: Number(formValues.rutinaId),
      ejercicioId: Number(formValues.ejercicioId),
      orden: Number(formValues.orden),
      series: Number(formValues.series),
      repeticiones: Number(formValues.repeticiones),
      descansoSeg: Number(formValues.descansoSeg),
    });

    await Swal.fire({
      icon: "success",
      title: "✅ Guardado",
      text: "Ejercicio agregado correctamente",
      customClass: { popup: "swal2-card-style" },
      buttonsStyling: false,
      timer: 1400,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err) {
    console.error("Error al crear ejercicio de rutina:", err);
    await Swal.fire({
      icon: "error",
      title: "❌ Error",
      text: "No se pudo crear el registro",
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange",
      },
      buttonsStyling: false,
    });
  }
}
