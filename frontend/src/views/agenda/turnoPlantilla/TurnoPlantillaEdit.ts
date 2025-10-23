import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function editarTurnoPlantilla(id: number, onSuccess?: () => void) {
  try {
    const [{ data: turno }, { data: salasRes }, { data: personalRes }] = await Promise.all([
      gymApi.get(`/turnosplantilla/${id}`),
      gymApi.get("/salas"),
      gymApi.get("/personal"),
    ]);

    const salas = salasRes.items || salasRes;
    const personal = personalRes.items || personalRes;
    const dias = [
      "Lunes",
      "Martes",
      "Miércoles",
      "Jueves",
      "Viernes",
      "Sábado",
      "Domingo",
    ];

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Turno",
      width: 650,
      customClass: { popup: "swal2-card-style" },
      html: `
        <style>
          .swal2-html-container .row { margin-bottom: 0.75rem; }
          .swal2-html-container label { font-weight: 600; margin-bottom: 0.25rem; }
        </style>

        <div class="row">
          <div class="col-md-6">
            <label>Sala</label>
            <select id="sala" class="form-select">
              ${salas
                .map(
                  (s: any) =>
                    `<option value="${s.id}" ${
                      s.id === turno.sala_id ? "selected" : ""
                    }>${s.nombre}</option>`
                )
                .join("")}
            </select>
          </div>
          <div class="col-md-6">
            <label>Profesor</label>
            <select id="personal" class="form-select">
              ${personal
                .map(
                  (p: any) =>
                    `<option value="${p.id}" ${
                      p.id === turno.personal_id ? "selected" : ""
                    }>${p.nombre}</option>`
                )
                .join("")}
            </select>
          </div>
        </div>

        <div class="row">
          <div class="col-md-4">
            <label>Día</label>
            <select id="dia" class="form-select">
              ${dias
                .map(
                  (d, i) =>
                    `<option value="${i + 1}" ${
                      i + 1 === turno.dia_semana_id ? "selected" : ""
                    }>${d}</option>`
                )
                .join("")}
            </select>
          </div>
          <div class="col-md-4">
            <label>Hora de inicio</label>
            <input type="time" id="hora" class="form-control" value="${
              turno.hora_inicio?.substring(0, 5) || ""
            }">
          </div>
          <div class="col-md-4">
            <label>Duración (min)</label>
            <input type="number" id="duracion" class="form-control" min="10" value="${
              turno.duracion_min || 60
            }">
          </div>
        </div>

        <div class="row">
          <div class="col-md-6">
            <label>Cupo máximo</label>
            <input type="number" id="cupo" class="form-control" min="1" value="${
              turno.cupo || 20
            }">
          </div>
          <div class="col-md-6 d-flex align-items-center justify-content-start gap-2 mt-4">
            <input type="checkbox" id="activo" ${
              turno.activo ? "checked" : ""
            }>
            <label for="activo">Activo</label>
          </div>
        </div>
      `,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      preConfirm: () => {
        const sala_id = (document.getElementById("sala") as HTMLSelectElement).value;
        const personal_id = (document.getElementById("personal") as HTMLSelectElement).value;
        const dia_semana_id = (document.getElementById("dia") as HTMLSelectElement).value;
        const hora_inicio = (document.getElementById("hora") as HTMLInputElement).value;
        const duracion_min = parseInt(
          (document.getElementById("duracion") as HTMLInputElement).value
        );
        const cupo = parseInt(
          (document.getElementById("cupo") as HTMLInputElement).value
        );
        const activo = (document.getElementById("activo") as HTMLInputElement).checked;

        if (!hora_inicio || duracion_min < 10 || cupo < 1) {
          Swal.showValidationMessage("⚠️ Complete los campos correctamente");
          return false;
        }

        return {
          sala_id: Number(sala_id),
          personal_id: Number(personal_id),
          dia_semana_id: Number(dia_semana_id),
          hora_inicio,
          duracion_min,
          cupo,
          activo,
        };
      },
    });

    if (!formValues) return;

    // Normaliza la hora al formato "HH:mm:ss"
    const hora_normalizada =
      formValues.hora_inicio.length === 5
        ? `${formValues.hora_inicio}:00`
        : formValues.hora_inicio;
        
    await gymApi.put(`/turnosplantilla/${id}`, {
        Id: id,
        SalaId: formValues.sala_id,
        PersonalId: formValues.personal_id,
        DiaSemanaId: formValues.dia_semana_id,
        HoraInicio: hora_normalizada,
        DuracionMin: formValues.duracion_min,
        Cupo: formValues.cupo,
        Activo: formValues.activo,
    });


    Swal.fire("✅ Actualizado", "El turno fue modificado correctamente", "success");
    if (onSuccess) onSuccess();
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudo editar el turno", "error");
  }
}
