import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function RutinaPlantillaEjercicioCreateSwal(onSuccess?: () => void) {
  try {
    const [{ data: rutinasRes }, { data: ejerciciosRes }] = await Promise.all([
      gymApi.get("/rutinasplantilla?page=1&pageSize=100"),
      gymApi.get("/ejercicios?page=1&pageSize=100"),
    ]);

    const rutinas = rutinasRes.items || rutinasRes;
    const ejercicios = ejerciciosRes.items || ejerciciosRes;

    const { value: formValues } = await Swal.fire({
      title: "➕ Agregar Ejercicio a Rutina",
      width: 650,
      customClass: { popup: "swal2-card-style" },
      html: `
        <div class="container-fluid text-start">
          
          <!-- Rutina -->
          <div class="row">
            <div class="col-12">
              <label for="rutina">Rutina</label>
              <select id="rutina" class="form-select">
                <option value="">Seleccionar rutina...</option>
                ${rutinas.map((r: any) => `<option value="${r.id}">${r.nombre}</option>`).join("")}
              </select>
            </div>
          </div>

          <!-- Ejercicio -->
          <div class="row mt-2">
            <div class="col-12">
              <label for="ejercicio">Ejercicio</label>
              <select id="ejercicio" class="form-select">
                <option value="">Seleccionar ejercicio...</option>
                ${ejercicios.map((e: any) => `<option value="${e.id}">${e.nombre}</option>`).join("")}
              </select>
            </div>
          </div>

          <!-- Orden / Series -->
          <div class="row mt-2">
            <div class="col-md-6">
              <label for="orden">Orden</label>
              <input id="orden" type="number" min="1" class="form-control" placeholder="Orden">
            </div>
            <div class="col-md-6">
              <label for="series">Series</label>
              <input id="series" type="number" min="1" class="form-control" placeholder="Series">
            </div>
          </div>

          <!-- Repeticiones / Descanso -->
          <div class="row mt-2">
            <div class="col-md-6">
              <label for="reps">Repeticiones</label>
              <input id="reps" type="number" min="1" class="form-control" placeholder="Reps">
            </div>
            <div class="col-md-6">
              <label for="descanso">Descanso (seg)</label>
              <input id="descanso" type="number" min="0" class="form-control" placeholder="Ej: 60">
            </div>
          </div>

        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "Guardar",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      preConfirm: () => {
        const rutinaId = (document.getElementById("rutina") as HTMLSelectElement).value;
        const ejercicioId = (document.getElementById("ejercicio") as HTMLSelectElement).value;
        const orden = (document.getElementById("orden") as HTMLInputElement).value;
        const series = (document.getElementById("series") as HTMLInputElement).value;
        const repeticiones = (document.getElementById("reps") as HTMLInputElement).value;
        const descansoSeg = (document.getElementById("descanso") as HTMLInputElement).value;

        if (!rutinaId || !ejercicioId)
          return Swal.showValidationMessage("Seleccioná una rutina y un ejercicio");

        return { rutinaId, ejercicioId, orden, series, repeticiones, descansoSeg };
      },
    });

    if (!formValues) return;

    // Enviar datos
    await gymApi.post("/rutinasplantillaejercicios", {
      rutinaId: Number(formValues.rutinaId),
      ejercicioId: Number(formValues.ejercicioId),
      orden: Number(formValues.orden),
      series: Number(formValues.series),
      repeticiones: Number(formValues.repeticiones),
      descansoSeg: Number(formValues.descansoSeg),
    });

    await Swal.fire("✅ Guardado", "Ejercicio agregado correctamente", "success");
    onSuccess?.();
  } catch (err) {
    console.error("Error al crear ejercicio de rutina:", err);
    Swal.fire("❌ Error", "No se pudo crear el registro", "error");
  }
}
