using System.Security.Claims;
using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Turnos;
using Gym.Domain.Entities;
using Gym.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.API.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class TurnosController : ControllerBase
{
    private readonly GymDbContext _db;

    public TurnosController(GymDbContext db)
    {
        _db = db;
    }

    [AllowAnonymous]
    [HttpGet("diasemana")]
    public async Task<IActionResult> GetDiasSemana(CancellationToken ct)
    {
        var dias = await _db.DiasSemana
            .AsNoTracking()
            .OrderBy(d => d.Id)
            .ToListAsync(ct);

        return Ok(dias);
    }

    [HttpGet("salas")]
    public async Task<IActionResult> GetSalas(CancellationToken ct)
    {
        var salas = await _db.Salas.AsNoTracking().ToListAsync(ct);

        // Cupos disponibles por sala (turnos activos con suscripción vigente)
        var hoy = DateTime.UtcNow.Date;
        var usadosPorSala = await _db.SuscripcionTurnos
            .Include(st => st.Suscripcion)
            .Where(st => st.Suscripcion.Estado && st.Suscripcion.Fin >= hoy)
            .GroupBy(st => st.TurnoPlantilla.SalaId)
            .Select(g => new { SalaId = g.Key, Cantidad = g.Count() })
            .ToDictionaryAsync(x => x.SalaId, x => x.Cantidad, ct);

        var result = salas.Select(s => new SalaDto
        {
            Id = s.Id,
            Nombre = s.Nombre,
            Cupo = s.Cupo,
            Activa = s.Activa,
            CupoDisponible = Math.Max(0, s.Cupo - (usadosPorSala.TryGetValue(s.Id, out var usados) ? usados : 0))
        }).ToList();

        return Ok(result);
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpPost("salas")]
    public async Task<IActionResult> CreateSala([FromBody] SalaCreateRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest(new { message = "El nombre es obligatorio." });

        var sala = new Sala
        {
            Nombre = request.Nombre.Trim(),
            Cupo = request.Cupo,
            Activa = request.Activa
        };

        _db.Salas.Add(sala);
        await _db.SaveChangesAsync(ct);

        return Ok(new SalaDto
        {
            Id = sala.Id,
            Nombre = sala.Nombre,
            Cupo = sala.Cupo,
            Activa = sala.Activa,
            CupoDisponible = sala.Cupo
        });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpPut("salas/{id:int}")]
    public async Task<IActionResult> UpdateSala(int id, [FromBody] SalaUpdateRequest request, CancellationToken ct)
    {
        var sala = await _db.Salas.FindAsync(new object[] { id }, ct);
        if (sala == null)
            return NotFound(new { message = "Sala no encontrada." });

        sala.Nombre = request.Nombre;
        sala.Cupo = request.Cupo;
        sala.Activa = request.Activa;

        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Sala actualizada correctamente." });
    }

    [HttpGet("turnosplantilla")]
    public async Task<IActionResult> GetTurnosPlantilla(
        int page = 1,
        int pageSize = 50,
        bool? activo = null,
        CancellationToken ct = default)
    {
        var query = _db.TurnosPlantilla
            .Include(t => t.Sala)
            .Include(t => t.Personal)
            .Include(t => t.DiaSemana)
            .AsQueryable();

        if (activo.HasValue)
            query = query.Where(t => t.Activo == activo.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(t => t.DiaSemanaId)
            .ThenBy(t => t.HoraInicio)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var cupos = await _db.SuscripcionTurnos
            .Include(st => st.Suscripcion)
            .Where(st => st.Suscripcion.Estado && st.Suscripcion.Fin >= DateTime.UtcNow.Date)
            .GroupBy(st => st.TurnoPlantillaId)
            .Select(g => new { TurnoId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TurnoId, x => x.Count, ct);

        var dtos = items.Select(t =>
        {
            var asignados = cupos.TryGetValue(t.Id, out var c) ? c : 0;
            var cupoDisp = Math.Max(0, (t.Sala?.Cupo ?? 0) - asignados);

            return new
            {
                id = t.Id,
                sala = t.Sala == null ? null : new
                {
                    id = t.SalaId,
                    nombre = t.Sala.Nombre,
                    cupo = t.Sala.Cupo,
                    cupoTotal = t.Sala.Cupo,
                    cupoDisponible = cupoDisp
                },
                personal = t.Personal == null ? null : new { id = t.PersonalId, nombre = t.Personal.Nombre },
                diaSemana = t.DiaSemana == null ? null : new { id = t.DiaSemanaId, nombre = t.DiaSemana.Nombre },
                horaInicio = t.HoraInicio.ToString(@"hh\:mm"),
                duracionMin = t.DuracionMin,
                activo = t.Activo,
                cupoDisponible = cupoDisp
            };
        }).ToList();

        return Ok(new PaginatedResult<object>
        {
            Items = dtos.Cast<object>().ToList(),
            TotalItems = total,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("turnosplantilla/activos")]
    public async Task<IActionResult> GetTurnosActivos(CancellationToken ct)
    {
        var turnos = await _db.TurnosPlantilla
            .Include(t => t.Sala)
            .Include(t => t.Personal)
            .Include(t => t.DiaSemana)
            .Where(t => t.Activo)
            .OrderBy(t => t.DiaSemanaId)
            .ThenBy(t => t.HoraInicio)
            .ToListAsync(ct);

        var cupos = await _db.SuscripcionTurnos
            .Include(st => st.Suscripcion)
            .Where(st => st.Suscripcion.Estado && st.Suscripcion.Fin >= DateTime.UtcNow.Date)
            .GroupBy(st => st.TurnoPlantillaId)
            .Select(g => new { TurnoId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TurnoId, x => x.Count, ct);

        var result = turnos.Select(t =>
        {
            var asignados = cupos.TryGetValue(t.Id, out var c) ? c : 0;
            var cupoDisp = Math.Max(0, (t.Sala?.Cupo ?? 0) - asignados);

            return new
            {
                id = t.Id,
                sala = t.Sala == null ? null : new
                {
                    id = t.SalaId,
                    nombre = t.Sala.Nombre,
                    cupoTotal = t.Sala.Cupo,
                    cupoDisponible = cupoDisp
                },
                personal = t.Personal == null ? null : new { id = t.PersonalId, nombre = t.Personal.Nombre },
                diaSemana = t.DiaSemana == null ? null : new { id = t.DiaSemanaId, nombre = t.DiaSemana.Nombre },
                horaInicio = t.HoraInicio.ToString(@"hh\:mm"),
                duracionMin = t.DuracionMin,
                activo = t.Activo,
                cupoDisponible = cupoDisp
            };
        });

        return Ok(result);
    }

    [HttpGet("turnosplantilla/dia/{diaId:int}")]
    public async Task<IActionResult> GetTurnosPorDia(int diaId, CancellationToken ct)
    {
        var turnos = await _db.TurnosPlantilla
            .Include(t => t.Sala)
            .Include(t => t.Personal)
            .Include(t => t.DiaSemana)
            .Where(t => t.Activo && t.DiaSemanaId == diaId)
            .OrderBy(t => t.HoraInicio)
            .ToListAsync(ct);

        var cupos = await _db.SuscripcionTurnos
            .Include(st => st.Suscripcion)
            .Where(st => st.TurnoPlantilla.DiaSemanaId == diaId && st.Suscripcion.Estado && st.Suscripcion.Fin >= DateTime.UtcNow.Date)
            .GroupBy(st => st.TurnoPlantillaId)
            .Select(g => new { TurnoId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TurnoId, x => x.Count, ct);

        var result = turnos.Select(t =>
        {
            var asignados = cupos.TryGetValue(t.Id, out var c) ? c : 0;
            var cupoDisp = Math.Max(0, (t.Sala?.Cupo ?? 0) - asignados);

            return new
            {
                id = t.Id,
                sala = t.Sala == null ? null : new
                {
                    id = t.SalaId,
                    nombre = t.Sala.Nombre,
                    cupoTotal = t.Sala.Cupo,
                    cupoDisponible = cupoDisp
                },
                personal = t.Personal == null ? null : new { id = t.PersonalId, nombre = t.Personal.Nombre },
                diaSemana = t.DiaSemana == null ? null : new { id = t.DiaSemanaId, nombre = t.DiaSemana.Nombre },
                horaInicio = t.HoraInicio.ToString(@"hh\:mm"),
                duracionMin = t.DuracionMin,
                activo = t.Activo,
                cupoDisponible = cupoDisp
            };
        });

        return Ok(result);
    }

    [HttpGet("turnosplantilla/{id:int}")]
    public async Task<IActionResult> GetTurnoPlantilla(int id, CancellationToken ct)
    {
        var turno = await _db.TurnosPlantilla
            .Include(t => t.Sala)
            .Include(t => t.Personal)
            .Include(t => t.DiaSemana)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (turno == null)
            return NotFound(new { message = "Turno no encontrado." });

        return Ok(new
        {
            id = turno.Id,
            salaId = turno.SalaId,
            personalId = turno.PersonalId,
            diaSemanaId = turno.DiaSemanaId,
            horaInicio = turno.HoraInicio.ToString(@"hh\:mm"),
            duracionMin = turno.DuracionMin,
            activo = turno.Activo,
            sala = turno.Sala == null ? null : new { turno.Sala.Id, turno.Sala.Nombre, turno.Sala.Cupo },
            personal = turno.Personal == null ? null : new { turno.Personal.Id, turno.Personal.Nombre },
            diaSemana = turno.DiaSemana == null ? null : new { turno.DiaSemana.Id, turno.DiaSemana.Nombre }
        });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpPost("turnosplantilla/crear")]
    public async Task<IActionResult> CreateTurnoPlantilla([FromBody] TurnoPlantillaCreateRequest request, CancellationToken ct)
    {
        if (request.SalaId <= 0 || request.PersonalId <= 0 || request.DiaSemanaId <= 0)
            return BadRequest(new { message = "Sala, personal y día de la semana son obligatorios." });

        var entity = new TurnoPlantilla
        {
            SalaId = request.SalaId,
            PersonalId = request.PersonalId,
            DiaSemanaId = request.DiaSemanaId,
            HoraInicio = request.HoraInicio,
            DuracionMin = request.DuracionMin,
            Activo = request.Activo
        };

        _db.TurnosPlantilla.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Turno creado correctamente.", id = entity.Id });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpPut("turnosplantilla/{id:int}")]
    public async Task<IActionResult> UpdateTurnoPlantilla(int id, [FromBody] TurnoPlantillaUpdateRequest request, CancellationToken ct)
    {
        var turno = await _db.TurnosPlantilla.FindAsync(new object[] { id }, ct);
        if (turno == null)
            return NotFound(new { message = "Turno no encontrado." });

        turno.SalaId = request.SalaId;
        turno.PersonalId = request.PersonalId;
        turno.DiaSemanaId = request.DiaSemanaId;
        turno.HoraInicio = request.HoraInicio;
        turno.DuracionMin = request.DuracionMin;
        turno.Activo = request.Activo;

        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Turno actualizado correctamente." });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpDelete("turnosplantilla/{id:int}")]
    public async Task<IActionResult> DeleteTurnoPlantilla(int id, CancellationToken ct)
    {
        var turno = await _db.TurnosPlantilla.FindAsync(new object[] { id }, ct);
        if (turno == null)
            return NotFound(new { message = "Turno no encontrado." });

        _db.TurnosPlantilla.Remove(turno);
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Turno eliminado correctamente." });
    }

    // ----- Suscripción a turnos -----

    [HttpGet("suscripcionturno/suscripcion/{suscripcionId:int}")]
    public async Task<IActionResult> GetTurnosBySuscripcion(int suscripcionId, CancellationToken ct)
    {
        var turnos = await _db.SuscripcionTurnos
            .Include(st => st.TurnoPlantilla).ThenInclude(tp => tp.Sala)
            .Include(st => st.TurnoPlantilla).ThenInclude(tp => tp.Personal)
            .Include(st => st.TurnoPlantilla).ThenInclude(tp => tp.DiaSemana)
            .Where(st => st.SuscripcionId == suscripcionId)
            .ToListAsync(ct);

        var result = turnos.Select(st => new
        {
            id = st.Id,
            suscripcionId = st.SuscripcionId,
            turnoPlantillaId = st.TurnoPlantillaId,
            turnoPlantilla = st.TurnoPlantilla == null ? null : new
            {
                id = st.TurnoPlantilla.Id,
                horaInicio = st.TurnoPlantilla.HoraInicio.ToString(@"hh\:mm"),
                duracionMin = st.TurnoPlantilla.DuracionMin,
                diaSemana = st.TurnoPlantilla.DiaSemana == null
                    ? null
                    : new { id = st.TurnoPlantilla.DiaSemana.Id, nombre = st.TurnoPlantilla.DiaSemana.Nombre },
                sala = st.TurnoPlantilla.Sala == null
                    ? null
                    : new
                    {
                        id = st.TurnoPlantilla.Sala.Id,
                        nombre = st.TurnoPlantilla.Sala.Nombre,
                        cupo = st.TurnoPlantilla.Sala.Cupo,
                        cupoDisponible = st.TurnoPlantilla.Sala.Cupo
                    },
                personal = st.TurnoPlantilla.Personal == null
                    ? null
                    : new { id = st.TurnoPlantilla.Personal.Id, nombre = st.TurnoPlantilla.Personal.Nombre }
            },
            rutina = st.Rutina == null ? null : new { id = st.Rutina.Id, nombre = st.Rutina.Nombre }
        });

        return Ok(result);
    }

    [HttpGet("suscripcionturno/socio/{socioId:int}")]
    public async Task<IActionResult> GetTurnosBySocio(int socioId, CancellationToken ct)
    {
        var hoy = DateTime.UtcNow.Date;
        var turnos = await _db.SuscripcionTurnos
            .Include(st => st.TurnoPlantilla).ThenInclude(tp => tp.Sala)
            .Include(st => st.TurnoPlantilla).ThenInclude(tp => tp.Personal)
            .Include(st => st.TurnoPlantilla).ThenInclude(tp => tp.DiaSemana)
            .Include(st => st.Suscripcion)
            .Where(st => st.Suscripcion.SocioId == socioId && st.Suscripcion.Estado && st.Suscripcion.Fin >= hoy)
            .ToListAsync(ct);

        var result = turnos.Select(st => new
        {
            id = st.Id,
            suscripcionId = st.SuscripcionId,
            turnoPlantillaId = st.TurnoPlantillaId,
            turnoPlantilla = st.TurnoPlantilla == null ? null : new
            {
                id = st.TurnoPlantilla.Id,
                horaInicio = st.TurnoPlantilla.HoraInicio.ToString(@"hh\:mm"),
                duracionMin = st.TurnoPlantilla.DuracionMin,
                diaSemana = st.TurnoPlantilla.DiaSemana == null
                    ? null
                    : new { id = st.TurnoPlantilla.DiaSemana.Id, nombre = st.TurnoPlantilla.DiaSemana.Nombre },
                sala = st.TurnoPlantilla.Sala == null
                    ? null
                    : new { nombre = st.TurnoPlantilla.Sala.Nombre, cupo = st.TurnoPlantilla.Sala.Cupo },
                personal = st.TurnoPlantilla.Personal == null
                    ? null
                    : new { nombre = st.TurnoPlantilla.Personal.Nombre }
            }
        });

        return Ok(result);
    }

    [HttpGet("suscripcionturno/rutina/socio")]
    public async Task<IActionResult> GetRutinaDelDia(CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                          User.FindFirstValue(ClaimTypes.Name) ??
                          User.FindFirstValue("sub");
        int? userId = int.TryParse(userIdClaim, out var parsed) ? parsed : null;

        if (!userId.HasValue)
            return Unauthorized(new { message = "No se pudo identificar al usuario." });

        var user = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId.Value, ct);
        if (user?.SocioId == null)
            return NotFound(new { message = "Usuario no está asociado a un socio." });

        var socioId = user.SocioId.Value;
        var hoy = DateTime.UtcNow.Date;
        var diaSemanaId = ((int)hoy.DayOfWeek + 6) % 7 + 1; // Monday=1 ... Sunday=7

        var turno = await _db.SuscripcionTurnos
            .Include(st => st.Rutina).ThenInclude(r => r!.RutinaPlantillaEjercicios)
            .ThenInclude(rpe => rpe.Ejercicio)
            .Include(st => st.TurnoPlantilla).ThenInclude(tp => tp.DiaSemana)
            .Include(st => st.Suscripcion)
            .Where(st =>
                st.Suscripcion.SocioId == socioId &&
                st.Suscripcion.Estado &&
                st.Suscripcion.Fin >= hoy &&
                st.TurnoPlantilla.DiaSemanaId == diaSemanaId)
            .FirstOrDefaultAsync(ct);

        if (turno?.Rutina == null)
            return NotFound(new { message = "No hay rutina asignada para hoy." });

        var rutina = turno.Rutina;
        var ejercicios = rutina.RutinaPlantillaEjercicios
            .OrderBy(e => e.Orden)
            .Select(e => new
            {
                e.Id,
                e.Series,
                e.Repeticiones,
                e.DescansoSeg,
                e.Orden,
                nombre = e.Ejercicio?.Nombre,
                tips = e.Ejercicio?.Tips,
                mediaUrl = e.Ejercicio?.MediaUrl
            }).ToList();

        // Observación: última del profesor (si existe) tomada del check-in
        var checkin = await _db.Checkins
            .Include(c => c.Profesor)
            .Where(c => c.SocioId == socioId && c.TurnoPlantillaId == turno.TurnoPlantillaId && c.Observaciones != null)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(ct);

        return Ok(new
        {
            id = rutina.Id,
            nombre = rutina.Nombre,
            objetivo = rutina.Objetivo,
            imagenUrl = rutina.ImagenUrl,
            dia = turno.TurnoPlantilla?.DiaSemana?.Nombre,
            ejercicios,
            observacion = checkin == null
                ? null
                : new
                {
                    profesor = checkin.Profesor?.Nombre,
                    texto = checkin.Observaciones,
                    fecha = checkin.CreatedAt
                }
        });
    }

    [HttpGet("SuscripcionTurno/con-checkin")]
    public async Task<IActionResult> GetTurnosConCheckin(CancellationToken ct)
    {
        var turnos = await _db.SuscripcionTurnos
            .Include(st => st.Suscripcion).ThenInclude(s => s.Socio)
            .Include(st => st.TurnoPlantilla).ThenInclude(tp => tp.Sala)
            .Include(st => st.TurnoPlantilla).ThenInclude(tp => tp.Personal)
            .Include(st => st.TurnoPlantilla).ThenInclude(tp => tp.DiaSemana)
            .Include(st => st.Rutina)
            .ToListAsync(ct);

        var socioIds = turnos.Select(t => t.Suscripcion.SocioId).Distinct().ToList();
        var turnoIds = turnos.Select(t => t.TurnoPlantillaId).Distinct().ToList();

        var checkins = await _db.Checkins
            .Where(c => socioIds.Contains(c.SocioId) && turnoIds.Contains(c.TurnoPlantillaId))
            .ToListAsync(ct);

        var result = turnos.Select(t =>
        {
            var cupoTotal = t.TurnoPlantilla.Sala?.Cupo ?? 0;
            var asignados = turnos.Count(x => x.TurnoPlantillaId == t.TurnoPlantillaId);
            var cupoDisp = Math.Max(0, cupoTotal - asignados);

            return new
            {
                id = t.Id,
                suscripcion = new
                {
                    id = t.SuscripcionId,
                    socio = t.Suscripcion.Socio == null
                        ? null
                        : new { id = t.Suscripcion.Socio.Id, nombre = t.Suscripcion.Socio.Nombre }
                },
                turnoPlantillaId = t.TurnoPlantillaId,
                turnoPlantilla = new
                {
                    id = t.TurnoPlantillaId,
                    horaInicio = t.TurnoPlantilla.HoraInicio.ToString(@"hh\:mm"),
                    duracionMin = t.TurnoPlantilla.DuracionMin,
                    diaSemana = t.TurnoPlantilla.DiaSemana == null
                        ? null
                        : new { nombre = t.TurnoPlantilla.DiaSemana.Nombre },
                    sala = t.TurnoPlantilla.Sala == null
                        ? null
                        : new
                        {
                            nombre = t.TurnoPlantilla.Sala.Nombre,
                            cupoTotal = cupoTotal,
                            cupoDisponible = cupoDisp
                        },
                    personal = t.TurnoPlantilla.Personal == null
                        ? null
                        : new { nombre = t.TurnoPlantilla.Personal.Nombre }
                },
                rutina = t.Rutina == null ? null : new { t.Rutina.Id, t.Rutina.Nombre },
                checkinHecho = checkins.Any(c => c.SocioId == t.Suscripcion.SocioId && c.TurnoPlantillaId == t.TurnoPlantillaId)
            };
        });

        return Ok(result);
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPost("suscripcionturno")]
    public async Task<IActionResult> CrearSuscripcionTurno([FromBody] SuscripcionTurnoCreateRequest request, CancellationToken ct)
    {
        var existe = await _db.SuscripcionTurnos.AnyAsync(st =>
            st.SuscripcionId == request.SuscripcionId &&
            st.TurnoPlantillaId == request.TurnoPlantillaId, ct);

        if (existe)
            return Conflict(new { message = "El turno ya fue asignado a esta suscripción." });

        var turno = await _db.TurnosPlantilla.Include(t => t.Sala)
            .FirstOrDefaultAsync(t => t.Id == request.TurnoPlantillaId, ct);
        if (turno == null)
            return NotFound(new { message = "Turno no encontrado." });

        // Validar cupo
        var asignados = await _db.SuscripcionTurnos
            .Include(st => st.Suscripcion)
            .CountAsync(st =>
                st.TurnoPlantillaId == request.TurnoPlantillaId &&
                st.Suscripcion.Estado &&
                st.Suscripcion.Fin >= DateTime.UtcNow.Date, ct);

        if (turno.Sala != null && asignados >= turno.Sala.Cupo)
            return Conflict(new { message = "No hay cupo disponible para este turno." });

        var entity = new SuscripcionTurno
        {
            SuscripcionId = request.SuscripcionId,
            TurnoPlantillaId = request.TurnoPlantillaId,
            RutinaId = request.RutinaId
        };

        _db.SuscripcionTurnos.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Turno asignado correctamente.", id = entity.Id });
    }

    public class ReagendarTurnoRequest
    {
        public int SuscripcionId { get; set; }
        public int TurnoActualId { get; set; }
        public int NuevoTurnoId { get; set; }
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPost("suscripcionturno/reagendar")]
    public async Task<IActionResult> Reagendar([FromBody] ReagendarTurnoRequest payload, CancellationToken ct)
    {
        int suscripcionId = payload.SuscripcionId;
        int turnoActualId = payload.TurnoActualId;
        int nuevoTurnoId = payload.NuevoTurnoId;

        var turno = await _db.SuscripcionTurnos.FirstOrDefaultAsync(st =>
            st.Id == turnoActualId && st.SuscripcionId == suscripcionId, ct);
        if (turno == null)
            return NotFound(new { message = "Asignación de turno no encontrada." });

        if (await _db.SuscripcionTurnos.AnyAsync(st =>
            st.SuscripcionId == suscripcionId && st.TurnoPlantillaId == nuevoTurnoId, ct))
            return Conflict(new { message = "La suscripción ya posee ese turno." });

        var nuevoTurno = await _db.TurnosPlantilla.Include(t => t.Sala)
            .FirstOrDefaultAsync(t => t.Id == nuevoTurnoId, ct);
        if (nuevoTurno == null)
            return NotFound(new { message = "Turno destino no encontrado." });

        var asignados = await _db.SuscripcionTurnos
            .Include(st => st.Suscripcion)
            .CountAsync(st =>
                st.TurnoPlantillaId == nuevoTurnoId &&
                st.Suscripcion.Estado &&
                st.Suscripcion.Fin >= DateTime.UtcNow.Date, ct);

        if (nuevoTurno.Sala != null && asignados >= nuevoTurno.Sala.Cupo)
            return Conflict(new { message = "No hay cupo disponible en el nuevo turno." });

        turno.TurnoPlantillaId = nuevoTurnoId;
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Turno reagendado correctamente." });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPatch("suscripcionturno/{id:int}/rutina")]
    public async Task<IActionResult> AsignarRutina(int id, [FromBody] int rutinaId, CancellationToken ct)
    {
        var turno = await _db.SuscripcionTurnos.FindAsync(new object[] { id }, ct);
        if (turno == null)
            return NotFound(new { message = "Turno no encontrado." });

        var rutina = await _db.RutinasPlantilla.FindAsync(new object[] { rutinaId }, ct);
        if (rutina == null)
            return NotFound(new { message = "Rutina no encontrada." });

        turno.RutinaId = rutinaId;
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Rutina asignada correctamente." });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpDelete("SuscripcionTurno/{id:int}")]
    public async Task<IActionResult> DeleteSuscripcionTurno(int id, CancellationToken ct)
    {
        var turno = await _db.SuscripcionTurnos.FindAsync(new object[] { id }, ct);
        if (turno == null)
            return NotFound(new { message = "Registro no encontrado." });

        _db.SuscripcionTurnos.Remove(turno);
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Turno eliminado correctamente." });
    }
}
