using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/ejercicios")]
    [Authorize(Roles = "Administrador, Profesor")]
    public class EjerciciosController : ControllerBase
    {
        private readonly IEjercicioRepository _repo;
        private readonly GymDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EjerciciosController(IEjercicioRepository repo, GymDbContext context, IWebHostEnvironment env)
        {
            _repo = repo;
            _context = context;
            _env = env;
        }

        // ===============================
        // 🔹 GET paginado
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

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(e =>
                    e.Nombre.ToLower().Contains(term) ||
                    e.Tips.ToLower().Contains(term) ||
                    (e.GrupoMuscular != null && e.GrupoMuscular.Nombre.ToLower().Contains(term)));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new
                {
                    e.Id,
                    e.Nombre,
                    e.Tips,
                    e.MediaUrl,
                    GrupoMuscularNombre = e.GrupoMuscular != null ? e.GrupoMuscular.Nombre : null
                })
                .ToListAsync();

            return Ok(new { items, totalItems });
        }

        // ===============================
        // 🔹 GET por ID
        // ===============================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var e = await _context.Ejercicios
                .Include(g => g.GrupoMuscular)
                .FirstOrDefaultAsync(e => e.Id == id, ct);

            if (e == null)
                return NotFound();

            return Ok(e);
        }

        // ===============================
        // 🔹 POST (crear nuevo)
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Ejercicio model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Ejercicios.Add(model);
            await _context.SaveChangesAsync(ct);
            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        // ===============================
        // 🔹 PUT (actualizar existente)
        // ===============================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Ejercicio model, CancellationToken ct)
        {
            var existing = await _context.Ejercicios.FindAsync(new object[] { id }, ct);
            if (existing == null)
                return NotFound();

            existing.Nombre = model.Nombre;
            existing.Tips = model.Tips;
            existing.GrupoMuscularId = model.GrupoMuscularId;
            existing.MediaUrl = model.MediaUrl;

            await _context.SaveChangesAsync(ct);
            return Ok(existing);
        }

        // ===============================
        // 🔹 DELETE
        // ===============================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var existing = await _context.Ejercicios.FindAsync(new object[] { id }, ct);
            if (existing == null)
                return NotFound();

            _context.Ejercicios.Remove(existing);
            await _context.SaveChangesAsync(ct);
            return NoContent();
        }

        // ===============================
        // 🔹 POST: api/ejercicios/upload
        // ===============================
        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)] // 10 MB
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo no válido.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", "Ejercicios");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var relativePath = Path.Combine("Uploads", "Ejercicios", fileName).Replace("\\", "/");
            return Ok(new { url = relativePath });
        }

    }
}
