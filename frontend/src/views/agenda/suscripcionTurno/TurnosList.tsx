import { useEffect, useState } from "react";
import Pagination from "@/components/Pagination";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";
import Select from "react-select";

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
    sala?: { nombre: string; cupoTotal?: number; cupoDisponible?: number };
    personal?: { nombre: string };
  };
  checkinHecho?: boolean;
}

export default function TurnosList() {
  const [turnos, setTurnos] = useState<Turno[]>([]);
 
  const [socios, setSocios] = useState<any[]>([]);
  const [selectedSocio, setSelectedSocio] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  
 const filteredTurnos = selectedSocio
  ? turnos.filter((t) => t.suscripcion?.socio?.id === selectedSocio)
  : turnos;

  useEffect(() => {
    const total = turnos.length;
    setTotalPages(Math.ceil(total / itemsPerPage));

    if (currentPage > Math.ceil(total / itemsPerPage)) {
      setCurrentPage(1);
    }
  }, [turnos, itemsPerPage, currentPage]);


  // 🔹 Cargar turnos desde la API
  const fetchTurnos = async () => {
    try {
      const res = await gymApi.get("/SuscripcionTurno/con-checkin");
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

  const fetchSocios = async () => {
    try {
      const res = await gymApi.get("/socios");
      setSocios(res.data.items || res.data);
    } catch (err) {
      console.error("⚠️ No se pudieron cargar los socios:", err);
    }
  };

  useEffect(() => {
    fetchTurnos();
    fetchSocios();
  }, []);

  // 🔹 Registrar check-in
  const handleCheckin = async (socioId: number, turnoPlantillaId: number) => {
    try {
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

      fetchTurnos();
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

  if (loading)
    return (
      <div className="text-center mt-5">
        <div className="spinner-border text-warning" role="status"></div>
        <p className="mt-3">Cargando turnos asignados...</p>
      </div>
    );
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const visibleTurnos = filteredTurnos.slice(startIndex, endIndex);
  const total = filteredTurnos.length; // cuántos turnos hay (filtrados)

  return (
    <div className="mt-4 container">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        TURNOS ASIGNADOS
      </h1>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <div className="flex-grow-1 w-100">
          <Select
            options={socios.map((s) => ({
              value: s.id,
              label: `${s.nombre} (${s.email ?? "sin email"})`,
            }))}
            placeholder="Seleccionar socio..."
            isClearable
            onChange={(opt) => setSelectedSocio(opt ? opt.value : null)}
            styles={{
              control: (base) => ({
                ...base,
                borderColor: "#ff6b00",
                boxShadow: "none",
                "&:hover": { borderColor: "#ff6b00" },
                borderRadius: "8px",
                fontWeight: 500,
              }),
            }}
          />
        </div>

        <button
          className="btn btn-warning fw-semibold px-4 py-2"
          style={{
            backgroundColor: "#ff6600",
            border: "none",
            borderRadius: "8px",
            color: "white",
            transition: "all 0.2s ease",
          }}
          onMouseOver={(e) =>
            ((e.target as HTMLButtonElement).style.backgroundColor = "#e65100")
          }
          onMouseOut={(e) =>
            ((e.target as HTMLButtonElement).style.backgroundColor = "#ff6600")
          }
          onClick={() => setSelectedSocio(null)}
        >
          Limpiar
        </button>
      </div>
     
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
            visibleTurnos.map((t) => {
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

              // ✅ Cupos dinámicos
              const cupoTotal = turno?.sala?.cupoTotal ?? 0;
              const cupoDisponible = turno?.sala?.cupoDisponible ?? 0;

              const cupoColor =
                cupoDisponible === 0
                  ? "text-danger"
                  : cupoDisponible <= 3
                  ? "text-warning"
                  : "text-success";

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
                    {cupoDisponible}/{cupoTotal}
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
      <Pagination
        currentPage={currentPage}
        totalPages={Math.ceil(filteredTurnos.length / itemsPerPage)}
        totalItems={filteredTurnos.length}
        pageSize={itemsPerPage}
        onPageChange={(page) => setCurrentPage(page)}
      />
    </div>
  );
}
