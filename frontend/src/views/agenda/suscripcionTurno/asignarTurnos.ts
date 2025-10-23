import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

/**
 * Muestra un modal para asignar turnos a una suscripción.
 * Genera selects dinámicos según los días del plan.
 */
export async function asignarTurnos(suscripcion: any) {
  try {
    // 1️⃣ Obtener los días de la semana
    const { data: diasRes } = await gymApi.get("/diasemana");
    const dias = diasRes.items || diasRes;

    const planNombre = suscripcion.plan?.nombre || suscripcion.plan || "(sin plan)";
    const diasPorSemana = suscripcion.diasPorSemana || suscripcion.plan?.dias_por_semana || 1;


    // 2️⃣ Generar los selects dinámicamente
    const inputsHtml = Array.from({ length: diasPorSemana }, (_, i) => {
      return `
        <div class="turno-grupo" style="margin-bottom:10px">
          <label><b>🗓️ Día ${i + 1}</b></label>
          <select id="dia-${i}" class="swal2-input">
            <option value="">Seleccionar día</option>
            ${dias.map((d: any) => `<option value="${d.id}">${d.nombre}</option>`).join("")}
          </select>

          <label><b>🕒 Turno disponible</b></label>
          <select id="turno-${i}" class="swal2-input">
            <option value="">Seleccione un día primero</option>
          </select>
        </div>
      `;
    }).join("");

    // 3️⃣ Mostrar modal principal
    const { value: formValues } = await Swal.fire({
      title: `➕ Asignar Turnos`,
      width: 650,
      confirmButtonText: "Guardar selección",
      showCancelButton: true,
      cancelButtonText: "Cancelar",
      customClass: { popup: "swal2-card-style" },
      html: `
        <style>
          .swal2-html-container label {
            font-weight: 600;
            display: block;
            margin-bottom: 4px;
          }
          .swal2-html-container select {
            width: 100%;
            margin-bottom: 8px;
          }
          .turno-grupo {
            border: 1px solid #eee;
            padding: 10px;
            border-radius: 8px;
            margin-bottom: 10px;
            background: #fff6ef;
          }
        </style>

        <div class="text-start">
          <p><strong>Socio:</strong> ${suscripcion.socio || "(sin socio)"}</p>
          <p><strong>Plan:</strong> ${planNombre}</p>
          <p><strong>Días por semana:</strong> ${diasPorSemana}</p>
          ${inputsHtml}
        </div>
      `,
      preConfirm: () => {
        const valores = [];
        for (let i = 0; i < diasPorSemana; i++) {
          const dia = (document.getElementById(`dia-${i}`) as HTMLSelectElement)?.value;
          const turno = (document.getElementById(`turno-${i}`) as HTMLSelectElement)?.value;
          if (!dia || !turno) {
            Swal.showValidationMessage(`Seleccioná día y turno en la fila ${i + 1}`);
            return false;
          }
          valores.push({ diaId: dia, turnoId: turno });
        }
        return valores;
      },
      didOpen: () => {
        // 4️⃣ Escuchar cambios en cada select de día
        for (let i = 0; i < diasPorSemana; i++) {
          const diaSelect = document.getElementById(`dia-${i}`) as HTMLSelectElement;
          const turnoSelect = document.getElementById(`turno-${i}`) as HTMLSelectElement;

          diaSelect.addEventListener("change", async (e) => {
            const diaId = (e.target as HTMLSelectElement).value;
            if (!diaId) return;
            turnoSelect.innerHTML = `<option>Cargando turnos...</option>`;

            try {
              const { data } = await gymApi.get(`/turnosplantilla/dia/${diaId}`);
              const turnos = data.items || data;

              turnoSelect.innerHTML = turnos.length
                ? turnos
                    .map(
                      (t: any) =>
                        `<option value="${t.id}">${t.horaInicio} - ${t.duracionMin} min (Cupo ${t.disponibles ?? t.cupo})</option>`
                    )
                    .join("")
                : `<option>No hay turnos disponibles</option>`;
            } catch (err) {
              console.error("Error al cargar turnos:", err);
              turnoSelect.innerHTML = `<option>Error al cargar</option>`;
            }
          });
        }
      },
    });

    // 5️⃣ Enviar la selección al backend
    if (formValues && Array.isArray(formValues)) {
      for (const f of formValues) {
        await gymApi.post("/suscripcionturno", {
          suscripcionId: suscripcion.id,
          turnoPlantillaId: f.turnoId,
        });
      }

      Swal.fire({
        icon: "success",
        title: "✅ Turnos asignados",
        text: "Se asignaron correctamente los turnos de la suscripción.",
        timer: 2000,
        showConfirmButton: false,
      });
    }
  } catch (err) {
    console.error(err);
    Swal.fire("Error", "No se pudieron cargar los turnos o días", "error");
  }
}
