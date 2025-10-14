using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvataresController : ControllerBase
    {
        private readonly IAvatarRepository _repo;

        public AvataresController(IAvatarRepository repo)
        {
            _repo = repo;
        }

        // ✅ GET: api/avatares
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Avatar>>> GetAll(CancellationToken ct)
        {
            var avatares = await _repo.GetAllAsync(ct);
            return Ok(avatares);
        }

        // ✅ GET: api/avatares/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Avatar>> GetById(int id, CancellationToken ct)
        {
            var avatar = await _repo.GetByIdAsync(id, ct);
            if (avatar == null)
                return NotFound($"No se encontró el avatar con ID {id}");

            return Ok(avatar);
        }

        // ✅ POST: api/avatares
        [HttpPost]
        public async Task<ActionResult<Avatar>> Create([FromBody] Avatar avatar, CancellationToken ct)
        {
            var created = await _repo.AddAsync(avatar, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ✅ PUT: api/avatares/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Avatar avatar, CancellationToken ct)
        {
            if (id != avatar.Id)
                return BadRequest("El ID del cuerpo no coincide con el de la URL.");

            var ok = await _repo.UpdateAsync(avatar, ct);
            if (!ok)
                return NotFound($"No se pudo actualizar el avatar con ID {id}");

            return NoContent();
        }

        // ✅ DELETE: api/avatares/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _repo.DeleteAsync(id, ct);
            if (!ok)
                return NotFound($"No se encontró el avatar con ID {id}");

            return NoContent();
        }
    }
}
