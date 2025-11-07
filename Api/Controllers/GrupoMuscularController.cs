using Api.Data.Models;
using Api.Dtos;
using Api.Mappers;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GrupoMuscularController : ControllerBase
    {
        private readonly IGrupoMuscularRepository _repo;

        public GrupoMuscularController(IGrupoMuscularRepository repo)
        {
            _repo = repo;
        }

        // ===============================
        //  GET: api/grupomuscular?page=1&pageSize=50&q=pecho
        // ===============================
        [HttpGet]
        public async Task<ActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? q = null,
            CancellationToken ct = default)
        {
            var (items, totalItems) = await _repo.GetPagedAsync(page, pageSize, q, ct);
            var dtos = items.Select(g => g.ToDto());
            return Ok(new { totalItems, items = dtos });
        }

        // ===============================
        // GET: api/grupomuscular/5
        // ===============================
        [HttpGet("{id}")]
        public async Task<ActionResult<GrupoMuscularDto>> GetById(int id, CancellationToken ct)
        {
            var grupo = await _repo.GetByIdAsync(id, ct);
            if (grupo == null)
                return NotFound(new { message = "Grupo muscular no encontrado." });

            return Ok(grupo.ToDto());
        }

        // ===============================
        // POST: api/grupomuscular
        // ===============================
        [HttpPost]
        public async Task<ActionResult<GrupoMuscularDto>> Create(
            [FromBody] GrupoMuscularDto dto,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = dto.ToEntity();
            await _repo.AddAsync(entity, ct);

            return CreatedAtAction(nameof(GetById),
                new { id = entity.Id },
                entity.ToDto());
        }

        // ===============================
        // PUT: api/grupomuscular/5
        // ===============================
        [HttpPut("{id}")]
        public async Task<ActionResult<GrupoMuscularDto>> Update(
            int id,
            [FromBody] GrupoMuscularDto dto,
            CancellationToken ct)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "El ID del cuerpo no coincide con la URL." });

            var updated = await _repo.UpdateAsync(id, dto.ToEntity(), ct);
            if (updated == null)
                return NotFound(new { message = "Grupo muscular no encontrado." });

            return Ok(updated.ToDto());
        }

        // ===============================
        // DELETE: api/grupomuscular/5
        // ===============================
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _repo.DeleteAsync(id, ct);
            if (!deleted)
                return NotFound(new { message = "Grupo muscular no encontrado." });

            return NoContent();
        }
    }
}
