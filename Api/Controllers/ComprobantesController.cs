using Api.Data;
using Api.Services;
using Api.Repositories.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprobantesController : ControllerBase
{
    private readonly GymDbContext _db;
    private readonly IFileStorage _storage;

    public ComprobantesController(GymDbContext db, IFileStorage storage)
    {
        _db = db;
        _storage = storage;
    }

    /// Sube un comprobante (form-data: orden_id, file)
    [HttpPost]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> Upload([FromForm] int orden_id, [FromForm] IFormFile file, CancellationToken ct)
    {
        if (orden_id <= 0) return BadRequest("orden_id requerido");
        if (file is null || file.Length == 0) return BadRequest("Archivo vacío");

        var orden = await _db.Orden_pago.FirstOrDefaultAsync(o => o.id == orden_id, ct);
        if (orden is null) return NotFound("Orden no encontrada");
        if (orden.estado == "verificado") return Conflict("La orden ya fue verificada");
        if (orden.estado == "rechazado") return Conflict("La orden fue rechazada");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
        if (!allowed.Contains(ext)) return BadRequest("Formato no permitido (.jpg/.png/.webp/.pdf)");

        await using var stream = file.OpenReadStream();
        var path = await _storage.SaveAsync(stream, file.FileName, $"orden_{orden.id}", ct);

        var comp = new Data.Models.Comprobante
        {
            orden_id = orden.id,
            file_url = path,
            mime_type = file.ContentType,
            // subido_en tiene default utc_timestamp() en DB
        };
        _db.Comprobante.Add(comp);

        // al subir, la orden pasa a "en_revision"
        orden.estado = "en_revision";
        await _db.SaveChangesAsync(ct);

        return Ok(new { ok = true, Comprobante_id = comp.id, file_url = comp.file_url });
    }

    /// Lista comprobantes de una orden
    [HttpGet("por-orden/{ordenId:int}")]
    public async Task<IActionResult> GetPorOrden([FromRoute] int ordenId, CancellationToken ct)
    {
        var data = await _db.Comprobante
            .Where(c => c.orden_id == ordenId)
            .OrderByDescending(c => c.id)
            .Select(c => new { c.id, c.file_url, c.mime_type, c.subido_en })
            .ToListAsync(ct);

        return Ok(data);
    }
}
