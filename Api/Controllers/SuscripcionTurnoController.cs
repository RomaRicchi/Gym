using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepción, Socio")]
    [ApiController]
    [Route("api/[controller]")]
    public class SuscripcionTurnoController : ControllerBase
    {
        private readonly ISuscripcionTurnoRepository _repo;
        private readonly GymDbContext _db;

        public SuscripcionTurnoController(ISuscripcionTurnoRepository repo, GymDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        // GET: api/SuscripcionTurno
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var turnos = await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .OrderBy(t => t.DiaSemanaId)
                .ThenBy(t => t.HoraInicio)
                .Select(t => new
                {
                    t.Id,
                    Sala = new
                    {
                        t.Sala!.Id,
                        t.Sala.Nombre,
                        t.Sala.Cupo
                    },
                    Profesor = new
                    {
                        t.Personal!.Id,
                        t.Personal.Nombre
                    },
                    Dia = new
                    {
                        t.DiaSemana!.Id,
                        t.DiaSemana.Nombre
                    },
                    t.HoraInicio,
                    t.DuracionMin,
                    t.Activo,

                    // 🔹 Capacidad total de la sala
                    CupoTotal = t.Sala.Cupo,

                    // 🔹 Cupo disponible dinámico (total - inscriptos)
                    CupoDisponible = t.Sala.Cupo -
                        _db.SuscripcionTurnos.Count(st => st.TurnoPlantillaId == t.Id)
                })
                .ToListAsync(ct);

            return Ok(new { ok = true, items = turnos });
        }

        // Obtener todos los turnos activos (para cualquier semana)
        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos(CancellationToken ct = default)
        {
            try
            {
                var turnos = await _db.TurnosPlantilla
                    .Include(t => t.Sala)
                    .Include(t => t.Personal)
                    .Include(t => t.DiaSemana)
                    .Where(t => t.Activo)
                    .OrderBy(t => t.DiaSemanaId)
                    .ThenBy(t => t.HoraInicio)
                    .Select(t => new
                    {
                        t.Id,
                        t.HoraInicio,
                        t.DuracionMin,
                        Dia = new
                        {
                            t.DiaSemana!.Id,
                            t.DiaSemana.Nombre
                        },
                        Sala = new
                        {
                            t.Sala!.Id,
                            t.Sala.Nombre,
                            CupoTotal = t.Sala.Cupo,
                            CupoDisponible = t.Sala.Cupo - _db.SuscripcionTurnos.Count(st => st.TurnoPlantillaId == t.Id)
                        },
                        Profesor = t.Personal != null
                            ? new { t.Personal.Id, t.Personal.Nombre }
                            : new { Id = 0, Nombre = "(sin profesor)" },
                        t.Activo
                    })
                    .ToListAsync(ct);

                return Ok(new
                {
                    ok = true,
                    count = turnos.Count,
                    items = turnos
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "Error al obtener los turnos activos.",
                    detail = ex.Message
                });
            }
        }


        // GET: api/SuscripcionTurno/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var turno = await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .FirstOrDefaultAsync(t => t.Id == id, ct);

            if (turno is null)
                return NotFound(new { message = $"No se encontró el turno con ID {id}" });

            // Calcular cupo disponible dinámicamente
            var inscriptos = await _db.SuscripcionTurnos
                .CountAsync(st => st.TurnoPlantillaId == turno.Id, ct);

            var cupoDisponible = turno.Sala!.Cupo - inscriptos;

            // Devolver objeto enriquecido con relaciones
            var result = new
            {
                turno.Id,
                turno.HoraInicio,
                turno.DuracionMin,
                turno.Activo,

                Sala = new
                {
                    turno.Sala.Id,
                    turno.Sala.Nombre,
                    CupoTotal = turno.Sala.Cupo,
                    CupoDisponible = cupoDisponible
                },
                Profesor = new
                {
                    turno.Personal!.Id,
                    turno.Personal.Nombre
                },
                Dia = new
                {
                    turno.DiaSemana!.Id,
                    turno.DiaSemana.Nombre
                }
            };

            return Ok(result);
        }

        // GET: api/SuscripcionTurno/suscripcion/{id}
        [HttpGet("suscripcion/{id:int}")]
        public async Task<IActionResult> GetBySuscripcion(int id, CancellationToken ct = default)
        {
            var list = await _repo.GetBySuscripcionAsync(id, ct);
            return Ok(list);
        }

        // GET: api/SuscripcionTurno/socio/{id}
        [HttpGet("socio/{id:int}")]
        public async Task<IActionResult> GetBySocio(int id, CancellationToken ct = default)
        {
            var list = await _repo.GetBySocioAsync(id, ct);
            return Ok(list);
        }

        // POST: api/SuscripcionTurno
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] SuscripcionTurnoCreateDto dto, CancellationToken ct = default)
        {
            if (dto == null)
                return BadRequest(new { message = "El cuerpo de la solicitud está vacío." });

            var suscripcion = await _db.Suscripciones
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == dto.SuscripcionId, ct);

            if (suscripcion == null)
                return BadRequest(new { message = "❌ La suscripción especificada no existe." });

            var cupoMaximo = suscripcion.Plan?.DiasPorSemana ?? 0;
            if (cupoMaximo <= 0)
                return BadRequest(new { message = "❌ El plan no tiene cupo configurado." });

            // 🔹 Validar límite de turnos del plan
            var asignadosActuales = await _db.SuscripcionTurnos
                .CountAsync(st => st.SuscripcionId == suscripcion.Id, ct);

            if (asignadosActuales >= cupoMaximo)
                return BadRequest(new { message = "⚠️ Ya alcanzaste el máximo de turnos para esta suscripción." });

            // 🔹 Buscar turno
            var turno = await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .FirstOrDefaultAsync(t => t.Id == dto.TurnoPlantillaId, ct);

            if (turno == null)
                return BadRequest(new { message = "❌ El turno seleccionado no existe." });

            if (!turno.Activo)
                return BadRequest(new { message = "⚠️ El turno está inactivo y no puede asignarse." });

            // 🔹 Duplicado
            bool yaAsignado = await _db.SuscripcionTurnos
                .AnyAsync(st => st.SuscripcionId == suscripcion.Id && st.TurnoPlantillaId == dto.TurnoPlantillaId, ct);

            if (yaAsignado)
                return Conflict(new { message = "⚠️ Este turno ya fue asignado a esta suscripción." });

            // 🔹 Cupo por sala
            var inscriptos = await _db.SuscripcionTurnos
                .CountAsync(st => st.TurnoPlantillaId == turno.Id, ct);
            var cupoSala = turno.Sala?.Cupo ?? 0;

            if (inscriptos >= cupoSala)
                return BadRequest(new { message = "⚠️ No hay más lugares disponibles en esta sala para este turno." });

            // 🔹 Crear nuevo registro
            var nuevo = new SuscripcionTurno
            {
                SuscripcionId = suscripcion.Id,
                TurnoPlantillaId = dto.TurnoPlantillaId
            };

            _db.SuscripcionTurnos.Add(nuevo);
            await _db.SaveChangesAsync(ct);

            // 🔹 Calcular cupo actualizado
            var cupoActualizado = cupoSala - (inscriptos + 1);

            return Ok(new
            {
                ok = true,
                message = "✅ Turno asignado correctamente.",
                cupoActualizado,
                turno = new
                {
                    turno.Id,
                    Sala = turno.Sala?.Nombre,
                    Profesor = turno.Personal?.Nombre,
                    Dia = turno.DiaSemana?.Nombre,
                    turno.HoraInicio,
                    turno.DuracionMin,
                    CupoTotal = cupoSala
                }
            });
        }

        // PUT: api/SuscripcionTurno/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] SuscripcionTurno dto, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null)
                return NotFound(new { message = $"No se encontró el turno asignado con ID {id}" });

            if (!await _db.Suscripciones.AnyAsync(s => s.Id == dto.SuscripcionId, ct))
                return BadRequest(new { message = "La suscripción no existe." });

            if (!await _db.TurnosPlantilla.AnyAsync(t => t.Id == dto.TurnoPlantillaId, ct))
                return BadRequest(new { message = "El turno no existe." });

            entity.SuscripcionId = dto.SuscripcionId;
            entity.TurnoPlantillaId = dto.TurnoPlantillaId;

            await _repo.UpdateAsync(entity, ct);
            return Ok(new { ok = true, message = "✅ Turno actualizado correctamente." });
        }

        // DELETE: api/SuscripcionTurno/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            var entity = await _db.SuscripcionTurnos
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .FirstOrDefaultAsync(st => st.Id == id, ct);

            if (entity is null)
                return NotFound(new { message = $"No se encontró el turno con ID {id}" });

            var turnoId = entity.TurnoPlantillaId;
            var salaNombre = entity.TurnoPlantilla?.Sala?.Nombre ?? "(sin sala)";
            var cupoSala = entity.TurnoPlantilla?.Sala?.Cupo ?? 0;

            _db.SuscripcionTurnos.Remove(entity);
            await _db.SaveChangesAsync(ct);

            // 🔹 Calcular nuevo cupo disponible
            var inscriptos = await _db.SuscripcionTurnos
                .CountAsync(st => st.TurnoPlantillaId == turnoId, ct);
            var cupoActualizado = cupoSala - inscriptos;

            return Ok(new
            {
                ok = true,
                message = $"🗑️ Turno eliminado correctamente. Se liberó un lugar en '{salaNombre}'.",
                cupoActualizado
            });
        }

        // GET: api/SuscripcionTurno/con-checkin
        [HttpGet("con-checkin")]
        public async Task<IActionResult> GetAllConCheckin(CancellationToken ct = default)
        {
            var result = await _repo.GetAllWithCheckinAsync(ct);
            return Ok(new { ok = true, items = result });
        }

        [HttpPost("reagendar")]
        public async Task<IActionResult> Reagendar([FromBody] JsonElement body, CancellationToken ct = default)
        {
            try
            {
                // 🔸 Extraer datos del JSON
                int suscripcionId = body.GetProperty("suscripcionId").GetInt32();
                int turnoActualId = body.GetProperty("turnoActualId").GetInt32();
                int nuevoTurnoId = body.GetProperty("nuevoTurnoId").GetInt32();

                var suscripcion = await _db.Suscripciones
                    .Include(s => s.Plan)
                    .FirstOrDefaultAsync(s => s.Id == suscripcionId, ct);
                if (suscripcion == null)
                    return NotFound(new { message = "Suscripción no encontrada." });

                var actual = await _db.SuscripcionTurnos
                    .Include(st => st.TurnoPlantilla)
                    .FirstOrDefaultAsync(st => st.Id == turnoActualId, ct);
                if (actual == null)
                    return NotFound(new { message = "Turno actual no encontrado." });

                var nuevo = await _db.TurnosPlantilla
                    .Include(t => t.Sala)
                    .Include(t => t.DiaSemana)
                    .FirstOrDefaultAsync(t => t.Id == nuevoTurnoId, ct);
                if (nuevo == null)
                    return NotFound(new { message = "Nuevo turno no encontrado." });

                // Validar que al turno actual le falte más de 1 hora
                var ahora = DateTime.UtcNow;
                var horaInicioActual = actual.TurnoPlantilla?.HoraInicio ?? TimeSpan.Zero;
                var fechaTurnoActual = ahora.Date.Add(horaInicioActual); // simula el turno de hoy

                if (fechaTurnoActual <= ahora.AddHours(1))
                    return BadRequest(new { message = "Solo se puede reagendar con al menos 1 hora de anticipación." });

                // Validar que el nuevo turno esté dentro de esta semana
                var finSemana = ahora.Date.AddDays(7 - (int)ahora.DayOfWeek);
                var fechaNuevoTurno = ahora.Date.Add(nuevo.HoraInicio);
                if (fechaNuevoTurno.Date > finSemana.Date)
                    return BadRequest(new { message = "Solo se puede reagendar dentro de esta semana." });

                // Eliminar el turno anterior
                _db.SuscripcionTurnos.Remove(actual);

                // Verificar cupo real
                var inscriptos = await _db.SuscripcionTurnos
                    .CountAsync(st => st.TurnoPlantillaId == nuevoTurnoId, ct);
                var cupo = nuevo.Sala?.Cupo ?? 0;
                if (inscriptos >= cupo)
                    return BadRequest(new { message = "No hay cupos disponibles en el turno seleccionado." });

                // Crear el nuevo turno
                var nuevoST = new SuscripcionTurno
                {
                    SuscripcionId = suscripcionId,
                    TurnoPlantillaId = nuevoTurnoId
                };

                _db.SuscripcionTurnos.Add(nuevoST);
                await _db.SaveChangesAsync(ct);

                return Ok(new { ok = true, message = "Turno reagendado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPatch("{id}/rutina")]
        public async Task<IActionResult> AsignarRutina(
            [FromRoute] int id,
            [FromBody] int rutinaId,
            CancellationToken ct = default)
        {
            try
            {
                var ok = await _repo.AsignarRutinaAsync(id, rutinaId, ct);

                if (!ok)
                    return NotFound(new { message = "No se pudo asignar la rutina (turno o rutina inexistente)." });

                return Ok(new
                {
                    ok = true,
                    message = "Rutina asignada correctamente y registrada en historial."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR AsignarRutina] {ex.Message}");
                return StatusCode(500, new { message = "Error interno al asignar rutina." });
            }
        }


    }
}
