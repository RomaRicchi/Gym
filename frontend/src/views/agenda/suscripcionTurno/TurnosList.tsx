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
  const [profesores, setProfesores] = useState<any[]>([]);
  const [dias, setDias] = useState<any[]>([]);

  const [selectedSocio, setSelectedSocio] = useState<number | null>(null);
  const [selectedProfesor, setSelectedProfesor] = useState<string | null>(null);
  const [selectedDia, setSelectedDia] = useState<string | null>(null);

  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(10);
  const [totalPages, setTotalPages] = useState(1);

  // 🔹 Filtros combinados
  const filteredTurnos = turnos.filter((t) => {
    const socioOk = selectedSocio ? t.suscripcion?.socio?.id === selectedSocio : true;
    const profOk = selectedProfesor
      ? t.turnoPlantilla?.personal?.nombre === selectedProfesor
      : true;
    const diaOk = selectedDia ? t.turnoPlantilla?.diaSemana?.nombre === selectedDia : true;
    return socioOk && profOk && diaOk;
  });

  // 🔹 Recalcular paginación cuando cambian filtros
  useEffect(() => {
    const total = filteredTurnos.length;
    setTotalPages(Math.ceil(total / itemsPerPage));
    if (currentPage > Math.ceil(total / itemsPerPage)) {
      setCurrentPage(1);
    }
  }, [filteredTurnos, itemsPerPage, currentPage]);

  // 🔹 Cargar datos
  const fetchTurnos = async () => {
    try {
      const res = await gymApi.get("/SuscripcionTurno/con-checkin");
      const data = res.data.items || res.data;
      setTurnos(data);
    } catch (err) {
      Swal.fire("Error", "No se pudieron cargar los turnos asignados", "error");
    } finally {
      setLoading(false);
    }
  };

  const fetchSocios = async () => {
    try {
      const res = await gymApi.get("/socios");
      setSocios(res.data.items || res.data);
    } catch {}
  };

  const fetchProfesores = async () => {
    try {
      const res = await gymApi.get("/personal");
      setProfesores(res.data.items || res.data);
    } catch {}
  };

  const fetchDias = async () => {
    try {
      const res = await gymApi.get("/diasemana");
      setDias(
        res.data.items || res.data || [
          { id: 1, nombre: "Lunes" },
          { id: 2, nombre: "Martes" },
          { id: 3, nombre: "Miércoles" },
          { id: 4, nombre: "Jueves" },
          { id: 5, nombre: "Viernes" },
          { id: 6, nombre: "Sábado" },
          { id: 7, nombre: "Domingo" },
        ]
      );
    } catch {}
  };

  useEffect(() => {
    fetchTurnos();
    fetchSocios();
    fetchProfesores();
    fetchDias();
  }, []);

  // 🔹 Check-in
  const handleCheckin = async (socioId: number, turnoPlantillaId: number) => {
    try {
      await gymApi.post("/Checkin", { socioId, turnoPlantillaId });
      Swal.fire({
        title: "✅ Check-in registrado",
        text: "Asistencia marcada correctamente.",
        icon: "success",
        timer: 1300,
        showConfirmButton: false,
      });
      fetchTurnos();
    } catch (error: any) {
      Swal.fire("Error", error.response?.data?.message || "No se pudo registrar", "error");
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
    } catch {
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

  // 🔹 Paginación
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const visibleTurnos = filteredTurnos.slice(startIndex, endIndex);

  return (
    <div className="container mt-4">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "1px" }}
      >
        TURNOS ASIGNADOS
      </h1>

      {/* 🔸 Filtros */}
      <div className="d-flex flex-wrap gap-3 mb-3">
        <div style={{ flex: 1 }}>
          <Select
            options={socios.map((s) => ({
              value: s.id,
              label: `${s.nombre} (${s.email ?? "sin email"})`,
            }))}
            placeholder="Filtrar por socio..."
            isClearable
            onChange={(opt) => setSelectedSocio(opt ? opt.value : null)}
            styles={{
              control: (base) => ({
                ...base,
                borderColor: "#ff6b00",
                boxShadow: "none",
                "&:hover": { borderColor: "#ff6b00" },
                borderRadius: "8px",
              }),
            }}
          />
        </div>

        <div style={{ flex: 1 }}>
          <Select
            options={dias.map((d) => ({ value: d.nombre, label: d.nombre }))}
            placeholder="Filtrar por día..."
            isClearable
            onChange={(opt) => setSelectedDia(opt ? opt.value : null)}
            styles={{
              control: (base) => ({
                ...base,
                borderColor: "#ff6b00",
                boxShadow: "none",
                "&:hover": { borderColor: "#ff6b00" },
                borderRadius: "8px",
              }),
            }}
          />
        </div>

        <div style={{ flex: 1 }}>
          <Select
            options={profesores.map((p) => ({
              value: p.nombre,
              label: p.nombre,
            }))}
            placeholder="Filtrar por profesor..."
            isClearable
            onChange={(opt) => setSelectedProfesor(opt ? opt.value : null)}
            styles={{
              control: (base) => ({
                ...base,
                borderColor: "#ff6b00",
                boxShadow: "none",
                "&:hover": { borderColor: "#ff6b00" },
                borderRadius: "8px",
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
            height: "fit-content",
          }}
          onClick={() => {
            setSelectedSocio(null);
            setSelectedProfesor(null);
            setSelectedDia(null);
          }}
        >
          Limpiar filtros
        </button>
      </div>

      {/* 🔸 Tabla */}
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
            <th>Rutina</th>
            <th>Check-in</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {visibleTurnos.length > 0 ? (
            visibleTurnos.map((t) => {
              const socio = t.suscripcion?.socio?.nombre || "—";
              const socioId = t.suscripcion?.socio?.id;
              const turno = t.turnoPlantilla;
              const turnoId = t.turnoPlantillaId ?? turno?.id;
              const dia = turno?.diaSemana?.nombre || "—";
              const hora = turno?.horaInicio?.slice(0, 5) || "—";
              const sala = turno?.sala?.nombre || "—";
              const profesor = turno?.personal?.nombre || "—";
              const duracion = turno?.duracionMin || 0;

              const cupoTotal = turno?.sala?.cupoTotal ?? 0;
              const cupoDisponible = turno?.sala?.cupoDisponible ?? 0;
              const color =
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
                  <td className={`${color} fw-bold`}>
                    {cupoDisponible}/{cupoTotal}
                  </td>
                  <td>{(t as any).rutina?.nombre || "—"}</td>
                  <td>
                    <button
                      className={`btn btn-sm fw-bold ${
                        checkinHecho ? "btn-success" : "btn-outline-success"
                      }`}
                      onClick={() => {
                        if (!checkinHecho && socioId && turnoId)
                          handleCheckin(socioId, turnoId);
                      }}
                      disabled={checkinHecho}
                    >
                      {checkinHecho ? "✅" : "☑️"}
                    </button>
                  </td>
                  <td>
                    {/* 🏋️ Asignar rutina */}
                    <button
                      className="btn btn-sm btn-success me-2"
                      title="Asignar rutina"
                      onClick={async () => {
                        try {
                          const { data } = await gymApi.get("/rutinasplantilla");
                          const rutinas = data.items || data;

                          if (!rutinas || rutinas.length === 0) {
                            Swal.fire("Sin rutinas", "No hay rutinas disponibles.", "info");
                            return;
                          }

                          const { value: rutinaId } = await Swal.fire({
                            title: "🏋️ Asignar rutina al turno",
                            input: "select",
                            inputOptions: Object.fromEntries(
                              rutinas.map((r: any) => [r.id, r.nombre])
                            ),
                            inputPlaceholder: "Seleccioná una rutina",
                            showCancelButton: true,
                            confirmButtonText: "Guardar",
                            cancelButtonText: "Cancelar",
                            confirmButtonColor: "#ff6600",
                          });

                          if (!rutinaId) return;

                          await gymApi.patch(`/suscripcionturno/${t.id}/rutina`, rutinaId);

                          Swal.fire({
                            icon: "success",
                            title: "Rutina asignada",
                            text: "La rutina fue asignada correctamente al turno.",
                            confirmButtonColor: "#ff6600",
                          });

                          fetchTurnos(); // 🔄 refresca la tabla
                        } catch (error: any) {
                          Swal.fire(
                            "Error",
                            error.response?.data?.message || "No se pudo asignar la rutina",
                            "error"
                          );
                        }
                      }}
                    >
                      🏋️
                    </button>

                    {/* 🗑️ Eliminar turno */}
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

      {/* 🔹 Paginación */}
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
