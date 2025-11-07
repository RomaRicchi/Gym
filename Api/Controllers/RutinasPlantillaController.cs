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
        // 🔹 GET paginado
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

            var items = await query
                .OrderBy(r => r.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new
                {
                    r.Id,
                    r.Nombre,
                    r.Objetivo,
                    r.ImagenUrl,
                    GrupoMuscularNombre = r.GrupoMuscular != null ? r.GrupoMuscular.Nombre : null
                })
                .ToListAsync(ct);

            return Ok(new { totalItems, items });
        }

        // ===============================
        // 🔹 GET por ID
        // ===============================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var rutina = await _context.RutinasPlantilla
                .Include(r => r.GrupoMuscular)
                .Include(r => r.RutinaPlantillaEjercicios)
                    .ThenInclude(re => re.Ejercicio)
                .FirstOrDefaultAsync(r => r.Id == id, ct);

            if (rutina == null)
                return NotFound();

            return Ok(rutina);
        }

        // ===============================
        // 🔹 POST (crear nueva rutina)
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RutinaPlantilla model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.RutinasPlantilla.Add(model);
            await _context.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        // ===============================
        // 🔹 PUT (actualizar existente)
        // ===============================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RutinaPlantilla model, CancellationToken ct)
        {
            var existing = await _context.RutinasPlantilla.FindAsync(new object[] { id }, ct);
            if (existing == null)
                return NotFound();

            existing.Nombre = model.Nombre;
            existing.Objetivo = model.Objetivo;
            existing.GrupoMuscularId = model.GrupoMuscularId;
            existing.ImagenUrl = model.ImagenUrl;

            await _context.SaveChangesAsync(ct);
            return Ok(existing);
        }

        // ===============================
        // 🔹 DELETE
        // ===============================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var existing = await _context.RutinasPlantilla.FindAsync(new object[] { id }, ct);
            if (existing == null)
                return NotFound();

            _context.RutinasPlantilla.Remove(existing);
            await _context.SaveChangesAsync(ct);
            return NoContent();
        }

        // ===============================
        // 🔹 POST /api/rutinasplantilla/upload (subida de imagen)
        // ===============================
        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)] // 10 MB
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo no válido.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", "Rutinas");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var relativePath = Path.Combine("Uploads", "Rutinas", fileName).Replace("\\", "/");
            return Ok(new { url = relativePath });
        }
    }
}
