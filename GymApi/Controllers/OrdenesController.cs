using GymApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdenesController : ControllerBase
{
    private readonly GymDbContext _db;
    public OrdenesController(GymDbContext db) => _db = db;

    // Pendientes o en revisión para panel
    [HttpGet("pendientes")]
    public async Task<IActionResult> GetPendientes([FromQuery] string estado = "en_revision", CancellationToken ct = default)
    {
        var list = await _db.orden_pago
            .Where(o => o.estado == estado || o.estado == "pendiente")
            .OrderByDescending(o => o.creado_en)
            .Select(o => new {
                o.id, o.socio_id, o.plan_id, o.monto, o.estado, o.creado_en, o.vence_en
            })
            .ToListAsync(ct);

        return Ok(list);
    }

    public record AprobarBody(string? notas, int? diasVigencia); // opcional: prolongar suscripción
    [HttpPatch("{id}/aprobar")]
    public async Task<IActionResult> Aprobar([FromRoute] int id, [FromBody] AprobarBody? body, CancellationToken ct)
    {
        var orden = await _db.orden_pago.FirstOrDefaultAsync(o => o.id == id, ct);
        if (orden is null) return NotFound("Orden no encontrada");
        if (orden.estado == "verificado") return Conflict("Orden ya verificada");
        if (orden.estado == "rechazado") return Conflict("Orden rechazada");
        if (orden.estado == "expirado") return Conflict("Orden expirada");

        orden.estado = "verificado";
        orden.notas = body?.notas;

        // Activar/crear suscripción: 30 días por defecto (o body.diasVigencia)
        var dias = (body?.diasVigencia is > 0) ? body!.diasVigencia!.Value : 30;
        var ahora = DateTime.UtcNow;
        var fin = ahora.AddDays(dias);

        var sus = await _db.suscripcion.FirstOrDefaultAsync(s => s.socio_id == orden.socio_id && s.plan_id == orden.plan_id && s.estado == "activa", ct);
        if (sus is null)
        {
            sus = new Data.Models.suscripcion {
                socio_id = orden.socio_id,
                plan_id = orden.plan_id,
                inicio = ahora,
                fin = fin,
                estado = "activa"
            };
            _db.suscripcion.Add(sus);
        }
        else
        {
            // renovar: extender fin
            if (sus.fin < fin) sus.fin = fin;
            sus.estado = "activa";
        }

        await _db.SaveChangesAsync(ct);
        return Ok(new { ok = true, orden_id = id });
    }

    public record RechazarBody(string motivo);
    [HttpPatch("{id}/rechazar")]
    public async Task<IActionResult> Rechazar([FromRoute] int id, [FromBody] RechazarBody body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.motivo)) return BadRequest("motivo requerido");

        var orden = await _db.orden_pago.FirstOrDefaultAsync(o => o.id == id, ct);
        if (orden is null) return NotFound("Orden no encontrada");
        if (orden.estado == "verificado") return Conflict("Orden ya verificada");

        orden.estado = "rechazado";
        orden.notas = body.motivo;

        // Si hubiese suscripción pendiente, dejarla cancelada
        var sus = await _db.suscripcion.FirstOrDefaultAsync(s => s.socio_id == orden.socio_id && s.plan_id == orden.plan_id && s.estado != "cancelada", ct);
        if (sus is not null) sus.estado = "cancelada";

        await _db.SaveChangesAsync(ct);
        return Ok(new { ok = true, orden_id = id });
    }
}
