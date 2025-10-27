import { useEffect, useState } from "react";
import { mostrarFormEditarSuscripcion } from "@/views/suscripciones/SuscripcionEdit";
import { asignarTurnos } from "@/views/agenda/suscripcionTurno/asignarTurnos";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Suscripcion {
  id: number;
  socio: string;
  plan: string;
  plan_id?: number;
  inicio: string;
  fin: string;
  estado: boolean;
  creado_en: string;
  orden_pago_id?: number;
  turnosAsignados?: number;
  cupoMaximo?: number;
}

export default function SuscripcionesList() {
  const [suscripciones, setSuscripciones] = useState<Suscripcion[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchSuscripciones = async () => {
    try {
      const res = await gymApi.get("/suscripciones");
      const data = res.data.items || res.data;

      const parsed: Suscripcion[] = await Promise.all(
        data.map(async (s: any) => {
          try {
            // 🧩 Buscar turnos asignados
            const { data: turnos } = await gymApi.get(
              `/suscripcionturno/suscripcion/${s.id}`
            );

            // Buscar el plan correctamente por ID
            let diasPorSemana = 0;
            if (s.planId) {
              const { data: plan } = await gymApi.get(`/planes/${s.planId}`);
              diasPorSemana =
                Number(plan.diasPorSemana ?? plan.dias_por_semana ?? plan.DiasPorSemana ?? 0);
            }

            return {
              id: s.id,
              socio: s.socio ?? "-",
              plan: s.plan ?? "-",
              plan_id: s.planId, 
              inicio: s.inicio,
              fin: s.fin,
              estado: Boolean(s.estado),
              creado_en: s.creadoEn,
              orden_pago_id: s.ordenPagoId,
              turnosAsignados: turnos.length,
              cupoMaximo: diasPorSemana,
            };
          } catch (error) {
            console.error(`Error cargando info de suscripción ${s.id}`, error);
            return {
              id: s.id,
              socio: s.socio ?? "-",
              plan: s.plan ?? "-",
              inicio: s.inicio,
              fin: s.fin,
              estado: Boolean(s.estado),
              creado_en: s.creadoEn,
              turnosAsignados: 0,
              cupoMaximo: 0,
            };
          }
        })
      );

      setSuscripciones(parsed);
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudieron cargar las suscripciones", "error");
    } finally {
      setLoading(false);
    }
  };


  useEffect(() => {
    fetchSuscripciones();
  }, []);

  //  Eliminar suscripción
  const handleDelete = async (id: number) => {
    const result = await Swal.fire({
      title: "¿Eliminar suscripción?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (!result.isConfirmed) return;

    try {
      await gymApi.delete(`/suscripciones/${id}`);
      Swal.fire("Eliminada", "Suscripción eliminada correctamente", "success");
      fetchSuscripciones();
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudo eliminar la suscripción", "error");
    }
  };

  if (loading)
    return (
      <div className="text-center mt-5">
        <div className="spinner-border text-warning" role="status"></div>
        <p className="mt-3">Cargando suscripciones...</p>
      </div>
    );

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        SUSCRIPCIONES
      </h1>

      <table className="table table-striped table-hover align-middle text-center shadow-sm">
        <thead className="table-dark">
          <tr>
            <th>Socio</th>
            <th>Plan</th>
            <th>Inicio</th>
            <th>Fin</th>
            <th>Estado</th>
            <th>Turnos</th>
            <th>Acciones</th>
          </tr>
        </thead>

        <tbody>
          {suscripciones.length > 0 ? (
            suscripciones.map((s) => {
              const completado =
                (s.cupoMaximo ?? 0) > 0 &&
                (s.turnosAsignados ?? 0) >= (s.cupoMaximo ?? 0);

              return (
                <tr key={s.id}>
                  <td>{s.socio}</td>
                  <td>{s.plan}</td>
                  <td>{new Date(s.inicio).toLocaleDateString()}</td>
                  <td>{new Date(s.fin).toLocaleDateString()}</td>
                  <td>
                    {s.estado ? (
                      <span className="text-success fw-bold">✅</span>
                    ) : (
                      <span className="text-danger fw-bold">❌</span>
                    )}
                  </td>
                  <td>
                    {(s.turnosAsignados ?? 0)}/{s.cupoMaximo ?? "?"}
                  </td>
                  <td>
                    <div className="d-flex justify-content-center gap-2">
                      {/* ✏️ Editar */}
                      <button
                        className="btn btn-sm btn-warning"
                        title="Editar suscripción"
                        onClick={async () => {
                          const ok = await mostrarFormEditarSuscripcion(s.id);
                          if (ok) fetchSuscripciones();
                        }}
                      >
                        ✏️
                      </button>

                      {/* 🗑️ Eliminar */}
                      <button
                        className="btn btn-sm btn-danger"
                        title="Eliminar suscripción"
                        onClick={() => handleDelete(s.id)}
                      >
                        🗑️
                      </button>

                      {/* 🗓️ Asignar Turnos */}
                      <button
                        className={`btn btn-sm ${completado ? "btn-secondary" : "btn-primary"}`}
                        title={
                          completado
                            ? "Todos los turnos ya fueron asignados"
                            : "Asignar turnos al socio"
                        }
                        onClick={async () => {
                          if (!completado) {
                            await asignarTurnos(s, fetchSuscripciones); 
                          }
                        }}
                        disabled={completado}
                      >
                        🗓️
                      </button>
                    </div>
                  </td>
                </tr>
              );
            })
          ) : (
            <tr>
              <td colSpan={7} className="text-muted">
                No hay suscripciones registradas.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}
