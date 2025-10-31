import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

/**
 * Muestra un modal para reagendar un turno dentro de la semana actual.
 * Retorna `true` si el turno fue reagendado correctamente.
 */
export async function reagendarTurnoModal(
  suscripcionId: number,
  turnoActualId: number,
  turnoPlantillaId: number,
  fetchTurnos: () => void
) {
  try {
    const hoy = new Date();
    const finSemana = new Date(hoy);
    finSemana.setDate(hoy.getDate() + (7 - hoy.getDay()));

    // 🔹 Obtener turnos activos
    const { data } = await gymApi.get(`/turnosplantilla/activos`);
    const disponibles = (data.items || data).filter((turno: any) => {
      const fechaTurno = new Date(turno.horaInicio);
      return fechaTurno >= hoy && fechaTurno <= finSemana;
    });

    if (!disponibles.length) {
      await Swal.fire("Sin turnos", "No hay turnos disponibles para esta semana.", "info");
      return false;
    }

    const opciones = disponibles
      .filter(
        (turno: any) =>
          turno.sala?.cupoDisponible > 0 &&
          turno.id !== turnoPlantillaId
      )
      .map(
        (turno: any) =>
          `<option value="${turno.id}">
            ${turno.diaSemana?.nombre} ${turno.horaInicio.slice(0, 5)} (${turno.sala?.nombre})
          </option>`
      )
      .join("");

    // 🔹 Modal de selección
    const { value: nuevoTurnoId } = await Swal.fire({
      title: "🔁 Reagendar turno",
      html: `
        <label><b>Nuevo turno disponible (esta semana):</b></label><br/>
        <select id="nuevo-turno" class="swal2-select" style="width:100%; padding:8px;">
          <option value="">Seleccionar turno</option>
          ${opciones}
        </select>
      `,
      showCancelButton: true,
      confirmButtonText: "Reagendar",
      cancelButtonText: "Cancelar",
      preConfirm: () => {
        const select = document.getElementById("nuevo-turno") as HTMLSelectElement;
        return select?.value;
      },
    });

    if (!nuevoTurnoId) return false;

    const payload = {
      suscripcionId,
      turnoActualId,
      nuevoTurnoId: parseInt(nuevoTurnoId),
    };

    const res = await gymApi.post("/suscripcionturno/reagendar", payload);
    await Swal.fire("✅ Reagendado", res.data.message, "success");

    fetchTurnos(); // recarga los turnos
    return true;
  } catch (err: any) {
    const msg = err.response?.data?.message || "Error al reagendar turno.";
    await Swal.fire("⚠️ Error", msg, "warning");
    return false;
  }
}
