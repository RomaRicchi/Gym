import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faDumbbell } from "@fortawesome/free-solid-svg-icons";

const RutinasSocio: React.FC = () => {
  return (
    <div className="min-h-screen bg-[#121212] text-white flex flex-col items-center justify-center">
      <FontAwesomeIcon icon={faDumbbell} size="3x" className="text-[#ff6b00] mb-4" />
      <h1 className="text-3xl font-bold text-[#ff6b00] mb-2">Rutinas</h1>
      <p className="text-gray-300 text-center max-w-md">
        Próximamente podrás ver tus rutinas personalizadas aquí 💪.
      </p>
    </div>
  );
};

export default RutinasSocio;
