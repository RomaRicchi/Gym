// @ts-nocheck
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-rutina.css"; // estilo naranja global (ajustá la ruta si no existe)

export async function RutinaPlantillaEjercicioEditSwal(id: number, onSuccess?: () => void) {
  try {
    // 🔹 Obtener datos del registro y catálogos
    const [
      { data: item },
      { data: rutinasRes },
      { data: ejerciciosRes },
    ] = await Promise.all([
      gymApi.get(`/rutinasplantillaejercicios/${id}`),
      gymApi.get("/rutinasplantilla?page=1&pageSize=100"),
      gymApi.get("/ejercicios?page=1&pageSize=100"),
    ]);

    const rutinas = rutinasRes.items || rutinasRes;
    const ejercicios = ejerciciosRes.items || ejerciciosRes;

    // 🧡 Modal unificado
    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Ejercicio de Rutina",
      width: 650,
      html: `
        <div class="container-fluid text-start">
          <div class="mb-2">
            <label for="rutina" class="form-label fw-semibold">Rutina</label>
            <select id="rutina" class="form-select">
              <option value="">Seleccionar rutina...</option>
              ${rutinas
                .map(
                  (r: any) =>
                    `<option value="${r.id}" ${r.id === item.rutinaId ? "selected" : ""}>${r.nombre}</option>`
                )
                .join("")}
            </select>
          </div>

          <div class="mb-2">
            <label for="ejercicio" class="form-label fw-semibold">Ejercicio</label>
            <select id="ejercicio" class="form-select">
              <option value="">Seleccionar ejercicio...</option>
              ${ejercicios
                .map(
                  (e: any) =>
                    `<option value="${e.id}" ${e.id === item.ejercicioId ? "selected" : ""}>${e.nombre}</option>`
                )
                .join("")}
            </select>
          </div>

          <div class="row g-2 mt-2">
            <div class="col-md-6">
              <label for="orden" class="form-label fw-semibold">Orden</label>
              <input id="orden" type="number" min="1" class="form-control" value="${item.orden || ""}" />
            </div>
            <div class="col-md-6">
              <label for="series" class="form-label fw-semibold">Series</label>
              <input id="series" type="number" min="1" class="form-control" value="${item.series || ""}" />
            </div>
          </div>

          <div class="row g-2 mt-2">
            <div class="col-md-6">
              <label for="reps" class="form-label fw-semibold">Repeticiones</label>
              <input id="reps" type="number" min="1" class="form-control" value="${item.repeticiones || ""}" />
            </div>
            <div class="col-md-6">
              <label for="descanso" class="form-label fw-semibold">Descanso (seg)</label>
              <input id="descanso" type="number" min="0" class="form-control" value="${item.descansoSeg || ""}" />
            </div>
          </div>
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      buttonsStyling: false,
      focusConfirm: false,
      preConfirm: () => {
        const rutinaId = (document.getElementById("rutina") as HTMLSelectElement).value;
        const ejercicioId = (document.getElementById("ejercicio") as HTMLSelectElement).value;
        const orden = (document.getElementById("orden") as HTMLInputElement).value;
        const series = (document.getElementById("series") as HTMLInputElement).value;
        const repeticiones = (document.getElementById("reps") as HTMLInputElement).value;
        const descansoSeg = (document.getElementById("descanso") as HTMLInputElement).value;

        if (!rutinaId || !ejercicioId)
          return Swal.showValidationMessage("⚠️ Seleccioná una rutina y un ejercicio.");

        return { rutinaId, ejercicioId, orden, series, repeticiones, descansoSeg };
      },
    });

    if (!formValues) return;

    // ✅ Enviar actualización
    await gymApi.put(`/rutinasplantillaejercicios/${id}`, {
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
      text: "Ejercicio actualizado correctamente.",
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange",
      },
      buttonsStyling: false,
      timer: 1500,
      showConfirmButton: false,
    });

    onSuccess?.();
  } catch (err) {
    console.error("❌ Error al editar ejercicio de rutina:", err);
    await Swal.fire({
      icon: "error",
      title: "❌ Error",
      text: "No se pudo actualizar el registro.",
      customClass: {
        popup: "swal2-card-style",
        confirmButton: "btn btn-orange",
        cancelButton: "btn btn-secondary",
      },
      buttonsStyling: false,
    });
  }
}
