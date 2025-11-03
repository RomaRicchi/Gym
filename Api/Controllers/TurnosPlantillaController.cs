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
            var list = await _repo.GetActivosAsync(ct);
            return Ok(list);
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
            var turnos = await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .Where(t => t.Activo && t.DiaSemanaId == id)
                .OrderBy(t => t.HoraInicio)
                .Select(t => new
                {
                    t.Id,
                    t.HoraInicio,
                    t.DuracionMin,
                    Dia = new { t.DiaSemana.Id, t.DiaSemana.Nombre },
                    Sala = new
                    {
                        t.Sala.Id,
                        t.Sala.Nombre,
                        CupoTotal = t.Sala.Cupo,
                        CupoDisponible = t.Sala.Cupo - _db.SuscripcionTurnos.Count(st => st.TurnoPlantillaId == t.Id)
                    },
                    Profesor = t.Personal != null ? t.Personal.Nombre : "(sin profesor)"
                })
                .ToListAsync(ct);

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
