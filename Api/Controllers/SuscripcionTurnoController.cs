using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                        t.Sala.Id,
                        t.Sala.Nombre,
                        t.Sala.Cupo
                    },
                    Profesor = new
                    {
                        t.Personal.Id,
                        t.Personal.Nombre
                    },
                    Dia = new
                    {
                        t.DiaSemana.Id,
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

        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos(CancellationToken ct = default)
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
                    t.Activo,

                    Sala = new
                    {
                        t.Sala.Id,
                        t.Sala.Nombre,
                        CupoTotal = t.Sala.Cupo,
                        CupoDisponible = t.Sala.Cupo -
                            _db.SuscripcionTurnos.Count(st => st.TurnoPlantillaId == t.Id)
                    },
                    Profesor = new
                    {
                        t.Personal.Id,
                        t.Personal.Nombre
                    },
                    Dia = new
                    {
                        t.DiaSemana.Id,
                        t.DiaSemana.Nombre
                    }
                })
                .ToListAsync(ct);

            return Ok(new { ok = true, items = turnos });
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

            // 🔹 Calcular cupo disponible dinámicamente
            var inscriptos = await _db.SuscripcionTurnos
                .CountAsync(st => st.TurnoPlantillaId == turno.Id, ct);

            var cupoDisponible = turno.Sala.Cupo - inscriptos;

            // 🔹 Devolver objeto enriquecido con relaciones
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
                    turno.Personal.Id,
                    turno.Personal.Nombre
                },
                Dia = new
                {
                    turno.DiaSemana.Id,
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

            // 🔹 Validar existencia de la suscripción
            var suscripcionExiste = await _db.Suscripciones.AnyAsync(s => s.Id == dto.SuscripcionId, ct);
            if (!suscripcionExiste)
                return BadRequest(new { message = "❌ La suscripción especificada no existe." });

            // 🔹 Buscar el turno con su sala
            var turno = await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .FirstOrDefaultAsync(t => t.Id == dto.TurnoPlantillaId, ct);

            if (turno == null)
                return BadRequest(new { message = "❌ El turno seleccionado no existe." });

            if (!turno.Activo)
                return BadRequest(new { message = "⚠️ El turno está inactivo y no puede asignarse." });

            // 🔹 Validar duplicado
            bool yaAsignado = await _db.SuscripcionTurnos
                .AnyAsync(st => st.SuscripcionId == dto.SuscripcionId && st.TurnoPlantillaId == dto.TurnoPlantillaId, ct);
            if (yaAsignado)
                return Conflict(new { message = "⚠️ Este turno ya fue asignado a esta suscripción." });

            // 🔹 Calcular cupo disponible dinámico
            var inscriptos = await _db.SuscripcionTurnos
                .CountAsync(st => st.TurnoPlantillaId == turno.Id, ct);

            var cupoDisponible = turno.Sala.Cupo - inscriptos;

            if (cupoDisponible <= 0)
                return BadRequest(new { message = "⚠️ No hay más lugares disponibles en esta sala para este turno." });

            // 🔹 Crear la nueva asignación
            var nuevo = new SuscripcionTurno
            {
                SuscripcionId = dto.SuscripcionId,
                TurnoPlantillaId = dto.TurnoPlantillaId
            };

            _db.SuscripcionTurnos.Add(nuevo);
            await _db.SaveChangesAsync(ct);

            // 🔹 Devolver datos enriquecidos
            var creado = await _db.SuscripcionTurnos
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.DiaSemana)
                .FirstAsync(st => st.Id == nuevo.Id, ct);

            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, new
            {
                creado.Id,
                creado.SuscripcionId,
                Turno = new
                {
                    creado.TurnoPlantilla.Id,
                    Sala = creado.TurnoPlantilla.Sala?.Nombre,
                    Profesor = creado.TurnoPlantilla.Personal?.Nombre,
                    Dia = creado.TurnoPlantilla.DiaSemana?.Nombre,
                    creado.TurnoPlantilla.HoraInicio,
                    creado.TurnoPlantilla.DuracionMin,
                    CupoTotal = creado.TurnoPlantilla.Sala?.Cupo,
                    CupoDisponible = cupoDisponible - 1 // 🔸 cuenta regresiva en respuesta
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

            _db.SuscripcionTurnos.Remove(entity);
            await _db.SaveChangesAsync(ct);

            return Ok(new
            {
                ok = true,
                message = $"🗑️ Turno eliminado correctamente. Se liberó un lugar en '{entity.TurnoPlantilla.Sala?.Nombre}'."
            });
        }

        // GET: api/SuscripcionTurno/con-checkin
        [HttpGet("con-checkin")]
        public async Task<IActionResult> GetAllConCheckin(CancellationToken ct = default)
        {
            var result = await _repo.GetAllWithCheckinAsync(ct);
            return Ok(new { ok = true, items = result });
        }

        // Cancelar turno con al menos 1h de anticipación
        [HttpPost("{id:int}/cancelar")]
        public async Task<IActionResult> CancelarTurno(int id, CancellationToken ct = default)
        {
            try
            {
                var turno = await _db.SuscripcionTurnos
                    .Include(t => t.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                    .FirstOrDefaultAsync(t => t.Id == id, ct);

                if (turno == null)
                    return NotFound(new { message = "Turno no encontrado." });

                var ahora = DateTime.UtcNow;
                var horaInicio = turno.TurnoPlantilla?.HoraInicio ?? new TimeSpan(0, 0, 0);
                var fechaTurno = ahora.Date.Add(horaInicio); // combina fecha de hoy + hora del turno

                if (fechaTurno <= ahora.AddHours(1))
                    return BadRequest(new { message = "Solo se puede cancelar con al menos 1 hora de anticipación." });

                _db.SuscripcionTurnos.Remove(turno);
                await _db.SaveChangesAsync(ct);

                return Ok(new { ok = true, message = "Turno cancelado correctamente. Cupo liberado." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Reagendar turno dentro de la misma semana
        [HttpPost("reagendar")]
        public async Task<IActionResult> Reagendar([FromBody] dynamic body, CancellationToken ct = default)
        {
            try
            {
                int suscripcionId = (int)body.suscripcionId;
                int turnoActualId = (int)body.turnoActualId;
                int nuevoTurnoId = (int)body.nuevoTurnoId;

                var suscripcion = await _db.Suscripciones
                    .Include(s => s.Plan)
                    .FirstOrDefaultAsync(s => s.Id == suscripcionId, ct);

                if (suscripcion == null)
                    return NotFound(new { message = "Suscripción no encontrada." });

                var actual = await _db.SuscripcionTurnos.FirstOrDefaultAsync(st => st.Id == turnoActualId, ct);
                if (actual == null)
                    return NotFound(new { message = "Turno actual no encontrado." });

                var nuevo = await _db.TurnosPlantilla
                    .Include(t => t.Sala)
                    .Include(t => t.DiaSemana)
                    .FirstOrDefaultAsync(t => t.Id == nuevoTurnoId, ct);

                if (nuevo == null)
                    return NotFound(new { message = "Nuevo turno no encontrado." });

                // 📅 Validar que el nuevo turno esté dentro de la semana actual
                var hoy = DateTime.UtcNow;
                var finSemana = hoy.Date.AddDays(7 - (int)hoy.DayOfWeek);
                var fechaCompleta = hoy.Date.Add(nuevo.HoraInicio);

                if (fechaCompleta.Date > finSemana.Date)
                    return BadRequest(new { message = "Solo se puede reagendar dentro de esta semana." });

                // ✅ Liberar turno anterior
                _db.SuscripcionTurnos.Remove(actual);

                // ✅ Verificar cupo
                var inscriptos = await _db.SuscripcionTurnos
                    .CountAsync(st => st.TurnoPlantillaId == nuevoTurnoId, ct);

                var cupo = nuevo.Sala?.Cupo ?? 0;
                if (inscriptos >= cupo)
                    return BadRequest(new { message = "No hay cupos disponibles en el turno seleccionado." });

                // ✅ Crear nuevo turno
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
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
