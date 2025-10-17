using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/estadoOrdenPago")]
    public class EstadoOrdenPagoController : ControllerBase
    {
        private readonly GymDbContext _db;

        public EstadoOrdenPagoController(GymDbContext db)
        {
            _db = db;
        }

        // ✅ GET /api/estados
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var estados = await _db.EstadoOrdenPago
                .OrderBy(e => e.Id)
                .ToListAsync(ct);
            return Ok(estados);
        }

        // ✅ GET /api/estados/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var estado = await _db.EstadoOrdenPago.FindAsync(new object[] { id }, ct);
            return estado is null ? NotFound() : Ok(estado);
        }

        // ✅ POST /api/estados
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EstadoOrdenPago dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre es obligatorio.");

            _db.EstadoOrdenPago.Add(dto);
            await _db.SaveChangesAsync(ct);
            return Ok(dto);
        }

        // ✅ PUT /api/estados/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EstadoOrdenPago dto, CancellationToken ct)
        {
            var estado = await _db.EstadoOrdenPago.FindAsync(new object[] { id }, ct);
            if (estado == null) return NotFound();

            estado.Nombre = dto.Nombre;
            estado.Descripcion = dto.Descripcion;

            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

        // ✅ DELETE /api/estados/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var estado = await _db.EstadoOrdenPago.FindAsync(new object[] { id }, ct);
            if (estado == null) return NotFound();

            _db.EstadoOrdenPago.Remove(estado);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
