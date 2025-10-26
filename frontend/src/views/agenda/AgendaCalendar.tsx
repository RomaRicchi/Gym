import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin, { DateClickArg } from "@fullcalendar/interaction";
import { EventClickArg } from "@fullcalendar/core";
import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { crearTurnoPlantilla } from "@/views/agenda/turnoPlantilla/TurnoPlantillaCreate"; 
import "@/styles/AgendaCalendar.css";
interface TurnoPlantilla {
  id: number;
  sala: { id: number; nombre: string };
  personal: { id: number; nombre: string };
  diaSemana: { id: number; nombre: string };
  horaInicio: string;
  duracionMin: number;
  cupo: number;
}

interface Sala {
  id: number;
  nombre: string;
}

interface Profesor {
  id: number;
  nombre: string;
}

export default function AgendaCalendar() {
  const [eventos, setEventos] = useState<any[]>([]);
  const [salas, setSalas] = useState<Sala[]>([]);
  const [profesores, setProfesores] = useState<Profesor[]>([]);
  const [filtroSala, setFiltroSala] = useState<string>("todos");
  const [filtroProfesor, setFiltroProfesor] = useState<string>("todos");

  // 🔹 Cargar filtros
  const cargarFiltros = async () => {
    try {
      const [{ data: salasRes }, { data: profRes }] = await Promise.all([
        gymApi.get("/salas"),
        gymApi.get("/personal"),
      ]);
      setSalas(salasRes.items || salasRes);
      setProfesores(profRes.items || profRes);
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudieron cargar los filtros.", "error");
    }
  };

  // 🔹 Cargar turnos plantilla
  const cargarTurnosPlantilla = async () => {
    try {
      const { data } = await gymApi.get("/turnosplantilla/activos");

      let filtrados = data;
      if (filtroSala !== "todos") {
        filtrados = filtrados.filter((t: any) => t.salaId === Number(filtroSala));
      }
      if (filtroProfesor !== "todos") {
        filtrados = filtrados.filter((t: any) => t.personalId === Number(filtroProfesor));
      }

      const eventosMapeados = filtrados.map((t: TurnoPlantilla) => {
        const [hora, minuto] = t.horaInicio.split(":").map(Number);
        const duracionHoras = Math.floor(t.duracionMin / 60);
        const duracionMinutos = t.duracionMin % 60;

        const horaFin = hora + duracionHoras + Math.floor((minuto + duracionMinutos) / 60);
        const minutoFin = (minuto + duracionMinutos) % 60;

        return {
          id: t.id.toString(),
          title: `${t.sala?.nombre || "Sala"} — ${t.personal?.nombre || "Profesor"} (${t.cupo} disp.)`,
          daysOfWeek: [t.diaSemana?.id || 1],
          startTime: `${hora.toString().padStart(2, "0")}:${minuto.toString().padStart(2, "0")}`,
          endTime: `${horaFin.toString().padStart(2, "0")}:${minutoFin.toString().padStart(2, "0")}`,
          backgroundColor: "#ff9800",
          borderColor: "#e65100",
          textColor: "#fff",
          extendedProps: {
            sala: t.sala?.nombre,
            profesor: t.personal?.nombre,
            duracion: t.duracionMin,
            cupo: t.cupo,
          },
        };
      });

      setEventos(eventosMapeados);
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudieron cargar los turnos plantilla.", "error");
    }
  };

  useEffect(() => {
    cargarFiltros();
  }, []);

  useEffect(() => {
    cargarTurnosPlantilla();
  }, [filtroSala, filtroProfesor]);

  // 🔸 Crear turno al hacer click en un día del calendario
  const handleDateClick = async (info: DateClickArg) => {
    const { isConfirmed } = await Swal.fire({
      title: "➕ Nuevo turno plantilla",
      text: `¿Deseas crear un turno para el ${info.date.toLocaleDateString()}?`,
      icon: "question",
      showCancelButton: true,
      confirmButtonText: "Sí, crear",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#ff6600",
    });

    if (isConfirmed) {
      await crearTurnoPlantilla(() => cargarTurnosPlantilla()); 
    }
  };

  // 🔸 Ver detalle de un turno existente
  const handleEventClick = async (info: EventClickArg) => {
    const { sala, profesor, duracion, cupo } = info.event.extendedProps;
    const horaInicio = info.event.startStr.slice(11, 16);
    const horaFin = info.event.endStr.slice(11, 16);

    Swal.fire({
      title: "🕓 Detalle del turno",
      html: `
        <p><strong>Sala:</strong> ${sala || "—"}</p>
        <p><strong>Profesor:</strong> ${profesor || "—"}</p>
        <p><strong>Horario:</strong> ${horaInicio} - ${horaFin}</p>
        <p><strong>Duración:</strong> ${duracion} min</p>
        <p><strong>Cupo disponible:</strong> ${cupo}</p>
      `,
      confirmButtonText: "Cerrar",
      confirmButtonColor: "#ff6600",
    });
  };

  return (
    <div className="agenda-calendar-container">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        CALENDARIO
      </h1>

      {/* 🎛️ Filtros */}
      <div className="agenda-filtros">
        <div>
          <label className="form-label fw-bold">Filtrar por sala</label>
          <select
            className="form-select"
            value={filtroSala}
            onChange={(e) => setFiltroSala(e.target.value)}
          >
            <option value="todos">Todas las salas</option>
            {salas.map((s) => (
              <option key={s.id} value={s.id}>
                {s.nombre}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label className="form-label fw-bold">Filtrar por profesor</label>
          <select
            className="form-select"
            value={filtroProfesor}
            onChange={(e) => setFiltroProfesor(e.target.value)}
          >
            <option value="todos">Todos los profesores</option>
            {profesores.map((p) => (
              <option key={p.id} value={p.id}>
                {p.nombre}
              </option>
            ))}
          </select>
        </div>
      </div>

      {/* 📅 Calendario */}
      <FullCalendar
        plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
        initialView="timeGridWeek"
        locale="es"
        allDaySlot={false}
        editable={false}
        selectable={true} // 👈 permite detectar clicks
        events={eventos}
        dateClick={handleDateClick}
        eventClick={handleEventClick}
        height="auto"
        slotMinTime="07:00:00"
        slotMaxTime="22:00:00"
        headerToolbar={{
          left: "prev,next today",
          center: "title",
          right: "",
        }}
      />
    </div>
  );
}
