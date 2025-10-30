import React, { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faCalendarAlt,
  faClock,
  faDumbbell,
  faCheckCircle,
  faArrowRight,
} from "@fortawesome/free-solid-svg-icons";
import gymApi from "../../api/gymApi";
import "@/styles/SuscripcionesSocio.css";
import { asignarTurnos } from "./AsignarTurnoSocio";

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

  useEffect(() => {
    const fetchSuscripciones = async () => {
      try {
        // ✅ Obtener ID del socio desde localStorage (guardado al loguearse)
        const socioId = localStorage.getItem("socioId");
        if (!socioId) {
          console.warn("No se encontró socioId en localStorage");
          setLoading(false);
          return;
        }

        // ✅ Endpoint correcto del backend
        const res = await gymApi.get(`/suscripciones?socioId=${socioId}`);
        const data = res.data;

        // ✅ Acepta tanto { items: [...] } como una lista directa
        setSuscripciones(data.items || data || []);
      } catch (err) {
        console.error("Error al obtener suscripciones:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchSuscripciones();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen text-white">
        <p className="text-lg animate-pulse">Cargando tus suscripciones...</p>
      </div>
    );
  }

  if (!suscripciones.length) {
    return (
      <div className="flex flex-col items-center justify-center min-h-screen text-gray-300">
        <FontAwesomeIcon
          icon={faDumbbell}
          size="3x"
          className="text-[#ff6b00] mb-4"
        />
        <p className="text-xl">No tenés suscripciones activas</p>
      </div>
    );
  }

  return (
    <div className="suscripciones-container">
      <h1 className="suscripciones-title">Mis Suscripciones 🧡</h1>

      <div className="suscripciones-grid">
        {suscripciones.map((s) => {
          const inicio = new Date(s.inicio).toLocaleDateString();
          const fin = new Date(s.fin).toLocaleDateString();
          const turnos = s.turnosAsignados ?? 0;
          const cupo = s.cupoMaximo ?? 0;

          return (
            <div key={s.id} className="suscripcion-card">
              <FontAwesomeIcon icon={faDumbbell} className="icon-top" />
              <h2 className="suscripcion-plan">{s.plan?.nombre}</h2>

              <div className="suscripcion-info">
                <p>
                  <FontAwesomeIcon icon={faCalendarAlt} className="fa-icon" />
                  {inicio} – {fin}
                </p>
                <p>
                  <FontAwesomeIcon icon={faClock} className="fa-icon" />
                  {turnos}/{cupo} clases usadas
                </p>
              </div>

              <div
                className={`suscripcion-estado ${
                  s.estado ? "activa" : "inactiva"
                }`}
              >
                <FontAwesomeIcon icon={faCheckCircle} />
                {s.estado ? "Activa" : "Inactiva"}
              </div>

              {/* ✅ Pasar objeto completo, no solo el ID */}
              <button
                className="suscripcion-btn"
                onClick={() => asignarTurnos(s)}
              >
                Seleccionar Turnos <FontAwesomeIcon icon={faArrowRight} />
              </button>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default SuscripcionesSocio;
