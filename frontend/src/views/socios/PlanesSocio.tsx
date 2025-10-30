import React, { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faDumbbell,
  faCalendarDay,
  faDollarSign,
} from "@fortawesome/free-solid-svg-icons";
import Swal from "sweetalert2";
import gymApi from "../../api/gymApi";
import "@/styles/PlanesSocio.css";

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
        const res = await gymApi.get("/planes?activo=true");
        const data = res.data;

        // Solo planes activos
        const activos = data.items.filter((p: Plan) => p.activo);
        setPlanes(activos);
      } catch (error) {
        console.error("Error al cargar los planes:", error);
        Swal.fire({
          icon: "error",
          title: "Error",
          text: "No se pudieron cargar los planes disponibles.",
          confirmButtonColor: "#ff6b00",
        });
      } finally {
        setLoading(false);
      }
    };

    fetchPlanes();
  }, []);

  const abrirFormularioOrden = (plan: Plan) => {
    Swal.fire({
      title: `Generar orden de pago`,
      html: `
        <p style="font-size:1rem; margin-bottom:0.5rem;">Plan seleccionado: <b>${plan.nombre}</b></p>
        <p style="font-size:1rem;">Precio: <b>$${plan.precio}</b></p>
        <input type="file" id="comprobante" class="swal2-input custom-file-input" accept="image/*,application/pdf">
      `,
      confirmButtonText: "Enviar orden",
      showCancelButton: true,
      cancelButtonText: "Cancelar",
      focusConfirm: false,
      confirmButtonColor: "#ff6b00",
      didOpen: () => {
        // 🔧 corrige visual del input
        const fileInput = document.querySelector(".custom-file-input") as HTMLElement;
        if (fileInput) {
          fileInput.style.display = "block";
          fileInput.style.width = "100%";
          fileInput.style.marginTop = "1rem";
          fileInput.style.padding = "0.4rem";
          fileInput.style.background = "white";
          fileInput.style.color = "#333";
          fileInput.style.borderRadius = "6px";
        }
      },
      preConfirm: async () => {
        const fileInput = document.getElementById("comprobante") as HTMLInputElement;
        if (!fileInput.files?.length) {
          Swal.showValidationMessage("Debe adjuntar un comprobante");
          return;
        }

        const formData = new FormData();
        formData.append("PlanId", plan.id.toString());
        formData.append("FechaInicio", new Date().toISOString());
        formData.append("Notas", "Orden generada por socio desde PlanesSocio");
        formData.append("file", fileInput.files[0]);

        const res = await gymApi.post("/ordenes/socio", formData, {
          headers: { "Content-Type": "multipart/form-data" },
        });

        return res.data;
      },
    }).then((result) => {
      if (result.isConfirmed) {
        Swal.fire({
          icon: "success",
          title: "Orden enviada",
          text: "Tu orden fue registrada y está pendiente de aprobación.",
          confirmButtonColor: "#ff6b00",
        });
      }
    });
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-[#121212] text-white">
        <p className="text-lg animate-pulse">Cargando planes...</p>
      </div>
    );
  }

  return (
    <div className="planes-socio-container">
      <h1 className="planes-socio-title">Planes Disponibles 🧡</h1>

      <div className="planes-socio-grid">
        {planes.map((plan) => (
          <button
            key={plan.id}
            onClick={() => abrirFormularioOrden(plan)}
            className="plan-card"
          >
            <FontAwesomeIcon icon={faDumbbell} className="plan-icon" />
            <h3 className="plan-title">{plan.nombre}</h3>

            <div className="plan-info">
              <p>
                <FontAwesomeIcon icon={faCalendarDay} />
                {plan.diasPorSemana} días por semana
              </p>
              <p>
                <FontAwesomeIcon icon={faDollarSign} /> ${plan.precio.toFixed(2)}
              </p>
            </div>
          </button>
        ))}
      </div>
    </div>
  );

};

export default PlanesSocio;
