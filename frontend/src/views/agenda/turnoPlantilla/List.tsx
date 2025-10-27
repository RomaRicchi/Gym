import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { crearTurnoPlantilla } from "./TurnoPlantillaCreate";
import { editarTurnoPlantilla } from "./TurnoPlantillaEdit";
import Pagination from "@/components/Pagination";
import Swal from "sweetalert2";
import gymApi from "@/api/gymApi";

interface Turno {
  id: number;
  sala: { id: number; nombre: string; cupo: number };
  personal: { id: number; nombre: string };
  dia_semana: { id: number; nombre: string };
  hora_inicio: string;
  duracion_min: number;
  activo: boolean | number;
  cupo: number; // se completa desde la sala
  sala_id?: number;
  personal_id?: number;
  dia_semana_id?: number;
}

export default function TurnosPlantillaList() {
  const [turnos, setTurnos] = useState<Turno[]>([]);
  const [filtered, setFiltered] = useState<Turno[]>([]);
  const [salas, setSalas] = useState<any[]>([]);
  const [profesores, setProfesores] = useState<any[]>([]);
  const [dias, setDias] = useState<any[]>([]);
  const [filtroDia, setFiltroDia] = useState<string>("");
  const [filtroProfesor, setFiltroProfesor] = useState<string>("");
  const [filtroSala, setFiltroSala] = useState<string>("");
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(10); // cantidad por página
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    const total = filtered.length;
    setTotalPages(Math.ceil(total / itemsPerPage));

    if (currentPage > Math.ceil(total / itemsPerPage)) {
      setCurrentPage(1);
    }
  }, [filtered, itemsPerPage, currentPage]);

  // 🔹 Cargar turnos, profesores, salas y días
  const fetchTurnos = async () => {
    try {
      const [resTurnos, resProfesores, resSalas, resDias] = await Promise.all([
        gymApi.get("/turnosplantilla"),
        gymApi.get("/personal"),
        gymApi.get("/salas"),
        gymApi.get("/diasemana"), 
      ]);

      console.log("✅ TURNOS:", resTurnos.data);

      const data = resTurnos.data.items || resTurnos.data;
      if (!Array.isArray(data)) throw new Error("Formato inesperado de datos");

      // 🔧 Normalizar campos
      const parsed = data
        .map((t: any) => ({
          ...t,
          activo: Boolean(t.activo),
          dia_semana_id: t.dia_semana_id ?? t.diaSemanaId ?? t.dia_semana?.id,
          hora_inicio: t.hora_inicio ?? t.horaInicio,
          duracion_min: t.duracion_min ?? t.duracionMin,
          personal_id: t.personal_id ?? t.personalId ?? t.personal?.id,
          sala_id: t.sala_id ?? t.salaId ?? t.sala?.id,
          dia_semana: t.dia_semana ?? t.diaSemana ?? null,
          cupo: t.sala?.cupo ?? 0, // ✅ cupo traído desde la sala
        }))
        .sort(
          (a: any, b: any) =>
            (a.dia_semana_id || 0) - (b.dia_semana_id || 0) ||
            (a.hora_inicio || "").localeCompare(b.hora_inicio || "")
        );

      setTurnos(parsed);
      setFiltered(parsed);
      setProfesores(resProfesores.data.items || resProfesores.data);
      setSalas(resSalas.data.items || resSalas.data);
      setDias(resDias.data.items || resDias.data);
    } catch (error: any) {
      console.error("❌ ERROR al cargar turnos:", error.response || error.message);
      Swal.fire("Error", "No se pudieron cargar los turnos", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTurnos();
  }, []);

  // 🔹 Aplicar filtros
  useEffect(() => {
    let temp = [...turnos];

    if (filtroDia)
      temp = temp.filter((t) => t.dia_semana_id === Number(filtroDia));

    if (filtroProfesor)
      temp = temp.filter((t) => t.personal_id === Number(filtroProfesor));

    if (filtroSala)
      temp = temp.filter((t) => t.sala_id === Number(filtroSala));

    setFiltered(temp);
  }, [filtroDia, filtroProfesor, filtroSala, turnos]);

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

    if (result.isConfirmed) {
      try {
        await gymApi.delete(`/turnosplantilla/${id}`);
        Swal.fire("Eliminado", "Turno eliminado correctamente", "success");
        fetchTurnos();
      } catch {
        Swal.fire("Error", "No se pudo eliminar el turno", "error");
      }
    }
  };

  if (loading) return <p className="text-center mt-4">Cargando turnos...</p>;
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const visibleTurnos = filtered.slice(startIndex, endIndex);

  return (
    <div className="mt-4 container">
      <h1
        className="text-center fw-bold mb-4"
        style={{ color: "#ff6600", fontSize: "2.5rem", letterSpacing: "2px" }}
      >
        PLANILLA DE TURNOS
      </h1>

      {/* 🔸 FILTROS */}
      <div className="card mb-3 p-3 shadow-sm">
        <div className="row g-3 align-items-end">
          <div className="col-md-4">
            <label className="form-label">Filtrar por Día</label>
            <select
              className="form-select"
              value={filtroDia}
              onChange={(e) => setFiltroDia(e.target.value)}
            >
              <option value="">Todos los días</option>
              {dias.map((d) => (
                <option key={d.id} value={d.id}>
                  {d.nombre}
                </option>
              ))}
            </select>
          </div>

          <div className="col-md-4">
            <label className="form-label">Filtrar por Profesor</label>
            <select
              className="form-select"
              value={filtroProfesor}
              onChange={(e) => setFiltroProfesor(e.target.value)}
            >
              <option value="">Todos los profesores</option>
              {profesores.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.nombre}
                </option>
              ))}
            </select>
          </div>

          <div className="col-md-4">
            <label className="form-label">Filtrar por Sala</label>
            <select
              className="form-select"
              value={filtroSala}
              onChange={(e) => setFiltroSala(e.target.value)}
            >
              <option value="">Todas las salas</option>
              {salas.map((s) => (
                <option key={s.id} value={s.id}>
                  {`${s.nombre} (${s.cupo} cupos)`} {/* ✅ cupo visible */}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="mt-3 d-flex justify-content-end gap-2">
          <button
            className="btn btn-secondary"
            onClick={() => {
              setFiltroDia("");
              setFiltroProfesor("");
              setFiltroSala("");
            }}
          >
            Limpiar Filtros
          </button>
          <button
            className="btn btn-success"
            onClick={() => crearTurnoPlantilla(fetchTurnos)} // ✅ refresca lista
          >
            ➕ Nuevo Turno
          </button>
        </div>
      </div>

      {/* 🔸 TABLA */}
      <table className="table table-striped table-hover text-center align-middle">
        <thead className="table-dark">
          <tr>
            <th>Sala</th>
            <th>Profesor</th>
            <th>Día</th>
            <th>Inicio</th>
            <th>Duración</th>
            <th>Cupo</th>
            <th>Activo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {visibleTurnos.map((t) => (
            <tr key={t.id}>
              <td>{t.sala?.nombre || "-"}</td>
              <td>{t.personal?.nombre || "-"}</td>
              <td>{t.dia_semana?.nombre || "-"}</td>
              <td>{t.hora_inicio}</td>
              <td>{t.duracion_min} min</td>
              <td>{t.cupo}</td> {/* ✅ cupo real de la sala */}
              <td>{t.activo ? "✅" : "❌"}</td>
              <td>
                <button
                  className="btn btn-warning"
                  onClick={() => editarTurnoPlantilla(t.id, fetchTurnos)}
                >
                  ✏️
                </button>
                <button
                  className="btn btn-danger"
                  onClick={() => handleDelete(t.id)}
                >
                  🗑️
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <Pagination
            currentPage={currentPage}
            totalPages={totalPages}
            totalItems={filtered.length}
            pageSize={itemsPerPage}
            onPageChange={(page) => setCurrentPage(page)}
      />
    </div>
    
  );
}
