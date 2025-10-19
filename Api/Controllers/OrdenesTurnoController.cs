using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepcionista")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenesTurnoController : ControllerBase
    {
        private readonly IOrdenTurnoRepository _repo;

        public OrdenesTurnoController(IOrdenTurnoRepository repo)
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

        [HttpGet("orden/{id}")]
        public async Task<IActionResult> GetByOrdenPago(int id, CancellationToken ct = default)
        {
            var list = await _repo.GetByOrdenPagoAsync((int)id, ct);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] OrdenTurno dto, CancellationToken ct = default)
        {
            var nuevo = await _repo.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] OrdenTurno dto, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null) return NotFound();

            entity.OrdenPagoId = dto.OrdenPagoId;
            entity.TurnoPlantillaId = dto.TurnoPlantillaId;
            entity.Activo = dto.Activo;
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
