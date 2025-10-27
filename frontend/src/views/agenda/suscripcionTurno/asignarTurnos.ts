import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

/**
 * Muestra un modal para asignar turnos a una suscripción.
 * Cada día del plan tiene su propio botón de guardado individual.
 * Cuando se cierra el modal, se ejecuta el callback onClose() para refrescar la lista.
 */
export async function asignarTurnos(suscripcion: any, onClose?: () => void) {
  try {
    // 🧩 1️⃣ Buscar el plan (por ID o nombre)
    let planData = null;

    if (suscripcion.plan_id) {
      const { data } = await gymApi.get(`/planes/${suscripcion.plan_id}`);
      planData = data;
    } else if (suscripcion.plan) {
      const { data } = await gymApi.get("/planes");
      const planes = data.items || data;
      planData = planes.find(
        (p: any) => p.nombre?.toLowerCase() === suscripcion.plan.toLowerCase()
      );
    }

    if (!planData) {
      Swal.fire("Error", "No se pudo obtener la información del plan.", "error");
      return;
    }

    // 🧠 2️⃣ Datos base
    const socioNombre = suscripcion.socio || "(sin socio)";
    const planNombre = planData.nombre;
    const diasPorSemana =
      planData.diasPorSemana || planData.dias_por_semana || 1;

    // 📅 3️⃣ Cargar días de la semana
    const { data: diasRes } = await gymApi.get("/diasemana");
    const dias = diasRes.items || diasRes;

    // 🔍 4️⃣ Obtener turnos ya asignados para evitar duplicados
    const { data: asignadosRes } = await gymApi.get(
      `/suscripcionturno/suscripcion/${suscripcion.id}`
    );
    const turnosAsignados = asignadosRes.map((t: any) => t.turnoPlantillaId);

    // 🧩 5️⃣ Generar los bloques según los días del plan
    const gruposHtml = Array.from({ length: diasPorSemana }, (_, i) => `
      <div class="turno-grupo" style="
        background: #fff;
        border-radius: 10px;
        padding: 15px;
        margin-bottom: 15px;
        box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        border-left: 5px solid #ff6600;
      ">
        <h6 style="color:#ff6600;margin-bottom:8px;">🗓️ Día ${i + 1}</h6>
        <label><b>Seleccionar día</b></label>
        <select id="dia-${i}" class="form-select swal2-input" style="margin-bottom:8px;">
          <option value="">-- Seleccionar día --</option>
          ${dias.map((d: any) => `<option value="${d.id}">${d.nombre}</option>`).join("")}
        </select>

        <label><b>Turno disponible</b></label>
        <select id="turno-${i}" class="form-select swal2-input" style="margin-bottom:8px;">
          <option value="">Seleccione un día primero</option>
        </select>

        <button id="btn-save-${i}" class="btn btn-sm" style="
          width:100%;
          background:#ff7b00;
          color:white;
          font-weight:600;
          border:none;
          border-radius:6px;
          padding:6px;
          margin-top:4px;
          cursor:pointer;
        ">💾 Guardar turno</button>
      </div>
    `).join("");

    // 🪄 6️⃣ Mostrar el modal
    await Swal.fire({
      title: "➕ Asignar Turnos",
      width: 650,
      showCancelButton: true,
      cancelButtonText: "Cerrar",
      customClass: { popup: "swal2-card-style" },
      html: `
        <div style="text-align:left; font-size:15px;">
          <p><strong>Socio:</strong> ${socioNombre}</p>
          <p><strong>Plan:</strong> ${planNombre}</p>
          <p><strong>Días por semana:</strong> ${diasPorSemana}</p>
          <hr/>
          ${gruposHtml}
        </div>
      `,
      didOpen: () => {
        for (let i = 0; i < diasPorSemana; i++) {
          const diaSelect = document.getElementById(`dia-${i}`) as HTMLSelectElement;
          const turnoSelect = document.getElementById(`turno-${i}`) as HTMLSelectElement;
          const btnSave = document.getElementById(`btn-save-${i}`) as HTMLButtonElement;

          // 📅 Cargar turnos al seleccionar día
          diaSelect.addEventListener("change", async () => {
            const diaId = diaSelect.value;
            if (!diaId) return;
            turnoSelect.innerHTML = `<option>Cargando...</option>`;

            try {
              const { data } = await gymApi.get(`/turnosPlantilla/dia/${diaId}`);
              const turnos = data.items || data;

              // 🚫 Filtrar turnos ya asignados
              const disponibles = turnos.filter(
                (t: any) => !turnosAsignados.includes(t.id)
              );

              turnoSelect.innerHTML = disponibles.length
                ? disponibles.map((t: any) => {
                    const hora = t.hora_inicio || t.horaInicio || "--:--";
                    const duracion = t.duracion_min || t.duracionMin || "?";
                    const sala = t.sala?.nombre || "Sala";
                    const cupo =
                      t.sala?.cupoDisponible ?? t.sala?.cupoTotal ?? "?";
                    return `<option value="${t.id}">
                      ${hora} (${duracion} min, ${sala}, cupo ${cupo})
                    </option>`;
                  }).join("")
                : `<option>No hay turnos disponibles</option>`;
            } catch (err) {
              console.error("Error al cargar turnos:", err);
              turnoSelect.innerHTML = `<option>Error al cargar turnos</option>`;
            }
          });

          // 💾 Guardar turno individual
          btnSave.addEventListener("click", async () => {
            const diaId = diaSelect.value;
            const turnoId = turnoSelect.value;

            if (!diaId || !turnoId) {
              Swal.fire(
                "⚠️ Atención",
                `Seleccioná día y turno válidos para el grupo ${i + 1}`,
                "warning"
              );
              return;
            }

            try {
              console.log("Enviando turno:", {
                suscripcionId: suscripcion.id,
                turnoPlantillaId: parseInt(turnoId),
              });

              await gymApi.post("/suscripcionturno", {
                suscripcionId: suscripcion.id,
                turnoPlantillaId: parseInt(turnoId),
              });

              btnSave.textContent = "✅ Guardado";
              btnSave.style.background = "#28a745";
              btnSave.disabled = true;
            } catch (error: any) {
              const status = error.response?.status;
              const msg = error.response?.data?.message;

              console.error("Error al guardar turno:", msg || error.message);

              if (status === 409) {
                Swal.fire({
                  icon: "warning",
                  title: "Turno duplicado",
                  text: "Este turno ya fue asignado a esta suscripción.",
                  confirmButtonColor: "#ff6600",
                });
              } else if (status === 400) {
                Swal.fire({
                  icon: "warning",
                  title: "Datos inválidos",
                  text: msg || "Verificá los datos enviados.",
                  confirmButtonColor: "#ff6600",
                });
              } else {
                Swal.fire({
                  icon: "error",
                  title: "Error",
                  text: msg || "No se pudo guardar el turno seleccionado.",
                  confirmButtonColor: "#ff6600",
                });
              }
            }
          });
        }
      },
    }).then(() => {
      if (onClose) onClose(); // refresca la lista al cerrar el modal
    });
  } catch (err) {
    console.error("Error global en asignarTurnos:", err);
    Swal.fire("Error", "No se pudieron cargar los datos del plan o turnos.", "error");
  }
}
