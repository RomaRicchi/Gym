import React, { useEffect, useState, useRef } from "react";
import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin from "@fullcalendar/interaction";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/AgendaCalendar.css";

interface TurnoSocio {
  id: number;
  turnoPlantilla: {
    id: number;
    horaInicio: string;
    duracionMin: number;
    sala: { nombre: string };
    personal: { nombre: string };
    diaSemana: { nombre: string; id: number };
  };
}

export default function TurnosSocioCalendar() {
  const [eventos, setEventos] = useState<any[]>([]);
  const calendarRef = useRef<any>(null);

  // Detectar socio logueado de forma robusta
  const socioId =
    localStorage.getItem("socioId") ||
    JSON.parse(localStorage.getItem("usuario") || "{}")?.socio_id ||
    null;

  // Cargar los turnos del socio
  const cargarTurnosSocio = async () => {
    try {
        if (!socioId) {
        await Swal.fire("Sin suscripción", "No se detectó ningún socio logueado.", "info");
        return;
        }

        // 📦 Obtener suscripción activa del socio
        const { data: susRes } = await gymApi.get(`/suscripciones?socioId=${socioId}`);
        const suscripciones = susRes.items || susRes || [];
        const suscripcionActiva = suscripciones.find((s: any) => s.estado);

        if (!suscripcionActiva) {
        await Swal.fire("Sin suscripción", "No tenés una suscripción activa.", "info");
        return;
        }

        // Rango de validez
        const inicioSus = new Date(suscripcionActiva.inicio);
        const finSus = new Date(suscripcionActiva.fin);

        //Obtener turnos asociados a la suscripción activa
        const { data: turnosRes } = await gymApi.get(
        `/suscripcionturno/suscripcion/${suscripcionActiva.id}`
        );
        const turnos: TurnoSocio[] = turnosRes || [];

        if (!turnos.length) {
        await Swal.fire("Sin turnos", "No tenés turnos asignados actualmente.", "info");
        return;
        }

        // Mapear turnos a eventos de calendario (con límites de recurrencia)
        const eventosMapeados = turnos.map((t) => {
        const tp = t.turnoPlantilla;
        const [hora, minuto] = tp.horaInicio.split(":").map(Number);
        const duracionHoras = Math.floor(tp.duracionMin / 60);
        const duracionMinutos = tp.duracionMin % 60;

        const horaFin = hora + duracionHoras + Math.floor((minuto + duracionMinutos) / 60);
        const minutoFin = (minuto + duracionMinutos) % 60;

        return {
            id: t.id.toString(),
            title: `${tp.sala?.nombre || "Sala"} — ${tp.personal?.nombre || "Profesor"}`,
            daysOfWeek: [tp.diaSemana?.id || 1],
            startTime: `${hora.toString().padStart(2, "0")}:${minuto
            .toString()
            .padStart(2, "0")}`,
            endTime: `${horaFin.toString().padStart(2, "0")}:${minutoFin
            .toString()
            .padStart(2, "0")}`,
            startRecur: inicioSus.toISOString().split("T")[0],
            endRecur: finSus.toISOString().split("T")[0],
            backgroundColor: "#ff6b00",
            borderColor: "#ff6b00",
            textColor: "#fff",
            extendedProps: {
            dia: tp.diaSemana?.nombre,
            sala: tp.sala?.nombre,
            profesor: tp.personal?.nombre,
            duracion: tp.duracionMin,
            inicioSus: inicioSus.toLocaleDateString(),
            finSus: finSus.toLocaleDateString(),
            },
        };
    });

        setEventos(eventosMapeados);
    } catch (err) {
        console.error("❌ Error al cargar turnos del socio:", err);
        Swal.fire("Error", "No se pudieron cargar los turnos del socio.", "error");
    }
    };


  useEffect(() => {
    cargarTurnosSocio();
  }, []);

  // 🪄 Redimensionar dinámicamente
  useEffect(() => {
    const resizeCalendar = () => {
      const calendarApi = calendarRef.current?.getApi();
      if (calendarApi) {
        setTimeout(() => calendarApi.updateSize(), 300);
      }
    };
    window.addEventListener("resize", resizeCalendar);
    return () => window.removeEventListener("resize", resizeCalendar);
  }, []);

  // 🧩 Mostrar detalle del turno
  const handleEventClick = async (info: any) => {
    const { sala, profesor, dia, duracion } = info.event.extendedProps;
    const horaInicio = info.event.startStr.slice(11, 16);
    const horaFin = info.event.endStr.slice(11, 16);

    await Swal.fire({
      title: "🧡 Detalle del Turno",
      html: `
        <p><strong>Día:</strong> ${dia || "—"}</p>
        <p><strong>Sala:</strong> ${sala || "—"}</p>
        <p><strong>Profesor:</strong> ${profesor || "—"}</p>
        <p><strong>Horario:</strong> ${horaInicio} - ${horaFin}</p>
        <p><strong>Duración:</strong> ${duracion} minutos</p>
      `,
      confirmButtonText: "Cerrar",
      confirmButtonColor: "#ff6600",
    });
  };

  return (
    <div className="agenda-container">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        MIS TURNOS 🧡
      </h1>

      <div className="agenda-calendar-container">
        <FullCalendar
          ref={calendarRef}
          plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
          initialView="timeGridWeek"
          locale="es"
          allDaySlot={false}
          editable={false}
          selectable={false}
          hiddenDays={[0, 6]} // lunes a viernes
          events={eventos}
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
