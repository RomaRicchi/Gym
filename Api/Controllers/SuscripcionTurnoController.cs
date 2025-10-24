using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepcionista")]
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

        // 🔹 GET: api/SuscripcionTurno
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return Ok(list);
        }

        // 🔹 GET: api/SuscripcionTurno/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null)
                return NotFound(new { message = $"No se encontró el turno asignado con ID {id}" });

            return Ok(entity);
        }

        // 🔹 GET: api/SuscripcionTurno/suscripcion/{id}
        [HttpGet("suscripcion/{id:int}")]
        public async Task<IActionResult> GetBySuscripcion(int id, CancellationToken ct = default)
        {
            var list = await _repo.GetBySuscripcionAsync(id, ct);
            return Ok(list);
        }

        // 🔹 GET: api/SuscripcionTurno/socio/{id}
        [HttpGet("socio/{id:int}")]
        public async Task<IActionResult> GetBySocio(int id, CancellationToken ct = default)
        {
            var list = await _repo.GetBySocioAsync(id, ct);
            return Ok(list);
        }

        // 🔹 POST: api/SuscripcionTurno
        [HttpPost]
public async Task<IActionResult> Crear([FromBody] SuscripcionTurnoCreateDto dto, CancellationToken ct = default)
{
    if (dto == null)
        return BadRequest(new { message = "El cuerpo de la solicitud está vacío." });

    // Validaciones de existencia
    var suscripcionExiste = await _db.Suscripciones.AnyAsync(s => s.Id == dto.SuscripcionId, ct);
    if (!suscripcionExiste)
        return BadRequest(new { message = "❌ La suscripción especificada no existe." });

    var turnoExiste = await _db.TurnosPlantilla.AnyAsync(t => t.Id == dto.TurnoPlantillaId, ct);
    if (!turnoExiste)
        return BadRequest(new { message = "❌ El turno seleccionado no existe." });

    // Duplicado
    bool yaAsignado = await _db.SuscripcionTurnos
        .AnyAsync(st => st.SuscripcionId == dto.SuscripcionId && st.TurnoPlantillaId == dto.TurnoPlantillaId, ct);
    if (yaAsignado)
        return Conflict(new { message = "⚠️ Este turno ya fue asignado a esta suscripción." });

    // Crear entidad nueva
    var nuevo = new SuscripcionTurno
    {
        SuscripcionId = dto.SuscripcionId,
        TurnoPlantillaId = dto.TurnoPlantillaId
    };

    _db.SuscripcionTurnos.Add(nuevo);
    await _db.SaveChangesAsync(ct);

    return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
}

        // 🔹 PUT: api/SuscripcionTurno/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] SuscripcionTurno dto, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null)
                return NotFound(new { message = $"No se encontró el turno asignado con ID {id}" });

            // Validar existencia de nuevos IDs si se modifican
            if (!await _db.Suscripciones.AnyAsync(s => s.Id == dto.SuscripcionId, ct))
                return BadRequest(new { message = "La suscripción no existe." });

            if (!await _db.TurnosPlantilla.AnyAsync(t => t.Id == dto.TurnoPlantillaId, ct))
                return BadRequest(new { message = "El turno no existe." });

            entity.SuscripcionId = dto.SuscripcionId;
            entity.TurnoPlantillaId = dto.TurnoPlantillaId;

            await _repo.UpdateAsync(entity, ct);
            return Ok(new { ok = true, message = "✅ Turno actualizado correctamente." });
        }

        // 🔹 DELETE: api/SuscripcionTurno/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null)
                return NotFound(new { message = $"No se encontró el turno con ID {id}" });

            await _repo.DeleteAsync(id, ct);
            return Ok(new { ok = true, message = "🗑️ Turno eliminado correctamente." });
        }

        [HttpGet("con-checkin")]
        public async Task<IActionResult> GetAllConCheckin(CancellationToken ct = default)
        {
            var result = await _repo.GetAllWithCheckinAsync(ct);
            return Ok(new { ok = true, items = result });
        }

    }
}
