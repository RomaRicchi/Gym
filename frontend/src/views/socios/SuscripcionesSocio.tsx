import React, { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faCalendarAlt,
  faClock,
  faDumbbell,
  faCheckCircle,
  faArrowRight,
} from "@fortawesome/free-solid-svg-icons";

interface Suscripcion {
  id: number;
  inicio: string;
  fin: string;
  estado: boolean;
  plan: { id: number; nombre: string };
  turnosAsignados?: number;
  cupoMaximo?: number;
}

const SuscripcionesSocio: React.FC = () => {
  const [suscripciones, setSuscripciones] = useState<Suscripcion[]>([]);
  const [loading, setLoading] = useState(true);

  const socioId = localStorage.getItem("socioId");

  useEffect(() => {
    const fetchSuscripciones = async () => {
      try {
        const res = await fetch(`/api/suscripciones?socioId=${socioId}`);
        const data = await res.json();
        setSuscripciones(data.items || []);
      } catch (err) {
        console.error("Error al obtener suscripciones:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchSuscripciones();
  }, [socioId]);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-[#121212] text-white">
        <p className="text-lg animate-pulse">Cargando tus suscripciones...</p>
      </div>
    );
  }

  if (!suscripciones.length) {
    return (
      <div className="flex flex-col items-center justify-center min-h-screen bg-[#121212] text-gray-300">
        <FontAwesomeIcon icon={faDumbbell} size="3x" className="text-[#ff6b00] mb-4" />
        <p className="text-xl">No tenés suscripciones activas</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-[#121212] text-white px-6 py-10">
      <h1 className="text-3xl font-bold text-center text-[#ff6b00] mb-10">
        Mis Suscripciones 🧡
      </h1>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 max-w-6xl mx-auto">
        {suscripciones.map((s) => {
          const inicio = new Date(s.inicio).toLocaleDateString();
          const fin = new Date(s.fin).toLocaleDateString();
          const turnos = s.turnosAsignados ?? 0;
          const cupo = s.cupoMaximo ?? 0;

          return (
            <div
              key={s.id}
              className="bg-[#1e1e1e] border border-[#ff6b00]/40 rounded-2xl shadow-lg p-6 hover:scale-105 transition-all duration-300"
            >
              <div className="flex items-center mb-4 space-x-3">
                <FontAwesomeIcon icon={faDumbbell} className="text-[#ff6b00]" />
                <h2 className="text-xl font-semibold">{s.plan?.nombre}</h2>
              </div>

              <div className="space-y-2 text-gray-300 text-sm">
                <p>
                  <FontAwesomeIcon icon={faCalendarAlt} className="text-[#ff6b00] mr-2" />
                  <span className="text-gray-200 font-medium">
                    {inicio} – {fin}
                  </span>
                </p>

                <p>
                  <FontAwesomeIcon icon={faClock} className="text-[#ff6b00] mr-2" />
                  {turnos}/{cupo} clases usadas
                </p>

                <p className="flex items-center">
                  <FontAwesomeIcon
                    icon={faCheckCircle}
                    className="text-green-500 mr-2"
                  />
                  Activa
                </p>
              </div>

              <button
                className="mt-6 w-full py-2 rounded-xl font-semibold bg-[#ff6b00] hover:bg-[#ff8533] transition-all text-white flex items-center justify-center space-x-2"
                onClick={() =>
                  (window.location.href = `/turnos?suscripcionId=${s.id}`)
                }
              >
                <span>Seleccionar Turnos</span>
                <FontAwesomeIcon icon={faArrowRight} />
              </button>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default SuscripcionesSocio;
