using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/ejercicios")]
    [Authorize(Roles = "Administrador, Profesor")]
    public class EjerciciosController : ControllerBase
    {
        private readonly IEjercicioRepository _repo;
        private readonly GymDbContext _context;

        public EjerciciosController(IEjercicioRepository repo, GymDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        // ===============================
        // 🔹 GET: api/ejercicios?page=1&pageSize=10&q=press
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? q = null)
        {
            var query = _context.Ejercicios
                .Include(e => e.GrupoMuscular)
                .AsQueryable();

            // 🔍 Búsqueda por nombre o grupo muscular
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(e =>
                    e.Nombre.ToLower().Contains(term) ||
                    e.GrupoMuscular.Nombre.ToLower().Contains(term));
            }

            var totalItems = await query.CountAsync();

            // 📄 Paginación
            var ejercicios = await query
                .OrderBy(e => e.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            // 🔹 DTO directo
            var items = ejercicios.Select(e => new
            {
                e.Id,
                e.Nombre,
                e.Tips,
                e.MediaUrl,
                e.GrupoMuscularId,
                GrupoMuscularNombre = e.GrupoMuscular != null ? e.GrupoMuscular.Nombre : null
            });

            return Ok(new { totalItems, items });
        }

        // ===============================
        // 🔹 GET: api/ejercicios/5
        // ===============================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var item = await _context.Ejercicios
                .Include(e => e.GrupoMuscular)
                .FirstOrDefaultAsync(e => e.Id == id, ct);

            if (item == null)
                return NotFound(new { message = "Ejercicio no encontrado." });

            var dto = new
            {
                item.Id,
                item.Nombre,
                item.Tips,
                item.MediaUrl,
                item.GrupoMuscularId,
                GrupoMuscularNombre = item.GrupoMuscular?.Nombre
            };

            return Ok(dto);
        }

        // ===============================
        // 🔹 POST: api/ejercicios
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Ejercicio model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repo.AddAsync(model, ct);

            var dto = new
            {
                model.Id,
                model.Nombre,
                model.Tips,
                model.MediaUrl,
                model.GrupoMuscularId
            };

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, dto);
        }

        // ===============================
        // 🔹 PUT: api/ejercicios/5
        // ===============================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Ejercicio model, CancellationToken ct)
        {
            if (id != model.Id)
                return BadRequest(new { message = "El ID no coincide con la URL." });

            var updated = await _repo.UpdateAsync(id, model, ct);
            if (updated == null)
                return NotFound(new { message = "Ejercicio no encontrado." });

            var dto = new
            {
                updated.Id,
                updated.Nombre,
                updated.Tips,
                updated.MediaUrl,
                updated.GrupoMuscularId
            };

            return Ok(dto);
        }

        // ===============================
        // 🔹 DELETE: api/ejercicios/5
        // ===============================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _repo.DeleteAsync(id, ct);
            if (!deleted)
                return NotFound(new { message = "Ejercicio no encontrado." });

            return NoContent();
        }
    }
}
