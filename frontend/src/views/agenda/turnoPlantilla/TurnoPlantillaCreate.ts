import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function crearTurnoPlantilla(onSuccess?: () => void) {
  try {
    const [{ data: salasRes }, { data: personalRes }] = await Promise.all([
      gymApi.get("/salas"),
      gymApi.get("/personal"),
    ]);

    const salas = salasRes.items || salasRes;
    const personal = personalRes.items || personalRes;
    const dias = ["Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo"];

    const { value: formValues } = await Swal.fire({
      title: "➕ Nuevo Turno",
      width: 650,
      customClass: { popup: "swal2-card-style" },
      html: `
        <style>
          .swal2-html-container .row {
            margin-bottom: 0.75rem;
          }
          .swal2-html-container label {
            font-weight: 600;
            margin-bottom: 0.25rem;
          }
          .swal2-html-container .form-control,
          .swal2-html-container .form-select {
            border-radius: 0.5rem;
            border: 1px solid #ccc;
            box-shadow: none;
            font-size: 0.95rem;
          }
          .swal2-html-container .form-control:focus,
          .swal2-html-container .form-select:focus {
            border-color: #ff8800;
            box-shadow: 0 0 4px #ff8800;
          }
          .swal2-html-container .form-check {
            display: flex;
            align-items: center;
            justify-content: flex-end;
            gap: 0.4rem;
            margin-top: 0.4rem;
          }
          .swal2-html-container .form-check-input {
            width: 18px;
            height: 18px;
            accent-color: #ff8800;
          }
        </style>

        <div class="container-fluid text-start">
          <div class="row">
            <div class="col-12">
              <label for="sala_id">Sala</label>
              <select id="sala_id" class="form-select">
                <option value="">Seleccionar sala...</option>
                ${salas.map((s: any) => `<option value="${s.id}">${s.nombre}</option>`).join("")}
              </select>
            </div>
          </div>

          <div class="row">
            <div class="col-12">
              <label for="personal_id">Profesor</label>
              <select id="personal_id" class="form-select">
                <option value="">Seleccionar profesor...</option>
                ${personal.map((p: any) => `<option value="${p.id}">${p.nombre}</option>`).join("")}
              </select>
            </div>
          </div>

          <div class="row">
            <div class="col-md-6">
              <label for="dia_semana_id">Día de la Semana</label>
              <select id="dia_semana_id" class="form-select">
                <option value="">Seleccionar día...</option>
                ${dias.map((d, i) => `<option value="${i + 1}">${d}</option>`).join("")}
              </select>
            </div>
            <div class="col-md-6">
              <label for="hora_inicio">Hora Inicio</label>
              <input id="hora_inicio" type="time" class="form-control" />
            </div>
          </div>

          <div class="row">
            <div class="col-md-6">
              <label for="duracion_min">Duración (min)</label>
              <input id="duracion_min" type="number" min="10" class="form-control" />
            </div>
            <div class="col-md-6">
              <label for="cupo">Cupo Máximo</label>
              <input id="cupo" type="number" min="1" class="form-control" />
            </div>
          </div>

          <div class="row">
            <div class="col-12 form-check">
              <input id="activo" type="checkbox" class="form-check-input" checked />
              <label for="activo" class="form-check-label">Activo</label>
            </div>
          </div>
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "Guardar",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      preConfirm: () => {
         const form = {
            SalaId: Number((document.getElementById("sala_id") as HTMLSelectElement).value),
            PersonalId: Number((document.getElementById("personal_id") as HTMLSelectElement).value),
            DiaSemanaId: Number((document.getElementById("dia_semana_id") as HTMLSelectElement).value),
            HoraInicio: (document.getElementById("hora_inicio") as HTMLInputElement).value,
            DuracionMin: Number((document.getElementById("duracion_min") as HTMLInputElement).value),
            Cupo: Number((document.getElementById("cupo") as HTMLInputElement).value),
            Activo: (document.getElementById("activo") as HTMLInputElement).checked,
        };

        if (!form.SalaId || !form.PersonalId || !form.DiaSemanaId)
            return Swal.showValidationMessage("Complete todos los campos obligatorios");
        if (!form.HoraInicio)
            return Swal.showValidationMessage("Debe especificar la hora de inicio");
        if (form.DuracionMin <= 0)
            return Swal.showValidationMessage("La duración debe ser mayor que 0");
        if (form.Cupo <= 0)
            return Swal.showValidationMessage("El cupo debe ser mayor que 0");

        return form;
        },

    });

    if (!formValues) return;

    await gymApi.post("/turnosplantilla", formValues);
    await Swal.fire("✅ Guardado", "Turno creado correctamente", "success");
    if (onSuccess) onSuccess();

  } catch (err) {
    console.error(err);
    Swal.fire("❌ Error", "No se pudo crear el turno", "error");
  }
}
