import { useEffect, useState } from "react";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Turno {
  id: number;
  turnoPlantillaId?: number;
  suscripcion?: {
    socio?: {
      id: number;
      nombre: string;
    };
  };
  turnoPlantilla?: {
    id: number;
    horaInicio: string;
    duracionMin: number;
    diaSemana?: { nombre: string };
    sala?: { nombre: string };
    personal?: { nombre: string };
    cupo?: number;
  };
  checkinHecho?: boolean; // 👈 nuevo campo del backend
}

export default function TurnosList() {
  const [turnos, setTurnos] = useState<Turno[]>([]);
  const [loading, setLoading] = useState(true);

  // 🔹 Cargar turnos desde la API
  const fetchTurnos = async () => {
    try {
      const res = await gymApi.get("/SuscripcionTurno/con-checkin"); // 👈 usamos el nuevo endpoint
      const data = res.data.items || res.data;
      console.log("📦 Datos de turnos:", data);
      setTurnos(data);
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudieron cargar los turnos asignados", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTurnos();
  }, []);

  // 🔹 Registrar check-in
  const handleCheckin = async (socioId: number, turnoPlantillaId: number) => {
    try {
      // ✅ Enviar los nombres exactos que el backend espera
      const payload = { socioId, turnoPlantillaId };
      console.log("📤 Enviando payload:", payload);

      await gymApi.post("/Checkin", payload);

      Swal.fire({
        title: "✅ Check-in registrado",
        text: "Asistencia marcada correctamente.",
        icon: "success",
        timer: 1300,
        showConfirmButton: false,
      });

      fetchTurnos(); // 👈 refresca la lista para actualizar el ícono
    } catch (error: any) {
      console.error(error);
      const msg =
        error.response?.data?.message || "No se pudo registrar el check-in";
      Swal.fire("Error", msg, "error");
    }
  };

  // 🔹 Eliminar turno
  const handleDelete = async (id: number) => {
    const result = await Swal.fire({
      title: "¿Eliminar turno?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar",
      confirmButtonColor: "#d33",
    });

    if (!result.isConfirmed) return;

    try {
      await gymApi.delete(`/SuscripcionTurno/${id}`);
      Swal.fire("Eliminado", "Turno eliminado correctamente", "success");
      fetchTurnos();
    } catch (err) {
      console.error(err);
      Swal.fire("Error", "No se pudo eliminar el turno", "error");
    }
  };

  // 🔹 Render
  if (loading)
    return (
      <div className="text-center mt-5">
        <div className="spinner-border text-warning" role="status"></div>
        <p className="mt-3">Cargando turnos asignados...</p>
      </div>
    );

  return (
    <div className="mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        TURNOS ASIGNADOS
      </h1>

      <table className="table table-striped table-hover align-middle text-center shadow-sm">
        <thead className="table-dark">
          <tr>
            <th>Socio</th>
            <th>Día</th>
            <th>Hora</th>
            <th>Sala</th>
            <th>Profesor</th>
            <th>Duración</th>
            <th>Cupo</th>
            <th>Check-in</th>
            <th>Acciones</th>
          </tr>
        </thead>

        <tbody>
          {turnos.length > 0 ? (
            turnos.map((t) => {
              const socio = t.suscripcion?.socio?.nombre || "—";
              const socioId = t.suscripcion?.socio?.id;
              const turno = t.turnoPlantilla;
              const turnoId = t.turnoPlantillaId ?? turno?.id;
              const dia = turno?.diaSemana?.nombre || "—";
              const hora = turno?.horaInicio
                ? turno.horaInicio.slice(0, 5)
                : "—";
              const sala = turno?.sala?.nombre || "—";
              const profesor = turno?.personal?.nombre || "—";
              const duracion = turno?.duracionMin || 0;
              const cupo = turno?.cupo ?? 0;
              const cupoColor =
                cupo > 5
                  ? "text-success"
                  : cupo > 0
                  ? "text-warning"
                  : "text-danger";
              const checkinHecho = t.checkinHecho ?? false;

              return (
                <tr key={t.id}>
                  <td>{socio}</td>
                  <td>{dia}</td>
                  <td>{hora}</td>
                  <td>{sala}</td>
                  <td>{profesor}</td>
                  <td>{duracion} min</td>
                  <td className={`${cupoColor} fw-bold`}>
                    {cupo > 0 ? `${cupo} disp.` : "Sin cupo"}
                  </td>
                  <td>
                    <button
                      className={`btn btn-sm fw-bold ${
                        checkinHecho ? "btn-success" : "btn-outline-success"
                      }`}
                      title={
                        checkinHecho
                          ? "Asistencia registrada"
                          : "Registrar asistencia"
                      }
                      onClick={() =>
                        !checkinHecho &&
                        handleCheckin(socioId || 0, turnoId || 0)
                      }
                      disabled={checkinHecho}
                    >
                      {checkinHecho ? "✅" : "☑️"}
                    </button>
                  </td>
                  <td>
                    <button
                      className="btn btn-sm btn-danger"
                      title="Eliminar turno"
                      onClick={() => handleDelete(t.id)}
                    >
                      🗑️
                    </button>
                  </td>
                </tr>
              );
            })
          ) : (
            <tr>
              <td colSpan={9} className="text-muted">
                No hay turnos registrados.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}
