using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/personal")]
    public class PersonalController : ControllerBase
    {
        private readonly GymDbContext _db;

        public PersonalController(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 GET /api/personal
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var lista = await _db.Personales
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .Select(p => new
                {
                    id = p.Id,
                    nombre = p.Nombre,
                    email = $"{p.Nombre.Replace(" ", ".").ToLower()}@gym.com", // simulado
                    telefono = p.Telefono,
                    rol = p.Especialidad,
                    activo = p.Estado
                })
                .ToListAsync(ct);

            return Ok(lista);
        }

        // 🔹 GET /api/personal/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var p = await _db.Personales
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(p => new
                {
                    id = p.Id,
                    nombre = p.Nombre,
                    email = $"{p.Nombre.Replace(" ", ".").ToLower()}@gym.com",
                    telefono = p.Telefono,
                    rol = p.Especialidad,
                    activo = p.Estado
                })
                .FirstOrDefaultAsync(ct);

            if (p is null) return NotFound(new { message = "Personal no encontrado." });
            return Ok(p);
        }

        // 🔹 POST /api/personal
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] dynamic dto, CancellationToken ct = default)
        {
            try
            {
                var nuevo = new Personal
                {
                    Nombre = dto.nombre,
                    Telefono = dto.telefono,
                    Direccion = "-",
                    Especialidad = dto.rol,
                    Estado = dto.activo
                };

                _db.Personales.Add(nuevo);
                await _db.SaveChangesAsync(ct);

                return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al crear el personal.", error = ex.Message });
            }
        }

        // 🔹 PUT /api/personal/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] dynamic dto, CancellationToken ct = default)
        {
            var p = await _db.Personales.FindAsync(new object[] { id }, ct);
            if (p is null)
                return NotFound(new { message = "Personal no encontrado." });

            p.Nombre = dto.nombre;
            p.Telefono = dto.telefono;
            p.Especialidad = dto.rol;
            p.Estado = dto.activo;

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, message = "Personal actualizado correctamente." });
        }

        // 🔹 DELETE /api/personal/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            var p = await _db.Personales.FindAsync(new object[] { id }, ct);
            if (p is null)
                return NotFound(new { message = "Personal no encontrado." });

            p.Estado = false;
            await _db.SaveChangesAsync(ct);

            return Ok(new { ok = true, message = "Personal desactivado correctamente." });
        }
    }
}
