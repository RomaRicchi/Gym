using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/ordenes")]
    public class OrdenesPagoController : ControllerBase
    {
        private readonly GymDbContext _db;

        public OrdenesPagoController(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 GET /api/ordenes
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var lista = await _db.OrdenesPago
                .Include(o => o.Socio)
                .Include(o => o.Plan)
                .Include(o => o.Estado)
                .AsNoTracking()
                .OrderByDescending(o => o.Id)
                .Select(o => new
                {
                    o.Id,
                    Socio = o.Socio != null ? o.Socio.Nombre : null,
                    Plan = o.Plan != null ? o.Plan.Nombre : null,
                    Estado = o.Estado != null ? o.Estado.Nombre : null,
                    o.Monto,
                })
                .ToListAsync(ct);

            return Ok(lista);
        }

        // 🔹 GET /api/ordenes/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago
                .Include(o => o.Socio)
                .Include(o => o.Plan)
                .Include(o => o.Estado)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id, ct);

            if (orden == null)
                return NotFound();

            return Ok(new
            {
                orden.Id,
                orden.Monto,
                Socio = orden.Socio?.Nombre,
                Plan = orden.Plan?.Nombre,
                Estado = orden.Estado?.Nombre
            });
        }

        // 🔹 POST /api/ordenes
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] OrdenPago dto, CancellationToken ct)
        {
            if (!await _db.Socios.AnyAsync(s => s.Id == dto.SocioId, ct))
                return BadRequest("El socio no existe.");

            if (!await _db.Planes.AnyAsync(p => p.Id == dto.PlanId, ct))
                return BadRequest("El plan no existe.");

            if (!await _db.EstadoOrdenPago.AnyAsync(e => e.Id == dto.EstadoId, ct))
                return BadRequest("El estado no existe.");

            // ✅ Estado por defecto
            if (dto.EstadoId == 0)
                dto.EstadoId = 1; // pendiente

            dto.Socio = null;
            dto.Plan = null;
            dto.Estado = null;

            _db.OrdenesPago.Add(dto);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }


        // 🔹 PUT /api/ordenes/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] OrdenPago dto, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago.FindAsync(new object[] { id }, ct);
            if (orden == null)
                return NotFound();

            orden.Monto = dto.Monto;
            orden.PlanId = dto.PlanId;
            orden.EstadoId = dto.EstadoId;

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, message = "Orden actualizada correctamente." });
        }

        // 🔹 PATCH /api/ordenes/{id}/estado
        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] int nuevoEstadoId, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago.FindAsync(new object[] { id }, ct);
            if (orden == null)
                return NotFound();

            orden.EstadoId = nuevoEstadoId;
            await _db.SaveChangesAsync(ct);

            return Ok(new { ok = true, message = "Estado actualizado correctamente." });
        }

        // 🔹 DELETE /api/ordenes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago.FindAsync(new object[] { id }, ct);
            if (orden == null)
                return NotFound();

            _db.OrdenesPago.Remove(orden);
            await _db.SaveChangesAsync(ct);

            return Ok(new { ok = true, message = "Orden eliminada correctamente." });
        }


        [HttpGet("por-estado/{estadoId:int:min(1)}")]
        public async Task<IActionResult> GetByEstadoId(
            [FromRoute] int estadoId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _db.OrdenesPago
                .AsNoTracking()
                .Include(o => o.Socio)
                .Include(o => o.Plan)
                .Include(o => o.Estado)
                .Where(o => o.EstadoId == estadoId);

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(o => o.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new
                {
                    o.Id,
                    Socio = o.Socio != null ? o.Socio.Nombre : null,
                    Plan = o.Plan != null ? o.Plan.Nombre : null,
                    Estado = o.Estado != null ? o.Estado.Nombre : null,
                    o.Monto
                })
                .ToListAsync(ct);

            if (!items.Any())
                return NotFound($"No hay órdenes con el estado ID {estadoId}.");

            return Ok(new
            {
                estadoId,
                total,
                page,
                pageSize,
                items
            });
        }
    }
}
