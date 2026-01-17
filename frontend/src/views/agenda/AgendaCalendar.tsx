import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin, { DateClickArg } from "@fullcalendar/interaction";
import { EventClickArg } from "@fullcalendar/core";
import { useEffect, useState, useRef } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import { crearTurnoPlantilla } from "@/views/agenda/turnoPlantilla/TurnoPlantillaCreate";
import "@/styles/AgendaCalendar.css";

interface TurnoPlantilla {
  id: number;
  sala: { id: number; nombre: string; cupoTotal?: number; cupoDisponible?: number };
  personal: { id: number; nombre: string; id_personal?: number };
  diaSemana: { id: number; nombre: string };
  horaInicio: string;
  duracionMin: number;
  activo?: boolean;
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
  const calendarRef = useRef<any>(null);

  // 🔹 Detectar usuario logueado y rol
  const usuario = JSON.parse(localStorage.getItem("usuario") || "{}");
  const rol = usuario?.rol || "";
  const idPersonal = usuario?.id_personal || usuario?.personal_id || null;

  // Cargar filtros de salas y profesores
  const cargarFiltros = async () => {
    try {
      if (rol === "Profesor") {
        // Solo carga salas, los profesores no necesitan ver el filtro de personal
        const { data: salasRes } = await gymApi.get("/salas");
        setSalas(salasRes.items || salasRes);
        return;
      }

      // Para roles con permisos (Admin, Recepcionista)
      const [{ data: salasRes }, { data: profRes }] = await Promise.all([
        gymApi.get("/salas"),
        gymApi.get("/personal"),
      ]);

      setSalas(salasRes.items || salasRes);
      setProfesores(profRes.items || profRes);
    } catch (err) {
      console.error("❌ Error al cargar filtros:", err);
      Swal.fire("Error", "No se pudieron cargar los filtros.", "error");
    }
  };


  // Cargar turnos plantilla con cupos dinámicos
  const cargarTurnosPlantilla = async () => {
    try {
      const { data } = await gymApi.get("/turnosplantilla/activos");
      let turnos: TurnoPlantilla[] = data.items || data;

      // Si el usuario es profesor, filtrar solo sus turnos
      if (rol === "Profesor" && idPersonal) {
        turnos = turnos.filter((t) => t.personal?.id === idPersonal);
      }

      // Filtros activos
      let filtrados = turnos;
      if (filtroSala !== "todos") {
        filtrados = filtrados.filter((t) => t.sala?.id === Number(filtroSala));
      }
      if (filtroProfesor !== "todos") {
        filtrados = filtrados.filter((t) => t.personal?.id === Number(filtroProfesor));
      }

      // Mapeo a eventos del calendario
      const eventosMapeados = filtrados.map((t) => {
        const [hora, minuto] = t.horaInicio.split(":").map(Number);
        const duracionHoras = Math.floor(t.duracionMin / 60);
        const duracionMinutos = t.duracionMin % 60;

        const horaFin = hora + duracionHoras + Math.floor((minuto + duracionMinutos) / 60);
        const minutoFin = (minuto + duracionMinutos) % 60;

        // 🎨 Color según cupo
        const cupoTotal = t.sala?.cupoTotal ?? 0;
        const cupoDisp = t.sala?.cupoDisponible ?? cupoTotal;
        const porcentaje = cupoTotal ? (cupoDisp / cupoTotal) * 100 : 100;

        let color = "#4caf50"; // verde (disponible)
        if (porcentaje <= 30 && porcentaje > 0) color = "#ff9800"; // naranja
        if (cupoDisp <= 0) color = "#e53935"; // rojo (sin cupo)

        return {
          id: t.id.toString(),
          title: `${t.sala?.nombre || "Sala"} — ${t.personal?.nombre || "Profesor"}`,
          daysOfWeek: [t.diaSemana?.id || 1],
          startTime: `${hora.toString().padStart(2, "0")}:${minuto
            .toString()
            .padStart(2, "0")}`,
          endTime: `${horaFin.toString().padStart(2, "0")}:${minutoFin
            .toString()
            .padStart(2, "0")}`,
          backgroundColor: color,
          borderColor: color,
          textColor: "#fff",
          extendedProps: {
            sala: t.sala?.nombre,
            profesor: t.personal?.nombre,
            duracion: t.duracionMin,
            cupoTotal: cupoTotal,
            cupoDisponible: cupoDisp,
          },
        };
      });

      setEventos(eventosMapeados);
    } catch (err) {
      console.error("❌ Error al cargar turnos plantilla:", err);
      Swal.fire("Error", "No se pudieron cargar los turnos plantilla.", "error");
    }
  };

  useEffect(() => {
    cargarFiltros();
  }, []);

  useEffect(() => {
    cargarTurnosPlantilla();
  }, [filtroSala, filtroProfesor]);

  // 🧩 Crear turno nuevo (solo admin o recepcionista)
  const handleDateClick = async (info: DateClickArg) => {
    if (rol !== "Administrador" && rol !== "Recepcionista") return;

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

  // 🧩 Mostrar detalle del turno
  const handleEventClick = async (info: EventClickArg) => {
    const { sala, profesor, duracion, cupoTotal, cupoDisponible } =
      info.event.extendedProps;
    const horaInicio = info.event.startStr.slice(11, 16);
    const horaFin = info.event.endStr.slice(11, 16);

    const color =
      cupoDisponible <= 0 ? "red" : cupoDisponible <= 3 ? "orange" : "green";

    await Swal.fire({
      title: "🕓 Detalle del turno",
      html: `
        <p><strong>Sala:</strong> ${sala || "—"}</p>
        <p><strong>Profesor:</strong> ${profesor || "—"}</p>
        <p><strong>Horario:</strong> ${horaInicio} - ${horaFin}</p>
        <p><strong>Duración:</strong> ${duracion} min</p>
        <p><strong>Cupos:</strong> 
          <span style="color:${color}; font-weight:600;">
            ${cupoDisponible}/${cupoTotal}
          </span>
        </p>
      `,
      confirmButtonText: "Cerrar",
      confirmButtonColor: "#ff6600",
    });
  };

  // 🔄 Redimensionar dinámicamente
  useEffect(() => {
    const resizeCalendar = () => {
      const calendarApi = calendarRef.current?.getApi();
      if (calendarApi) {
        setTimeout(() => calendarApi.updateSize(), 300);
      }
    };

    window.addEventListener("resize", resizeCalendar);
    const container = document.querySelector(".agenda-calendar-container")?.parentElement;
    if (container) {
      const observer = new ResizeObserver(resizeCalendar);
      observer.observe(container);
      return () => {
        window.removeEventListener("resize", resizeCalendar);
        observer.disconnect();
      };
    } else {
      return () => window.removeEventListener("resize", resizeCalendar);
    }
  }, []);

  return (
    <div className="agenda-container">
      <h1
        className="text-center fw-bold mb-3"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        CALENDARIO
      </h1>

      {/* Filtros (solo para roles no profesor) */}
      {rol !== "Profesor" && (
        <div className="agenda-filtros mb-4 d-flex gap-3 justify-content-center">
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
      )}

      {/* 📅 Calendario */}
      <div className="agenda-calendar-container">
        <FullCalendar
          ref={calendarRef}
          plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
          initialView="timeGridWeek"
          locale="es"
          allDaySlot={false}
          editable={false}
          selectable={true}
          hiddenDays={[0, 6]} // lunes a viernes
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
    </div>
  );
}
