import React, { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faClock,
  faCalendarAlt,
  faDumbbell,
  faUser,
  faDoorOpen,
  faTimesCircle,
  faSyncAlt,
} from "@fortawesome/free-solid-svg-icons";
import gymApi from "@/api/gymApi";
import Swal from "sweetalert2";
import "@/styles/TurnosSocio.css";
import { reagendarTurnoModal } from "./reagendarTurno";
import { cancelarTurnoModal } from "./cancelarTurno";

interface TurnoSocio {
  id: number;
  suscripcionId: number;
  turnoPlantilla?: {
    id: number;
    horaInicio: string;
    duracionMin: number;
    diaSemana?: { id: number; nombre: string };
    sala?: { nombre: string; cupo?: number; cupoDisponible?: number };
    personal?: { nombre: string };
  };
}

const TurnosSocio: React.FC = () => {
  const [turnos, setTurnos] = useState<TurnoSocio[]>([]);
  const [loading, setLoading] = useState(true);
  const socioId = localStorage.getItem("socioId");

  useEffect(() => {
    fetchTurnos();
  }, [socioId]);

  const fetchTurnos = async () => {
    try {
      const { data } = await gymApi.get(`/suscripcionturno/socio/${socioId}`);
      setTurnos(data.items || data);
    } catch (err) {
      console.error("❌ Error al obtener turnos:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleCancelar = async (turnoId: number) => {
    await cancelarTurnoModal(turnoId, fetchTurnos);
  };

  const handleReagendar = async (t: TurnoSocio) => {
    await reagendarTurnoModal(
      t.suscripcionId,
      t.id,
      t.turnoPlantilla?.id || 0,
      fetchTurnos
    );
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-[#121212] text-white">
        <p className="text-lg animate-pulse">Cargando tus turnos...</p>
      </div>
    );
  }

  if (!turnos.length) {
    return (
      <div className="flex flex-col items-center justify-center min-h-screen bg-[#121212] text-gray-300">
        <FontAwesomeIcon icon={faDumbbell} size="3x" className="text-[#ff6b00] mb-4" />
        <p className="text-xl">No tenés turnos asignados todavía</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-[#121212] text-white px-6 py-10">
      <h1 className="text-3xl font-bold text-center text-[#ff6b00] mb-10">
        Mis Turnos 🧡
      </h1>

      <div className="turnos-grid">
        {turnos.map((t) => {
          const turno = t.turnoPlantilla;
          if (!turno) return null;

          const horaInicio = turno.horaInicio?.slice(0, 5) || "--:--";
          const duracion = turno.duracionMin || 0;
          const horaFin = calcularHoraFin(turno.horaInicio, duracion);
          const dia = turno.diaSemana?.nombre || "Día sin asignar";
          const sala = turno.sala?.nombre || "Sala no definida";
          const profesor = turno.personal?.nombre || "Profesor no asignado";

          return (
            <div key={t.id} className="turno-card">
              <div className="turno-contenido">
                <div className="turno-header">
                  <FontAwesomeIcon icon={faCalendarAlt} />
                  <h2>{dia}</h2>
                </div>

                <div className="turno-body">
                  <p>
                    <FontAwesomeIcon icon={faClock} className="mr-2" />
                    {horaInicio} - {horaFin} hs
                  </p>
                  <p>
                    <FontAwesomeIcon icon={faDoorOpen} className="mr-2" />
                    {sala}
                  </p>
                  <p>
                    <FontAwesomeIcon icon={faUser} className="mr-2" />
                    {profesor}
                  </p>
                  <p className="italic text-sm">Duración: {duracion} min</p>
                </div>
              </div>

              <div className="turno-acciones">
                <button
                  onClick={() => handleCancelar(t.id)}
                  className="turno-btn cancelar flex items-center justify-center gap-2"
                >
                  <FontAwesomeIcon icon={faTimesCircle} />
                  Cancelar
                </button>

                <button
                  onClick={() => handleReagendar(t)}
                  className="turno-btn reagendar flex items-center justify-center gap-2"
                >
                  <FontAwesomeIcon icon={faSyncAlt} />
                  Reagendar
                </button>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
};

function calcularHoraFin(horaInicio: string, duracionMin: number): string {
  if (!horaInicio) return "--:--";
  const [h, m] = horaInicio.split(":").map(Number);
  const date = new Date();
  date.setHours(h, m + duracionMin);
  return date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
}

export default TurnosSocio;
