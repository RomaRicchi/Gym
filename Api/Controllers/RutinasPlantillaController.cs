using Api.Data;
using Api.Data.Models;
using Api.Contracts;
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

        public RutinasPlantillaController(
            IRutinaPlantillaRepository repo,
            GymDbContext context,
            IWebHostEnvironment env)
        {
            _repo = repo;
            _context = context;
            _env = env;
        }

        // ===============================
        //  GET paginado con búsqueda
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? q = null,
            CancellationToken ct = default)
        {
            // Asegurar límites mínimos
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var queryable = _context.RutinasPlantilla
                .Include(r => r.GrupoMuscular)
                .AsQueryable();

            //  Filtro por texto (nombre, objetivo o grupo muscular)
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLowerInvariant();
                queryable = queryable.Where(r =>
                    r.Nombre.ToLower().Contains(term) ||
                    (r.Objetivo != null && r.Objetivo.ToLower().Contains(term)) ||
                    (r.GrupoMuscular != null && r.GrupoMuscular.Nombre.ToLower().Contains(term)));
            }

            var totalItems = await queryable.CountAsync(ct);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await queryable
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

            return Ok(new
            {
                page,
                pageSize,
                totalItems,
                totalPages,
                items
            });
        }


        // ===============================
        //  GET por ID
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
        //  POST (crear nueva rutina con imagen)
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] RutinaPlantillaDto dto, IFormFile? Imagen, CancellationToken ct)
        {
            var rutina = new RutinaPlantilla
            {
                Nombre = dto.Nombre,
                Objetivo = dto.Objetivo,
                GrupoMuscularId = dto.GrupoMuscularId
            };

            if (Imagen != null && Imagen.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", "Rutinas");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(Imagen.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await Imagen.CopyToAsync(stream, ct);

                rutina.ImagenUrl = Path.Combine("Uploads", "Rutinas", fileName).Replace("\\", "/");
            }

            _context.RutinasPlantilla.Add(rutina);
            await _context.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetById), new { id = rutina.Id }, rutina);
        }


        // ===============================
        // PUT (actualizar rutina existente con o sin imagen)
        // ===============================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] RutinaPlantillaDto dto, IFormFile? imagen, CancellationToken ct)
        {
            var existing = await _context.RutinasPlantilla.FindAsync(new object[] { id }, ct);
            if (existing == null)
                return NotFound();

            existing.Nombre = dto.Nombre;
            existing.Objetivo = dto.Objetivo;
            existing.GrupoMuscularId = dto.GrupoMuscularId;

            if (imagen != null && imagen.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", "Rutinas");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imagen.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await imagen.CopyToAsync(stream, ct);

                existing.ImagenUrl = Path.Combine("Uploads", "Rutinas", fileName).Replace("\\", "/");
            }
            else if (!string.IsNullOrEmpty(dto.ImagenUrl))
            {
                existing.ImagenUrl = dto.ImagenUrl; // mantiene la actual
            }

            await _context.SaveChangesAsync(ct);
            return Ok(existing);
        }

        // ===============================
        // DELETE
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
        // POST: api/rutinasplantilla/upload
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

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = Path.Combine("Uploads", "Rutinas", fileName).Replace("\\", "/");
            return Ok(new { url = relativePath });
        }
        // ===============================
        // GET todas (sin paginación)
        // ===============================
        [HttpGet("all")]
        public async Task<IActionResult> GetAllSinPaginacion()
        {
            var rutinas = await _context.RutinasPlantilla
                .Include(r => r.GrupoMuscular)
                .OrderBy(r => r.Nombre)
                .Select(r => new
                {
                    r.Id,
                    r.Nombre,
                    r.Objetivo,
                    r.ImagenUrl,
                    GrupoMuscularNombre = r.GrupoMuscular != null ? r.GrupoMuscular.Nombre : "(Sin grupo)"
                })
                .ToListAsync();

            return Ok(rutinas);
        }
    }
}
