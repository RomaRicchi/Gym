using Api.Data;
using Api.Data.Models;
using Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenesPagoController : ControllerBase
    {
        private readonly GymDbContext _db;
        public OrdenesPagoController(GymDbContext db) => _db = db;

        // ---------- POST: Crear Orden ----------
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] OrdenPagoCreateDto dto, CancellationToken ct)
        {
            if (!await _db.Socios.AnyAsync(s => s.Id == dto.SocioId, ct))
                return BadRequest("El socio no existe.");

            if (!await _db.Planes.AnyAsync(p => p.Id == dto.PlanId, ct))
                return BadRequest("El plan no existe.");

            if (!await _db.EstadoOrdenPago.AnyAsync(e => e.Id == dto.EstadoId, ct))
                return BadRequest("El estado no existe.");

            var entity = new OrdenPago
            {
                SocioId = dto.SocioId,
                PlanId = dto.PlanId,
                EstadoId = dto.EstadoId,
                Monto = dto.Monto,
                VenceEn = dto.VenceEn,
                Notas = dto.Notas,
                CreadoEn = DateTime.UtcNow
            };

            _db.OrdenesPago.Add(entity);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = entity.Id }, entity);
        }

        // ---------- GET: Obtener por ID ----------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago
                .Include(o => o.Socio)
                .Include(o => o.Plan)
                .Include(o => o.Estado)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

            if (orden is null)
                return NotFound();

            return Ok(orden);
        }

        // ---------- GET: Órdenes pendientes ----------
        [HttpGet("pendientes")]
        public async Task<IActionResult> GetPendientes(CancellationToken ct = default)
        {
            var list = await _db.OrdenesPago
                .Include(o => o.Estado)
                .Where(o => o.Estado.Nombre == "pendiente" || o.Estado.Nombre == "en_revision")
                .OrderByDescending(o => o.CreadoEn)
                .Select(o => new
                {
                    o.Id,
                    o.SocioId,
                    o.PlanId,
                    o.Monto,
                    Estado = o.Estado.Nombre,
                    o.CreadoEn,
                    o.VenceEn
                })
                .ToListAsync(ct);

            return Ok(list);
        }

        // ---------- PATCH: Aprobar Orden ----------
        public record AprobarBody(string? Notas, int? DiasVigencia);

        [HttpPatch("{id:int}/aprobar")]
        public async Task<IActionResult> Aprobar([FromRoute] int id, [FromBody] AprobarBody? body, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago
                .Include(o => o.Estado)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

            if (orden is null)
                return NotFound("Orden no encontrada");

            if (orden.Estado.Nombre == "verificado")
                return Conflict("Orden ya verificada");
            if (orden.Estado.Nombre == "rechazado")
                return Conflict("Orden rechazada");

            var estadoVerificado = await _db.EstadoOrdenPago
                .FirstOrDefaultAsync(e => e.Nombre == "verificado", ct);

            if (estadoVerificado is null)
                return BadRequest("Estado 'verificado' no definido.");

            orden.EstadoId = estadoVerificado.Id;
            orden.Notas = body?.Notas;

            // Crear o reactivar suscripción
            var dias = body?.DiasVigencia ?? 30;
            var ahora = DateTime.UtcNow;
            var fin = ahora.AddDays(dias);

            var sus = await _db.Suscripciones.FirstOrDefaultAsync(
                s => s.SocioId == orden.SocioId && s.PlanId == orden.PlanId && s.Estado, ct);

            if (sus is null)
            {
                sus = new Suscripcion
                {
                    SocioId = orden.SocioId,
                    PlanId = orden.PlanId,
                    Inicio = ahora,
                    Fin = fin,
                    Estado = true,
                    CreadoEn = ahora
                };
                _db.Suscripciones.Add(sus);
            }
            else
            {
                if (sus.Fin < fin)
                    sus.Fin = fin;
                sus.Estado = true;
            }

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, ordenId = id });
        }

        // ---------- PATCH: Rechazar Orden ----------
        public record RechazarBody(string Motivo);

        [HttpPatch("{id:int}/rechazar")]
        public async Task<IActionResult> Rechazar([FromRoute] int id, [FromBody] RechazarBody body, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(body.Motivo))
                return BadRequest("Motivo requerido.");

            var orden = await _db.OrdenesPago
                .Include(o => o.Estado)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

            if (orden is null)
                return NotFound("Orden no encontrada");

            if (orden.Estado.Nombre == "verificado")
                return Conflict("Orden ya verificada");

            // Buscar estado 'rechazado'
            var estadoRechazado = await _db.EstadoOrdenPago
                .FirstOrDefaultAsync(e => e.Nombre == "rechazado", ct);

            if (estadoRechazado is null)
                return BadRequest("Estado 'rechazado' no definido.");

            orden.EstadoId = estadoRechazado.Id;
            orden.Notas = body.Motivo;

            // Buscar suscripción activa del mismo socio y plan
            var sus = await _db.Suscripciones.FirstOrDefaultAsync(
                s => s.SocioId == orden.SocioId && s.PlanId == orden.PlanId && s.Estado, ct);

            if (sus is not null)
                sus.Estado = false; // baja lógica de la suscripción

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, ordenId = id });
        }

        // ---------- GET: Listado completo de órdenes ----------
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var list = await _db.OrdenesPago
                .Include(o => o.Socio)
                .Include(o => o.Plan)
                .Include(o => o.Estado)
                .OrderByDescending(o => o.CreadoEn)
                .Select(o => new
                {
                    o.Id,
                    Socio = o.Socio.Nombre,
                    Plan = o.Plan.Nombre,
                    Estado = o.Estado.Nombre,
                    o.Monto,
                    o.VenceEn,
                    o.Notas
                })
                .ToListAsync(ct);

            return Ok(new
            {
                ok = true,
                total = list.Count,
                data = list
            });
        }
    }
}
