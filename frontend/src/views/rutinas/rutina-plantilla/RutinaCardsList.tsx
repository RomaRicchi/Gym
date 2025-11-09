// @ts-nocheck
import { useEffect, useState, useRef } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import "@/styles/swal-card.css";
import "@/styles/rutina-cards.css";
import { RutinaMostrarEjerciciosSwal } from "@/views/rutinas/rutina-ejercicios/RutinaMostrarEjerciciosSwal";

export default function RutinaCardsList() {
  const [grupos, setGrupos] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const trackRefs = useRef([]); // ✅ refs para cada grupo

  // === Cargar rutinas (sin paginación) ===
  const cargarRutinas = async () => {
    try {
      const { data } = await gymApi.get("/rutinasplantilla/all");
      if (!data) return;

      const agrupado = data.reduce((acc, rutina) => {
        const grupo = rutina.grupoMuscularNombre || "Sin grupo";
        if (!acc[grupo]) acc[grupo] = [];
        acc[grupo].push(rutina);
        return acc;
      }, {});

      const arr = Object.entries(agrupado).map(([nombre, rutinas]) => ({
        nombre,
        rutinas,
      }));

      setGrupos(arr);
      // ⚙️ Inicializa refs sin usar hooks dentro del map
      trackRefs.current = arr.map(() => ({ current: null }));
    } catch (error) {
      console.error(error);
      Swal.fire("Error", "No se pudieron cargar las rutinas", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarRutinas();
  }, []);

  // === Render ===
  if (loading)
    return <p className="text-center text-light mt-5">Cargando rutinas...</p>;

  return (
    <div className="container mt-4">
      <h1 className="titulo-modulo">RUTINAS DISPONIBLES</h1>

      {grupos.map((grupo, idx) => {
        const scrollLeft = () => {
          const el = trackRefs.current[idx]?.current;
          if (el) el.scrollBy({ left: -260, behavior: "smooth" });
        };

        const scrollRight = () => {
          const el = trackRefs.current[idx]?.current;
          if (el) el.scrollBy({ left: 260, behavior: "smooth" });
        };

        return (
          <div key={idx} className="mb-5">
            <h3
              className="fw-bold text-warning mb-3 text-center"
              style={{
                textTransform: "uppercase",
                borderBottom: "2px solid #ff6600",
                display: "inline-block",
                paddingBottom: "4px",
              }}
            >
              {grupo.nombre}
            </h3>

            <div className="rutina-carrusel-container">
              <button className="btn-carrusel left" onClick={scrollLeft}>
                ‹
              </button>

              <div
                className="rutina-carrusel"
                ref={(el) => (trackRefs.current[idx].current = el)}
              >
                {grupo.rutinas.map((r) => (
                  <div
                    key={r.id}
                    className="rutina-card"
                    onClick={() => RutinaMostrarEjerciciosSwal(r.id, r.nombre)}
                  >
                    <img
                      src={
                        r.imagenUrl
                          ? `${import.meta.env.VITE_API_BASE_URL.replace(
                              /\/api$/,
                              ""
                            )}/${r.imagenUrl}`
                          : "/placeholder.png"
                      }
                      alt={r.nombre}
                      onError={(ev) =>
                        (ev.currentTarget.src = "/placeholder.png")
                      }
                      className="rutina-img"
                    />
                    <div className="rutina-info">
                      <h5 className="rutina-nombre">{r.nombre}</h5>
                      <p className="rutina-objetivo">
                        {r.objetivo || "Sin objetivo"}
                      </p>
                    </div>
                  </div>
                ))}
              </div>

              <button className="btn-carrusel right" onClick={scrollRight}>
                ›
              </button>
            </div>
          </div>
        );
      })}
    </div>
  );
}
