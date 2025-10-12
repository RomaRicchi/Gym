using Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;

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

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var list = await _db.Personales
                .OrderBy(p => p.Nombre)
                .ToListAsync(ct);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(uint id, CancellationToken ct = default)
        {
            var personal = await _db.Personales.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (personal is null) return NotFound();
            return Ok(personal);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Personal dto, CancellationToken ct = default)
        {
            _db.Personales.Add(dto);
            await _db.SaveChangesAsync(ct);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(uint id, [FromBody] Personal dto, CancellationToken ct = default)
        {
            var personal = await _db.Personales.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (personal is null) return NotFound();

            personal.Nombre = dto.Nombre;
            personal.Telefono = dto.Telefono;
            personal.Direccion = dto.Direccion;
            personal.Especialidad = dto.Especialidad;
            personal.Estado = dto.Estado;

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(uint id, CancellationToken ct = default)
        {
            var personal = await _db.Personales.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (personal is null) return NotFound();

            _db.Personales.Remove(personal);
            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true });
        }
    }
}
