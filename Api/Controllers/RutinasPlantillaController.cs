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
    [Route("api/rutinasplantilla")]
    [Authorize(Roles = "Administrador, Profesor")]
    public class RutinasPlantillaController : ControllerBase
    {
        private readonly IRutinaPlantillaRepository _repo;
        private readonly GymDbContext _context;

        public RutinasPlantillaController(IRutinaPlantillaRepository repo, GymDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        // ===============================
        // 🔹 GET: api/rutinasplantilla?page=1&pageSize=10&q=pecho
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? q = null,
            CancellationToken ct = default)
        {
            var query = _context.RutinasPlantilla
                .Include(r => r.GrupoMuscular)
                .AsQueryable();

            // 🔍 Filtro por nombre, objetivo o grupo muscular
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(r =>
                    r.Nombre.ToLower().Contains(term) ||
                    (r.Objetivo != null && r.Objetivo.ToLower().Contains(term)) ||
                    r.GrupoMuscular.Nombre.ToLower().Contains(term));
            }

            var totalItems = await query.CountAsync(ct);

            // 📄 Paginación
            var rutinas = await query
                .OrderBy(r => r.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(ct);

            // 🔹 DTO directo
            var items = rutinas.Select(r => new
            {
                r.Id,
                r.Nombre,
                r.Objetivo,
                r.GrupoMuscularId,
                GrupoMuscularNombre = r.GrupoMuscular != null ? r.GrupoMuscular.Nombre : null,
                r.ImagenUrl
            });

            return Ok(new { totalItems, items });
        }

        // ===============================
        // 🔹 GET: api/rutinasplantilla/5
        // ===============================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var item = await _context.RutinasPlantilla
                .Include(r => r.GrupoMuscular)
                .Include(r => r.RutinaPlantillaEjercicios)
                    .ThenInclude(re => re.Ejercicio)
                .FirstOrDefaultAsync(r => r.Id == id, ct);

            if (item == null)
                return NotFound(new { message = "Rutina no encontrada." });

            // 🔹 DTO con ejercicios incluidos
            var dto = new
            {
                item.Id,
                item.Nombre,
                item.Objetivo,
                item.GrupoMuscularId,
                GrupoMuscularNombre = item.GrupoMuscular?.Nombre,
                item.ImagenUrl,
                ejercicios = item.RutinaPlantillaEjercicios.Select(e => new
                {
                    e.EjercicioId,
                    e.Ejercicio.Nombre,
                    e.Series,
                    e.Repeticiones,
                    e.DescansoSeg,
                    e.Orden,
                    e.Ejercicio.MediaUrl
                })
            };

            return Ok(dto);
        }

        // ===============================
        // 🔹 POST: api/rutinasplantilla
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RutinaPlantilla model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repo.AddAsync(model, ct);

            var dto = new
            {
                model.Id,
                model.Nombre,
                model.Objetivo,
                model.GrupoMuscularId,
                model.ImagenUrl
            };

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, dto);
        }

        // ===============================
        // 🔹 PUT: api/rutinasplantilla/5
        // ===============================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RutinaPlantilla model, CancellationToken ct)
        {
            if (id != model.Id)
                return BadRequest(new { message = "El ID no coincide con la URL." });

            var updated = await _repo.UpdateAsync(id, model, ct);
            if (updated == null)
                return NotFound(new { message = "Rutina no encontrada." });

            var dto = new
            {
                updated.Id,
                updated.Nombre,
                updated.Objetivo,
                updated.GrupoMuscularId,
                updated.ImagenUrl
            };

            return Ok(dto);
        }

        // ===============================
        // 🔹 DELETE: api/rutinasplantilla/5
        // ===============================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _repo.DeleteAsync(id, ct);
            if (!deleted)
                return NotFound(new { message = "Rutina no encontrada." });

            return NoContent();
        }
    }
}
