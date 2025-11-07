using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador, Profesor")]
    public class EjerciciosController : ControllerBase
    {
        private readonly GymDbContext _db;
        private readonly IWebHostEnvironment _env;

        public EjerciciosController(GymDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // === GET: api/ejercicios ===
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? q = null)
        {
            var query = _db.Ejercicios
                .Include(e => e.GrupoMuscular)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(e =>
                    e.Nombre.ToLower().Contains(term) ||
                    (e.GrupoMuscular != null && e.GrupoMuscular.Nombre.ToLower().Contains(term)));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return Ok(new { items, totalItems });
        }

        // === GET: api/ejercicios/5 ===
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _db.Ejercicios
                .Include(e => e.GrupoMuscular)
                .FirstOrDefaultAsync(e => e.Id == id);

            return item == null ? NotFound() : Ok(item);
        }

        // === POST: api/ejercicios ===
        [HttpPost]
        [RequestSizeLimit(10_000_000)] // 10 MB máx
        public async Task<IActionResult> Create([FromForm] Ejercicio model, IFormFile? image)
        {
            if (image != null)
            {
                var uploadsDir = Path.Combine(_env.ContentRootPath, "Uploads", "Ejercicios");
                Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsDir, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);

                model.MediaUrl = Path.Combine("Uploads", "Ejercicios", fileName).Replace("\\", "/");
            }

            _db.Ejercicios.Add(model);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        // === PUT: api/ejercicios/5 ===
        [HttpPut("{id:int}")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> Update(int id, [FromForm] Ejercicio model, IFormFile? image)
        {
            var existing = await _db.Ejercicios.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Nombre = model.Nombre;
            existing.Tips = model.Tips;
            existing.GrupoMuscularId = model.GrupoMuscularId;

            if (image != null)
            {
                var uploadsDir = Path.Combine(_env.ContentRootPath, "Uploads", "Ejercicios");
                Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsDir, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);

                existing.MediaUrl = Path.Combine("Uploads", "Ejercicios", fileName).Replace("\\", "/");
            }

            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        // === DELETE: api/ejercicios/5 ===
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Ejercicios.FindAsync(id);
            if (entity == null)
                return NotFound();

            _db.Ejercicios.Remove(entity);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // === POST: api/ejercicios/upload ===
        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Debe seleccionar un archivo válido.");

            var uploadsDir = Path.Combine(_env.ContentRootPath, "Uploads", "Ejercicios");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = Path.Combine("Uploads", "Ejercicios", fileName).Replace("\\", "/");
            return Ok(new { imageUrl = relativePath });
        }
    }
}
