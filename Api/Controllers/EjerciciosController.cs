using Api.Data; 
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? q = null)
        {
            var query = _context.Ejercicios.AsQueryable();

            // Filtro por nombre o grupo
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(e =>
                    e.Nombre.ToLower().Contains(term) ||
                    e.Grupo.ToLower().Contains(term));
            }

            // Total antes del paginado
            var totalItems = await query.CountAsync();

            // Paginación
            var ejercicios = await query
                .OrderBy(e => e.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Respuesta esperada por el frontend
            return Ok(new
            {
                items = ejercicios,
                totalItems
            });
        }


        // GET: api/ejercicios/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var item = await _repo.GetByIdAsync(id, ct);
            return item == null ? NotFound() : Ok(item);
        }

        // POST: api/ejercicios
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Ejercicio model, CancellationToken ct)
        {
            var created = await _repo.AddAsync(model, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/ejercicios/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Ejercicio model, CancellationToken ct)
        {
            var updated = await _repo.UpdateAsync(id, model, ct);
            return updated == null ? NotFound() : Ok(updated);
        }

        // DELETE: api/ejercicios/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _repo.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
