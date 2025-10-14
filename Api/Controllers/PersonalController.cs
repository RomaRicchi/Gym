using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalController : ControllerBase
    {
        private readonly GymDbContext _db;

        public PersonalController(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 GET: /api/personal
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var list = await _db.Personales
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .ToListAsync(ct);

            return Ok(list);
        }

        // 🔹 GET: /api/personal/activos
        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos(CancellationToken ct = default)
        {
            var list = await _db.Personales
                .AsNoTracking()
                .Where(p => p.Estado == true)
                .OrderBy(p => p.Nombre)
                .ToListAsync(ct);

            return Ok(list);
        }

        // 🔹 GET: /api/personal/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var personal = await _db.Personales
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, ct);

            if (personal is null)
                return NotFound(new { message = "Personal no encontrado." });

            return Ok(personal);
        }

        // 🔹 POST: /api/personal
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Personal dto, CancellationToken ct = default)
        {
            try
            {
                _db.Personales.Add(dto);
                await _db.SaveChangesAsync(ct);

                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al crear el personal.", error = ex.Message });
            }
        }

        // 🔹 PUT: /api/personal/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Personal dto, CancellationToken ct = default)
        {
            var personal = await _db.Personales.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (personal is null)
                return NotFound(new { message = "Personal no encontrado." });

            personal.Nombre = dto.Nombre;
            personal.Telefono = dto.Telefono;
            personal.Direccion = dto.Direccion;
            personal.Especialidad = dto.Especialidad;
            personal.Estado = dto.Estado;

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, message = "Personal actualizado correctamente." });
        }

        // 🔹 DELETE: /api/personal/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> BajaLogica(int id, CancellationToken ct = default)
        {
            var personal = await _db.Personales.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (personal is null)
                return NotFound(new { message = "Personal no encontrado para eliminar." });

            if (!personal.Estado)
                return BadRequest(new { message = "El personal ya está dado de baja." });

            personal.Estado = false;
            await _db.SaveChangesAsync(ct);

            return Ok(new { ok = true, message = "Baja lógica realizada correctamente." });
        }
    }
}
