using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class GrupoMuscularController : ControllerBase
    {
        private readonly GymDbContext _context;
        private readonly IGrupoMuscularRepository _repo;

        public GrupoMuscularController(GymDbContext context, IGrupoMuscularRepository repo)
        {
            _context = context;
            _repo = repo;
        }

        // 🔹 GET: api/grupomuscular
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var grupos = await _context.GruposMusculares
                .OrderBy(g => g.Nombre)
                .Select(g => new
                {
                    g.Id,
                    g.Nombre
                })
                .ToListAsync(ct);

            return Ok(grupos);
        }

        // 🔹 GET: api/grupomuscular/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var grupo = await _context.GruposMusculares.FindAsync(new object[] { id }, ct);
            if (grupo == null)
                return NotFound(new { message = "Grupo muscular no encontrado" });

            return Ok(new
            {
                grupo.Id,
                grupo.Nombre
            });
        }

        // 🔹 POST: api/grupomuscular
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GrupoMuscular model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repo.AddAsync(model, ct);
            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        // 🔹 PUT: api/grupomuscular/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] GrupoMuscular model, CancellationToken ct)
        {
            if (id != model.Id)
                return BadRequest(new { message = "El ID no coincide con la URL" });

            var updated = await _repo.UpdateAsync(id, model, ct);
            if (updated == null)
                return NotFound(new { message = "Grupo muscular no encontrado" });

            return Ok(updated);
        }

        // 🔹 DELETE: api/grupomuscular/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _repo.DeleteAsync(id, ct);
            if (!deleted)
                return NotFound(new { message = "Grupo muscular no encontrado" });

            return NoContent();
        }
    }
}
