using Api.Data;
using Api.Data.Models;
using Api.Contracts;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepción, Socio")]
    [ApiController]
    [Route("api/turnosplantilla")]
    public class TurnosPlantillaController : ControllerBase
    {
        private readonly ITurnoPlantillaRepository _repo;
        private readonly GymDbContext _db;

        public TurnosPlantillaController(ITurnoPlantillaRepository repo, GymDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        // Obtener todos
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return Ok(list);
        }

        // Obtener activos
        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos(CancellationToken ct = default)
        {
            // Precalcular inscripciones por sala y hora
            var inscripciones = await _db.SuscripcionTurnos
                .Include(st => st.TurnoPlantilla)
                .GroupBy(st => new
                {
                    st.TurnoPlantilla!.SalaId,
                    st.TurnoPlantilla.HoraInicio
                })
                .Select(g => new
                {
                    g.Key.SalaId,
                    g.Key.HoraInicio,
                    Cantidad = g.Count()
                })
                .ToListAsync(ct);

            var mapaInscriptos = inscripciones.ToDictionary(
                x => new { x.SalaId, x.HoraInicio },
                x => x.Cantidad
            );

            // 2Traer los turnos activos con relaciones
            var turnosDb = await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .Where(t => t.Activo)
                .OrderBy(t => t.DiaSemanaId)
                .ThenBy(t => t.HoraInicio)
                .ToListAsync(ct);

            //  Calcular cupos en memoria (rápido)
            var turnos = turnosDb.Select(t => new
            {
                t.Id,
                t.HoraInicio,
                t.DuracionMin,
                DiaSemana = new
                {
                    Id = t.DiaSemana?.Id ?? 0,
                    Nombre = t.DiaSemana?.Nombre ?? "(sin día)"
                },
                Sala = new
                {
                    Id = t.Sala?.Id ?? 0,
                    Nombre = t.Sala?.Nombre ?? "(sin sala)",
                    CupoTotal = t.Sala?.Cupo ?? 0,
                    CupoDisponible = (t.Sala?.Cupo ?? 0) -
                        (mapaInscriptos.TryGetValue(new { t.SalaId, t.HoraInicio }, out var cantidad)
                            ? cantidad
                            : 0)
                },
                Personal = new
                {
                    Id = t.Personal?.Id ?? 0,
                    Nombre = t.Personal?.Nombre ?? "(sin profesor)"
                },
                t.Activo
            }).ToList();

            return Ok(new { ok = true, items = turnos });
        }


        // Obtener por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var turno = await _repo.GetByIdAsync(id, ct);
            if (turno is null)
                return NotFound(new { message = "No se encontró el turno solicitado." });

            return Ok(turno);
        }

        
        [HttpGet("dia/{id:int}")]
        public async Task<IActionResult> GetByDia(int id, CancellationToken ct = default)
        {
            // 1️⃣ Precalcular inscripciones agrupadas por sala y hora
            var inscripciones = await _db.SuscripcionTurnos
                .Include(st => st.TurnoPlantilla)
                .GroupBy(st => new
                {
                    st.TurnoPlantilla!.SalaId,
                    st.TurnoPlantilla.HoraInicio
                })
                .Select(g => new
                {
                    g.Key.SalaId,
                    g.Key.HoraInicio,
                    Cantidad = g.Count()
                })
                .ToListAsync(ct);

            // 2️⃣ Crear diccionario
            var mapaInscriptos = inscripciones.ToDictionary(
                x => new { x.SalaId, x.HoraInicio },
                x => x.Cantidad
            );

            // 3️⃣ Obtener los turnos del día (en memoria)
            var turnosDb = await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .Where(t => t.Activo && t.DiaSemanaId == id)
                .OrderBy(t => t.HoraInicio)
                .ToListAsync(ct);

            // 4️⃣ Calcular cupos en memoria
            var turnos = turnosDb.Select(t => new
            {
                t.Id,
                t.HoraInicio,
                t.DuracionMin,
                Dia = new
                {
                    Id = t.DiaSemana?.Id ?? 0,
                    Nombre = t.DiaSemana?.Nombre ?? "(sin día)"
                },
                Sala = new
                {
                    Id = t.Sala?.Id ?? 0,
                    Nombre = t.Sala?.Nombre ?? "(sin sala)",
                    CupoTotal = t.Sala?.Cupo ?? 0,
                    CupoDisponible = (t.Sala?.Cupo ?? 0) -
                        (mapaInscriptos.TryGetValue(new { t.SalaId, t.HoraInicio }, out var cantidad)
                            ? cantidad
                            : 0)
                },
                Profesor = t.Personal?.Nombre ?? "(sin profesor)"
            }).ToList();

            // 5️⃣ Devolver resultado
            return Ok(new { ok = true, items = turnos });
        }



        // Crear nuevo turno plantilla
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] TurnoPlantillaCreateDto dto, CancellationToken ct)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "El cuerpo de la solicitud está vacío." });

                // Validación de solapamiento
                var existe = await _repo.ExisteSolapamientoAsync(
                    dto.SalaId, dto.DiaSemanaId, dto.HoraInicio, dto.DuracionMin, ct);

                if (existe)
                    return BadRequest(new { message = "Ya existe un turno que se solapa en ese horario." });

                var nuevo = new TurnoPlantilla
                {
                    SalaId = dto.SalaId,
                    PersonalId = dto.PersonalId,
                    DiaSemanaId = dto.DiaSemanaId,
                    HoraInicio = dto.HoraInicio,
                    DuracionMin = dto.DuracionMin,
                    Activo = dto.Activo
                };

                var creado = await _repo.AddAsync(nuevo, ct);
                return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // Actualizar turno existente
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] TurnoPlantilla dto, CancellationToken ct = default)
        {
            if (dto == null)
                return BadRequest(new { message = "El cuerpo de la solicitud está vacío." });

            if (id != dto.Id)
                return BadRequest(new { message = "El ID del turno no coincide con el de la URL." });

            var turnoExistente = await _repo.GetByIdAsync(id, ct);
            if (turnoExistente is null)
                return NotFound(new { message = "Turno no encontrado." });

            // Evitar validar solapamiento si no cambian sala, día u hora
            bool requiereValidacion =
                turnoExistente.SalaId != dto.SalaId ||
                turnoExistente.DiaSemanaId != dto.DiaSemanaId ||
                turnoExistente.HoraInicio != dto.HoraInicio ||
                turnoExistente.DuracionMin != dto.DuracionMin;

            if (requiereValidacion)
            {
                var existe = await _repo.ExisteSolapamientoAsync(
                    dto.SalaId, dto.DiaSemanaId, dto.HoraInicio, dto.DuracionMin, ct);

                if (existe)
                    return BadRequest(new { message = "Ya existe un turno que se solapa en ese horario." });
            }

            // Actualizar datos
            turnoExistente.SalaId = dto.SalaId;
            turnoExistente.PersonalId = dto.PersonalId;
            turnoExistente.DiaSemanaId = dto.DiaSemanaId;
            turnoExistente.HoraInicio = dto.HoraInicio;
            turnoExistente.DuracionMin = dto.DuracionMin;

            await _repo.UpdateAsync(turnoExistente, ct);
            return Ok(new { ok = true, message = "Turno actualizado correctamente." });
}

        // Eliminar turno- Borrado lógico
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            var turno = await _repo.GetByIdAsync(id, ct);
            if (turno is null)
                return NotFound(new { message = "Turno no encontrado." });

            if (!turno.Activo)
                return BadRequest(new { message = "El turno ya se encuentra inactivo." });

            turno.Activo = false;

            await _repo.UpdateAsync(turno, ct);

            return Ok(new
            {
                ok = true,
                message = "Turno desactivado correctamente (borrado lógico)."
            });
        }

    }
}
