using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

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
        var list = await _db.OrdenesPago
            .Where(o => o.Estado == estado || o.Estado == "pendiente")
            .OrderByDescending(o => o.CreadoEn)
            .Select(o => new
            {
                o.Id,
                o.SocioId,
                o.PlanId,
                o.Monto,
                o.Estado,
                o.CreadoEn,
                o.VenceEn
            })
            .ToListAsync(ct);

        return Ok(list);
    }

    // ---------- PATCH: Aprobar Orden ----------
    public record AprobarBody(string? Notas, int? DiasVigencia);

    [HttpPatch("{id}/aprobar")]
    public async Task<IActionResult> Aprobar([FromRoute] int id, [FromBody] AprobarBody? body, CancellationToken ct)
    {
        var orden = await _db.OrdenesPago.FirstOrDefaultAsync(o => o.Id == id, ct);
        if (orden is null) return NotFound("Orden no encontrada");
        if (orden.Estado == "verificado") return Conflict("Orden ya verificada");
        if (orden.Estado == "rechazado") return Conflict("Orden rechazada");
        if (orden.Estado == "expirado") return Conflict("Orden expirada");

        orden.Estado = "verificado";
        orden.Notas = body?.Notas;

        // Activar o crear suscripción (30 días por defecto)
        var dias = (body?.DiasVigencia is > 0) ? body!.DiasVigencia!.Value : 30;
        var ahora = DateTime.UtcNow;
        var fin = ahora.AddDays(dias);

        var sus = await _db.Suscripciones.FirstOrDefaultAsync(
            s => s.SocioId == orden.SocioId && s.PlanId == orden.PlanId && s.Estado == "activa", ct);

        if (sus is null)
        {
            sus = new Suscripcion
            {
                SocioId = orden.SocioId,
                PlanId = orden.PlanId,
                Inicio = ahora,
                Fin = fin,
                Estado = "activa",
                CreadoEn = ahora
            };
            _db.Suscripciones.Add(sus);
        }
        else
        {
            if (sus.Fin < fin)
                sus.Fin = fin;

            sus.Estado = "activa";
        }

        await _db.SaveChangesAsync(ct);
        return Ok(new { ok = true, OrdenId = id });
    }

    // ---------- PATCH: Rechazar Orden ----------
    public record RechazarBody(string Motivo);

    [HttpPatch("{id}/rechazar")]
    public async Task<IActionResult> Rechazar([FromRoute] int id, [FromBody] RechazarBody body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.Motivo))
            return BadRequest("motivo requerido");

        var orden = await _db.OrdenesPago.FirstOrDefaultAsync(o => o.Id == id, ct);
        if (orden is null) return NotFound("Orden no encontrada");
        if (orden.Estado == "verificado") return Conflict("Orden ya verificada");

        orden.Estado = "rechazado";
        orden.Notas = body.Motivo;

        // Si existe suscripción pendiente, cancelarla
        var sus = await _db.Suscripciones.FirstOrDefaultAsync(
            s => s.SocioId == orden.SocioId && s.PlanId == orden.PlanId && s.Estado != "cancelada", ct);

        if (sus is not null)
            sus.Estado = "cancelada";

        await _db.SaveChangesAsync(ct);
        return Ok(new { ok = true, OrdenId = id });
    }
}
