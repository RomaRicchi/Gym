import React, { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faClock,
  faCalendarAlt,
  faDumbbell,
  faCheckCircle,
  faTimesCircle,
} from "@fortawesome/free-solid-svg-icons";

interface Turno {
  id: number;
  fechaHora: string;
  sala: string;
  estado: boolean;
}

const TurnosSocio: React.FC = () => {
  const [turnos, setTurnos] = useState<Turno[]>([]);
  const [loading, setLoading] = useState(true);

  const socioId = localStorage.getItem("socioId");

  useEffect(() => {
    const fetchTurnos = async () => {
      try {
        const res = await fetch(`/api/turnos?socioId=${socioId}`);
        if (!res.ok) throw new Error("Error al obtener los turnos");
        const data = await res.json();
        setTurnos(data.items || []);
      } catch (err) {
        console.error("Error al obtener turnos:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchTurnos();
  }, [socioId]);

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
        <p className="text-xl">No tenés turnos reservados</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-[#121212] text-white px-6 py-10">
      <h1 className="text-3xl font-bold text-center text-[#ff6b00] mb-10">
        Mis Turnos 🧡
      </h1>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 max-w-6xl mx-auto">
        {turnos.map((t) => {
          const fecha = new Date(t.fechaHora).toLocaleDateString();
          const hora = new Date(t.fechaHora).toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit",
          });

          return (
            <div
              key={t.id}
              className="bg-[#1e1e1e] border border-[#ff6b00]/40 rounded-2xl shadow-lg p-6 hover:scale-105 transition-all duration-300"
            >
              <div className="flex items-center mb-4 space-x-3">
                <FontAwesomeIcon icon={faDumbbell} className="text-[#ff6b00]" />
                <h2 className="text-xl font-semibold">Turno #{t.id}</h2>
              </div>

              <div className="space-y-2 text-gray-300 text-sm">
                <p>
                  <FontAwesomeIcon icon={faCalendarAlt} className="text-[#ff6b00] mr-2" />
                  {fecha}
                </p>

                <p>
                  <FontAwesomeIcon icon={faClock} className="text-[#ff6b00] mr-2" />
                  {hora}
                </p>

                <p className="flex items-center">
                  <FontAwesomeIcon
                    icon={t.estado ? faCheckCircle : faTimesCircle}
                    className={`mr-2 ${t.estado ? "text-green-500" : "text-red-500"}`}
                  />
                  {t.estado ? "Confirmado" : "Cancelado"}
                </p>

                <p className="text-sm text-gray-400 italic">Sala: {t.sala}</p>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default TurnosSocio;
