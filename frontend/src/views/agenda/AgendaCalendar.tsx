import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin, { DateClickArg } from "@fullcalendar/interaction";
import { EventClickArg } from "@fullcalendar/core";
import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

export default function AgendaCalendar() {
  const [eventos, setEventos] = useState<any[]>([]);

  const cargarTurnos = async () => {
    try {
      const { data } = await gymApi.get("/turnosplantilla/activos");
      const eventos = data.map((t: any) => ({
        id: t.id.toString(),
        title: `Sala ${t.salaId || ""} - Cupo ${t.cupo}`,
        daysOfWeek: [Number(t.diaSemanaId)],
        startTime: t.horaInicio,
        duration: `${t.duracionMin}:00`,
        backgroundColor: "#4CAF50",
        borderColor: "#388E3C",
      }));
      setEventos(eventos);
    } catch (error) {
      console.error(error);
      Swal.fire("Error", "No se pudieron cargar los turnos.", "error");
    }
  };

  useEffect(() => {
    cargarTurnos();
  }, []);

  const handleDateClick = (info: DateClickArg) => {
    Swal.fire({
      title: "Nuevo turno",
      text: `¿Crear turno para el ${info.dateStr}?`,
      showCancelButton: true,
    });
  };

  const handleEventClick = async (info: EventClickArg) => {
    const turnoId = info.event.id;
    const { isConfirmed } = await Swal.fire({
      title: "Reservar turno",
      text: `¿Deseas reservar este turno?`,
      icon: "question",
      showCancelButton: true,
    });
    if (isConfirmed) {
      await gymApi.post("/suscripcionesturno", {
        turnoPlantillaId: turnoId,
        suscripcionId: 1,
      });
      Swal.fire("Hecho", "Turno reservado correctamente", "success");
    }
  };

  return (
    <div className="container mt-4">
      <h2 className="text-center mb-3">Agenda Semanal</h2>
      <FullCalendar
        plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
        initialView="timeGridWeek"
        locale="es"
        editable={true}
        selectable={true}
        events={eventos}
        dateClick={handleDateClick}
        eventClick={handleEventClick}
      />
    </div>
  );
}
