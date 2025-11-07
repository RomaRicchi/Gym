using Api.Data;
using Api.Data.Models;
using Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador, Profesor")]
    public class GrupoMuscularController : ControllerBase
    {
        private readonly GymDbContext _context;

        public GrupoMuscularController(GymDbContext context)
        {
            _context = context;
        }

        // === GET: api/grupomuscular ===
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? q = null)
        {
            var query = _context.GruposMusculares.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(g =>
                    g.Nombre.ToLower().Contains(term) ||
                    (g.Descripcion != null && g.Descripcion.ToLower().Contains(term)));
            }

            var totalItems = await query.CountAsync();

            var grupos = await query
                .OrderBy(g => g.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { items = grupos, totalItems });
        }

        // === GET: api/grupomuscular/5 ===
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var grupo = await _context.GruposMusculares.FindAsync(id);
            return grupo == null ? NotFound() : Ok(grupo);
        }

        // === POST: api/grupomuscular ===
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GrupoMuscularDto dto)
        {
            var entity = new GrupoMuscular
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                ImagenUrl = dto.ImagenUrl
            };

            _context.GruposMusculares.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        // === PUT: api/grupomuscular/5 ===
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] GrupoMuscularDto dto)
        {
            var entity = await _context.GruposMusculares.FindAsync(id);
            if (entity == null)
                return NotFound();

            entity.Nombre = dto.Nombre;
            entity.Descripcion = dto.Descripcion;
            entity.ImagenUrl = dto.ImagenUrl;

            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        // === DELETE: api/grupomuscular/5 ===
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.GruposMusculares.FindAsync(id);
            if (entity == null)
                return NotFound();

            _context.GruposMusculares.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
