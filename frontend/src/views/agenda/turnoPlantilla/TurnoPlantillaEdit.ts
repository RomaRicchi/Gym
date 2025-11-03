import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function editarTurnoPlantilla(id: number, onSuccess?: () => void) {
  try {
    const [
      { data: turno },
      { data: salasRes },
      { data: personalRes },
      { data: diasRes },
    ] = await Promise.all([
      gymApi.get(`/turnosplantilla/${id}`),
      gymApi.get("/salas"),
      gymApi.get("/personal"),
      gymApi.get("/diasemana"),
    ]);

    const salas = salasRes.items || salasRes;
    const personal = personalRes.items || personalRes;
    const dias =
      diasRes.items ||
      diasRes || [
        { id: 1, nombre: "Lunes" },
        { id: 2, nombre: "Martes" },
        { id: 3, nombre: "Miércoles" },
        { id: 4, nombre: "Jueves" },
        { id: 5, nombre: "Viernes" },
        { id: 6, nombre: "Sábado" },
        { id: 7, nombre: "Domingo" },
      ];

    const { value: formValues } = await Swal.fire({
      title: "✏️ Editar Turno",
      width: 650,
      customClass: { popup: "swal2-card-style" },
      html: `
        <div class="container-fluid text-start">
          <div class="row">
            <div class="col-12">
              <label for="SalaId">Sala</label>
              <select id="SalaId" class="form-select">
                ${salas
                  .map(
                    (s: any) =>
                      `<option value="${s.id}" ${
                        s.id === turno.sala?.id ? "selected" : ""
                      }>${s.nombre}</option>`
                  )
                  .join("")}
              </select>
            </div>
          </div>

          <div class="row mt-2">
            <div class="col-12">
              <label for="PersonalId">Profesor</label>
              <select id="PersonalId" class="form-select">
                ${personal
                  .map(
                    (p: any) =>
                      `<option value="${p.id}" ${
                        p.id === turno.personal?.id ? "selected" : ""
                      }>${p.nombre}</option>`
                  )
                  .join("")}
              </select>
            </div>
          </div>

          <div class="row mt-2">
            <div class="col-md-6">
              <label for="DiaSemanaId">Día</label>
              <select id="DiaSemanaId" class="form-select">
                ${dias
                  .map(
                    (d: any) =>
                      `<option value="${d.id}" ${
                        d.id === turno.diaSemana?.id ? "selected" : ""
                      }>${d.nombre}</option>`
                  )
                  .join("")}
              </select>
            </div>
            <div class="col-md-6">
              <label for="HoraInicio">Hora Inicio</label>
              <input
                id="HoraInicio"
                type="time"
                class="form-control"
                value="${turno.horaInicio?.substring(0, 5) || ""}"
              />
            </div>
          </div>

          <div class="row mt-2">
            <div class="col-md-6">
              <label for="DuracionMin">Duración (min)</label>
              <input
                id="DuracionMin"
                type="number"
                min="10"
                class="form-control"
                value="${turno.duracionMin || 60}"
              />
            </div>
          </div>
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      preConfirm: () => {
        const data = {
          Id: id,
          SalaId: Number(
            (document.getElementById("SalaId") as HTMLSelectElement).value
          ),
          PersonalId: Number(
            (document.getElementById("PersonalId") as HTMLSelectElement).value
          ),
          DiaSemanaId: Number(
            (document.getElementById("DiaSemanaId") as HTMLSelectElement).value
          ),
          HoraInicio:
            (document.getElementById("HoraInicio") as HTMLInputElement).value +
            ":00",
          DuracionMin: Number(
            (document.getElementById("DuracionMin") as HTMLInputElement).value
          ),
        };

        if (!data.SalaId || !data.PersonalId || !data.DiaSemanaId)
          return Swal.showValidationMessage(
            "Complete todos los campos obligatorios"
          );
        if (!data.HoraInicio)
          return Swal.showValidationMessage("Debe especificar la hora de inicio");
        if (data.DuracionMin <= 0)
          return Swal.showValidationMessage(
            "La duración debe ser mayor que 0"
          );

        return data;
      },
    });

    if (!formValues) return;

    await gymApi.put(`/turnosplantilla/${id}`, formValues);
    await Swal.fire("✅ Actualizado", "El turno fue modificado correctamente", "success");
    onSuccess?.();
  } catch (err: any) {
    console.error("❌ Error al editar turno:", err);
    const msg = err.response?.data?.message || "No se pudo editar el turno";
    Swal.fire("❌ Error", msg, "error");
  }
}
