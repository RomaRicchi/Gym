// @ts-nocheck
import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/rutina-cards.css";

export default function RutinasSocio() {
  const [rutina, setRutina] = useState<any>(null);
  const [ejercicios, setEjercicios] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  // 🔧 Corrige la ruta de imagen (soporta rutas relativas, nulas o absolutas)
  const getImagen = (url: string | null | undefined) => {
    if (!url || url === "null" || url.trim() === "") return "/images/empty.png";
    // si es ruta relativa (ej: 'uploads/...') le antepone la base API
    if (!url.startsWith("http")) {
      const base = import.meta.env.VITE_API_BASE_URL?.replace(/\/$/, "") || "http://localhost:5144/api";
      return `${base}/${url.replace(/^\/+/, "")}`;
    }
    return url;
  };

  const cargarRutina = async () => {
    try {
      const { data } = await gymApi.get("/suscripcionturno/rutina/socio");
      setRutina(data || null);
      setEjercicios(data?.ejercicios || []);
    } catch (error) {
      console.error(error);
      Swal.fire("Error", "No se pudo cargar la rutina de hoy.", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarRutina();
  }, []);

  const verImagen = (imgUrl: string, nombre: string, tips: string) => {
    Swal.fire({
      title: nombre,
      text: tips || "Sin descripción",
      imageUrl: getImagen(imgUrl),
      imageWidth: 420,
      imageHeight: 320,
      background: "#1e1e1e",
      color: "#fff",
      confirmButtonColor: "#ff6600",
    });
  };

  if (loading) return <p style={{ color: "#fff" }}>Cargando rutina...</p>;
  if (!rutina)
    return <p style={{ color: "#ccc" }}>No hay rutina asignada para hoy.</p>;

  return (
    <div className="rutina-container">
      <h2 className="titulo-principal">Rutina de hoy 🏋️</h2>

      <div className="rutina-card-grande">
        {/* 🧍 LADO IZQUIERDO */}
        <div className="rutina-izquierda">
          <img
            src={getImagen(rutina.imagenUrl)}
            alt={rutina.nombre}
            className="rutina-imagen-grande"
            onError={(e) => (e.currentTarget.src = "/images/empty.png")}
            onClick={() =>
              verImagen(rutina.imagenUrl, rutina.nombre, rutina.objetivo)
            }
          />
          <div className="rutina-detalle">
            <h3>{rutina.nombre}</h3>
            <p>{rutina.objetivo}</p>
            <p>
              <strong>Día:</strong> {rutina.dia}
            </p>
          </div>
        </div>

        {/* 💪 LADO DERECHO */}
        <div className="rutina-derecha">
          <h4>Ejercicios</h4>
          {ejercicios.length === 0 ? (
            <p>Esta rutina todavía no tiene ejercicios cargados.</p>
          ) : (
            <div className="lista-ejercicios">
              {ejercicios.map((e) => (
                <div key={e.id} className="ejercicio-fila">
                  <img
                    src={getImagen(e.mediaUrl)}
                    alt={e.nombre}
                    className="img-ejercicio-mini"
                    onClick={() => verImagen(e.mediaUrl, e.nombre, e.tips)}
                    onError={(ev) =>
                      (ev.currentTarget.src = "/images/empty.png")
                    }
                  />
                  <div className="info-ejercicio">
                    <h5>{e.nombre}</h5>
                    <p>
                      {e.series}x{e.repeticiones} | Descanso: {e.descansoSeg}s
                    </p>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
