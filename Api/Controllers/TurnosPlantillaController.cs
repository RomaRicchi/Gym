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
            var turnos = await _repo.GetByDiaAsync(id, ct);
            if (turnos == null || !turnos.Any())
                return Ok(new { ok = true, items = new List<object>() }); // 🔸 devuelve array vacío

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

            // Validar solapamiento
            var existe = await _repo.ExisteSolapamientoAsync(
                dto.SalaId, dto.DiaSemanaId, dto.HoraInicio, dto.DuracionMin, ct);

            if (existe)
                return BadRequest(new { message = "Ya existe un turno que se solapa en ese horario." });

            turnoExistente.SalaId = dto.SalaId;
            turnoExistente.PersonalId = dto.PersonalId;
            turnoExistente.DiaSemanaId = dto.DiaSemanaId;
            turnoExistente.HoraInicio = dto.HoraInicio;
            turnoExistente.DuracionMin = dto.DuracionMin;
            turnoExistente.Activo = dto.Activo;

            await _repo.UpdateAsync(turnoExistente, ct);
            return Ok(new { ok = true, message = "Turno actualizado correctamente." });
        }

        // Eliminar turno
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            var eliminado = await _repo.DeleteAsync(id, ct);
            if (!eliminado)
                return NotFound(new { message = "Turno no encontrado." });

            return Ok(new { ok = true, message = "Turno eliminado correctamente." });
        }
    }
}
