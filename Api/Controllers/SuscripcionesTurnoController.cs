using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepcionista")]
    [ApiController]
    [Route("api/[controller]")]
    public class SuscripcionesTurnoController : ControllerBase
    {
        private readonly ISuscripcionTurnoRepository _repo;

        public SuscripcionesTurnoController(ISuscripcionTurnoRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null) return NotFound();
            return Ok(entity);
        }

        [HttpGet("suscripcion/{id}")]
        public async Task<IActionResult> GetBySuscripcion(int id, CancellationToken ct = default)
        {
            var list = await _repo.GetBySuscripcionAsync(id, ct);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] SuscripcionTurno dto, CancellationToken ct = default)
        {
            var nuevo = await _repo.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] SuscripcionTurno dto, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null) return NotFound();

            entity.SuscripcionId = dto.SuscripcionId;
            entity.TurnoPlantillaId = dto.TurnoPlantillaId;
            

            await _repo.UpdateAsync(entity, ct);
            return Ok(new { ok = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            await _repo.DeleteAsync(id, ct);
            return Ok(new { ok = true });
        }
    }
}
