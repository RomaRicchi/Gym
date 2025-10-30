import React, { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faDumbbell,
  faCalendarDay,
  faDollarSign,
  faCheckCircle,
  faTimesCircle,
} from "@fortawesome/free-solid-svg-icons";

interface Plan {
  id: number;
  nombre: string;
  diasPorSemana: number;
  precio: number;
  activo: boolean;
}

const PlanesSocio: React.FC = () => {
  const [planes, setPlanes] = useState<Plan[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchPlanes = async () => {
      try {
        const res = await fetch("/api/planes");
        const data = await res.json();
        setPlanes(data.items);
      } catch (error) {
        console.error("Error al cargar los planes:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchPlanes();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-[#121212] text-white">
        <p className="text-lg animate-pulse">Cargando planes...</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-[#121212] text-white px-6 py-10">
      <h1 className="text-3xl font-bold text-center text-[#ff6b00] mb-8">
        Planes Disponibles 🧡
      </h1>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 max-w-6xl mx-auto">
        {planes.map((plan) => (
          <div
            key={plan.id}
            className="bg-[#1e1e1e] border border-[#ff6b00]/40 rounded-2xl shadow-lg p-6 hover:scale-105 transition-all duration-300"
          >
            <div className="flex items-center space-x-3 mb-3">
              <FontAwesomeIcon icon={faDumbbell} className="text-[#ff6b00]" />
              <h2 className="text-xl font-semibold">{plan.nombre}</h2>
            </div>

            <div className="space-y-2 text-sm text-gray-300">
              <p>
                <FontAwesomeIcon icon={faCalendarDay} className="text-[#ff6b00] mr-2" />
                {plan.diasPorSemana} días por semana
              </p>
              <p>
                <FontAwesomeIcon icon={faDollarSign} className="text-[#ff6b00] mr-2" />
                ${plan.precio.toFixed(2)}
              </p>
              <p className="flex items-center">
                <FontAwesomeIcon
                  icon={plan.activo ? faCheckCircle : faTimesCircle}
                  className={`mr-2 ${plan.activo ? "text-green-500" : "text-red-500"}`}
                />
                {plan.activo ? "Activo" : "Inactivo"}
              </p>
            </div>

            <button
              disabled={!plan.activo}
              className={`mt-6 w-full py-2 rounded-xl font-semibold transition-all ${
                plan.activo
                  ? "bg-[#ff6b00] hover:bg-[#ff8533] text-white"
                  : "bg-gray-700 text-gray-400 cursor-not-allowed"
              }`}
            >
              {plan.activo ? "Suscribirme" : "No disponible"}
            </button>
          </div>
        ))}
      </div>
    </div>
  );
};

export default PlanesSocio;
