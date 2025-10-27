import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export async function editarTurnoPlantilla(id: number, onSuccess?: () => void) {
  try {
    const [{ data: turno }, { data: salasRes }, { data: personalRes }, { data: diasRes }] = await Promise.all([
      gymApi.get(`/turnosplantilla/${id}`),
      gymApi.get("/salas"),
      gymApi.get("/personal"),
      gymApi.get("/dia_semana"), // ✅ si lo tenés en el backend
    ]);

    const salas = salasRes.items || salasRes;
    const personal = personalRes.items || personalRes;
    const dias = diasRes.items || diasRes || [
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
        <style>
          .swal2-html-container .row { margin-bottom: 0.75rem; }
          .swal2-html-container label { font-weight: 600; margin-bottom: 0.25rem; }
        </style>

        <div class="row">
          <div class="col-md-6">
            <label>Sala</label>
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
          <div class="col-md-6">
            <label>Profesor</label>
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

        <div class="row">
          <div class="col-md-4">
            <label>Día</label>
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
          <div class="col-md-4">
            <label>Hora de inicio</label>
            <input type="time" id="HoraInicio" class="form-control" value="${
              turno.horaInicio?.substring(0, 5) || ""
            }">
          </div>
          <div class="col-md-4">
            <label>Duración (min)</label>
            <input type="number" id="DuracionMin" class="form-control" min="10" value="${
              turno.duracionMin || 60
            }">
          </div>
        </div>

        <div class="row">
          <div class="col-12 d-flex align-items-center justify-content-start gap-2 mt-3">
            <input type="checkbox" id="Activo" ${
              turno.activo ? "checked" : ""
            }>
            <label for="Activo">Activo</label>
          </div>
        </div>
      `,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: "💾 Guardar cambios",
      cancelButtonText: "Cancelar",
      preConfirm: () => {
        const data = {
          Id: id,
          SalaId: Number((document.getElementById("SalaId") as HTMLSelectElement).value),
          PersonalId: Number((document.getElementById("PersonalId") as HTMLSelectElement).value),
          DiaSemanaId: Number((document.getElementById("DiaSemanaId") as HTMLSelectElement).value),
          HoraInicio: (document.getElementById("HoraInicio") as HTMLInputElement).value + ":00",
          DuracionMin: Number((document.getElementById("DuracionMin") as HTMLInputElement).value),
          Activo: (document.getElementById("Activo") as HTMLInputElement).checked,
        };

        if (!data.SalaId || !data.PersonalId || !data.DiaSemanaId)
          return Swal.showValidationMessage("Complete todos los campos obligatorios");
        if (!data.HoraInicio)
          return Swal.showValidationMessage("Debe especificar la hora de inicio");
        if (data.DuracionMin < 10)
          return Swal.showValidationMessage("La duración debe ser mayor que 10 minutos");

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
