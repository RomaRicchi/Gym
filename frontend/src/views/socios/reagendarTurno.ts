import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/asignarTurnos.css"; // ✅ usa los mismos estilos naranjas

/**
 * Modal unificado de reagendado (idéntico en diseño al de asignarTurnos)
 */
export async function reagendarTurnoModal(
  suscripcionId: number,
  turnoActualId: number,
  turnoPlantillaId: number,
  fetchTurnos: () => void
) {
  try {
    // 📅 Obtener lista de días
    const { data: diasRes } = await gymApi.get("/diasemana");
    const dias = diasRes.items || diasRes;

    // 🪄 Modal principal
    await Swal.fire({
      title: "🔁 Reagendar Turno",
      width: 600,
      showCancelButton: true,
      cancelButtonText: "Cerrar",
      customClass: { popup: "swal2-card-style" },
      html: `
        <div class="turnos-modal">
          <p><strong>Elegí un nuevo día y horario para tu turno:</strong></p>
          <hr/>

          <div class="turno-grupo">
            <label><b>Seleccionar día</b></label>
            <select id="select-dia" class="turno-input">
              <option value="">-- Elegí un día --</option>
              ${dias.map((d: any) => `<option value="${d.id}">${d.nombre}</option>`).join("")}
            </select>

            <label><b>Turno disponible</b></label>
            <select id="select-turno" class="turno-input">
              <option value="">Seleccione un día primero</option>
            </select>

            <button id="btn-reagendar" class="turno-btn">🔁 Reagendar</button>
          </div>
        </div>
      `,
      didOpen: () => {
        const diaSelect = document.getElementById("select-dia") as HTMLSelectElement;
        const turnoSelect = document.getElementById("select-turno") as HTMLSelectElement;
        const btnReagendar = document.getElementById("btn-reagendar") as HTMLButtonElement;

        // 📅 Cargar turnos del día
        diaSelect.addEventListener("change", async () => {
          const diaId = diaSelect.value;
          if (!diaId) return;
          turnoSelect.innerHTML = `<option>Cargando...</option>`;

          try {
            const { data } = await gymApi.get(`/turnosplantilla/dia/${diaId}`);
            const turnos = data.items || data;

            // filtrar el actual
            const disponibles = turnos.filter((t: any) => t.id !== turnoPlantillaId);

            turnoSelect.innerHTML = disponibles.length
              ? disponibles.map((t: any) => {
                  const id = t.id ?? t.Id;
                  const hora = t.horaInicio ?? t.HoraInicio ?? "--:--";
                  const profe = t.profesor ?? t.Personal?.Nombre ?? "(sin profesor)";
                  const sala = t.sala?.nombre ?? t.Sala?.Nombre ?? "Sala";
                  const cupoDisp = t.sala?.cupoDisponible ?? t.Sala?.CupoDisponible ?? 0;
                  const cupoTot = t.sala?.cupoTotal ?? t.Sala?.CupoTotal ?? 0;
                  return `<option value="${id}">
                    ${hora} hs - ${profe} (${sala}) | Cupo: ${cupoDisp}/${cupoTot}
                  </option>`;
                }).join("")
              : `<option>No hay turnos disponibles ese día</option>`;
          } catch {
            turnoSelect.innerHTML = `<option>Error al cargar turnos</option>`;
          }
        });

        // 💾 Reagendar turno
        btnReagendar.addEventListener("click", async () => {
          const nuevoTurnoId = parseInt(turnoSelect.value || "0", 10);
          if (!nuevoTurnoId) {
            Swal.fire("⚠️ Atención", "Seleccioná un turno válido", "warning");
            return;
          }

          try {
            const payload = {
              suscripcionId,
              turnoActualId,
              nuevoTurnoId,
            };
            const res = await gymApi.post("/suscripcionturno/reagendar", payload);

            btnReagendar.textContent = "✅ Reagendado";
            btnReagendar.classList.add("guardado");
            btnReagendar.disabled = true;

            await Swal.fire({
              icon: "success",
              title: "Turno reagendado",
              text: res.data.message || "El turno fue cambiado correctamente.",
              confirmButtonColor: "#ff6b00",
            });

            fetchTurnos();
          } catch (error: any) {
            const msg = error.response?.data?.message;
            Swal.fire({
              icon: "error",
              title: "Error",
              text: msg || "No se pudo reagendar el turno.",
              confirmButtonColor: "#ff6600",
            });
          }
        });
      },
    });
  } catch (err) {
    console.error("Error en reagendarTurnoModal:", err);
    Swal.fire("Error", "No se pudieron cargar los datos.", "error");
  }
}
