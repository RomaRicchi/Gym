using Api.Data;
using Api.Data.Models;
using Api.Services;
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

    /// <summary>Sube un comprobante (form-data: ordenId, file)</summary>
    [HttpPost]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> Upload([FromForm] int ordenId, [FromForm] IFormFile file, CancellationToken ct)
    {
        if (ordenId <= 0) return BadRequest("orden_id requerido");
        if (file is null || file.Length == 0) return BadRequest("Archivo vacío");

        var orden = await _db.OrdenesPago.FirstOrDefaultAsync(o => o.Id == ordenId, ct);
        if (orden is null) return NotFound("Orden no encontrada");
        if (orden.Estado == "verificado") return Conflict("La orden ya fue verificada");
        if (orden.Estado == "rechazado") return Conflict("La orden fue rechazada");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
        if (!allowed.Contains(ext)) return BadRequest("Formato no permitido (.jpg/.png/.webp/.pdf)");

        await using var stream = file.OpenReadStream();
        var path = await _storage.SaveAsync(stream, file.FileName, $"orden_{orden.Id}", ct);

        var comp = new Comprobante
        {
            OrdenId = (uint)orden.Id,
            FileUrl = path,
            MimeType = file.ContentType,
            SubidoEn = DateTime.UtcNow
        };

        _db.Comprobantes.Add(comp);

        // al subir, la orden pasa a "en_revision"
        orden.Estado = "en_revision";
        await _db.SaveChangesAsync(ct);

        return Ok(new { ok = true, ComprobanteId = comp.Id, comp.FileUrl });
    }

    /// <summary>Lista comprobantes de una orden</summary>
    [HttpGet("por-orden/{ordenId:int}")]
    public async Task<IActionResult> GetPorOrden([FromRoute] int ordenId, CancellationToken ct)
    {
        var data = await _db.Comprobantes
            .Where(c => c.OrdenId == ordenId)
            .OrderByDescending(c => c.Id)
            .Select(c => new { c.Id, c.FileUrl, c.MimeType, c.SubidoEn })
            .ToListAsync(ct);

        return Ok(data);
    }
}
