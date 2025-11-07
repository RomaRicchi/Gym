using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/rutinasplantilla")]
    [Authorize(Roles = "Administrador, Profesor")]
    public class RutinasPlantillaController : ControllerBase
    {
        private readonly IRutinaPlantillaRepository _repo;
        private readonly GymDbContext _context;
        private readonly IWebHostEnvironment _env;

        public RutinasPlantillaController(IRutinaPlantillaRepository repo, GymDbContext context, IWebHostEnvironment env)
        {
            _repo = repo;
            _context = context;
            _env = env;
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

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(r =>
                    r.Nombre.ToLower().Contains(term) ||
                    (r.Objetivo != null && r.Objetivo.ToLower().Contains(term)) ||
                    (r.GrupoMuscular != null && r.GrupoMuscular.Nombre.ToLower().Contains(term)));
            }

            var totalItems = await query.CountAsync(ct);

            var rutinas = await query
                .OrderBy(r => r.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(ct);

            var items = rutinas.Select(r => new
            {
                r.Id,
                r.Nombre,
                r.Objetivo,
                r.GrupoMuscularId,
                GrupoMuscularNombre = r.GrupoMuscular?.Nombre,
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
        [RequestSizeLimit(10_000_000)] // hasta 10 MB
        public async Task<IActionResult> Create([FromForm] RutinaPlantilla model, IFormFile? image, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 📸 Subir imagen si existe
            if (image != null)
            {
                var uploadsDir = Path.Combine(_env.ContentRootPath, "Uploads", "Rutinas");
                Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsDir, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream, ct);

                model.ImagenUrl = Path.Combine("Uploads", "Rutinas", fileName).Replace("\\", "/");
            }

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
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> Update(int id, [FromForm] RutinaPlantilla model, IFormFile? image, CancellationToken ct)
        {
            if (id != model.Id)
                return BadRequest(new { message = "El ID no coincide con la URL." });

            var existing = await _context.RutinasPlantilla.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Rutina no encontrada." });

            existing.Nombre = model.Nombre;
            existing.Objetivo = model.Objetivo;
            existing.GrupoMuscularId = model.GrupoMuscularId;

            if (image != null)
            {
                var uploadsDir = Path.Combine(_env.ContentRootPath, "Uploads", "Rutinas");
                Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsDir, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream, ct);

                existing.ImagenUrl = Path.Combine("Uploads", "Rutinas", fileName).Replace("\\", "/");
            }

            await _context.SaveChangesAsync(ct);

            var dto = new
            {
                existing.Id,
                existing.Nombre,
                existing.Objetivo,
                existing.GrupoMuscularId,
                existing.ImagenUrl
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

        // ===============================
        // 🔹 POST: api/rutinasplantilla/upload
        // ===============================
        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Debe seleccionar un archivo válido.");

            var uploadsDir = Path.Combine(_env.ContentRootPath, "Uploads", "Rutinas");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = Path.Combine("Uploads", "Rutinas", fileName).Replace("\\", "/");
            return Ok(new { imageUrl = relativePath });
        }
    }
}
