using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprobantesController : ControllerBase
    {
        private readonly GymDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ComprobantesController(GymDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Subir([FromForm] IFormFile archivo, [FromForm] int ordenPagoId)
        {
            var orden = await _db.OrdenesPago.FindAsync(ordenPagoId);
            if (orden == null)
                return NotFound($"No se encontró la orden de pago #{ordenPagoId}");

            if (archivo == null || archivo.Length == 0)
                return BadRequest("No se envió ningún archivo");

            var uploadsDir = Path.Combine(_env.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(archivo.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await archivo.CopyToAsync(stream);

            var comprobante = new Comprobante
            {
                OrdenPagoId = ordenPagoId,
                FileUrl = fileName,
                MimeType = archivo.ContentType,
                SubidoEn = DateTime.UtcNow
            };

            _db.Comprobantes.Add(comprobante);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Comprobante subido correctamente",
                comprobante = new
                {
                    comprobante.Id,
                    comprobante.OrdenPagoId,
                    comprobante.FileUrl,
                    comprobante.MimeType,
                    comprobante.SubidoEn
                }
            });
        }

        // ---------- PATCH: Aprobar comprobante ----------
        public record AprobarComprobanteBody(string? Notas);

        [HttpPatch("{id}/aprobar")]
        public async Task<IActionResult> AprobarComprobante([FromRoute] int id, [FromBody] AprobarComprobanteBody? body, CancellationToken ct)
        {
            var comp = await _db.Comprobantes
                .Include(c => c.OrdenPago)
                .ThenInclude(o => o.Estado)
                .FirstOrDefaultAsync(c => c.Id == id, ct);

            if (comp is null)
                return NotFound("Comprobante no encontrado");

            var orden = comp.OrdenPago;
            if (orden is null)
                return NotFound("Orden asociada no encontrada");

            var estadoVerificado = await _db.EstadoOrdenPago
                .FirstOrDefaultAsync(e => e.Nombre == "verificado", ct);

            if (estadoVerificado is null)
                return BadRequest("No existe el estado 'verificado'.");

            orden.EstadoId = estadoVerificado.Id;
            orden.Notas = body?.Notas;

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, ComprobanteId = id });
        }

        // ---------- PATCH: Rechazar comprobante ----------
        public record RechazarComprobanteBody(string Motivo);

        [HttpPatch("{id}/rechazar")]
        public async Task<IActionResult> RechazarComprobante([FromRoute] int id, [FromBody] RechazarComprobanteBody body, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(body.Motivo))
                return BadRequest("Motivo requerido.");

            var comp = await _db.Comprobantes
                .Include(c => c.OrdenPago)
                .ThenInclude(o => o.Estado)
                .FirstOrDefaultAsync(c => c.Id == id, ct);

            if (comp is null)
                return NotFound("Comprobante no encontrado");

            var orden = comp.OrdenPago;
            if (orden is null)
                return NotFound("Orden asociada no encontrada");

            var estadoRechazado = await _db.EstadoOrdenPago
                .FirstOrDefaultAsync(e => e.Nombre == "rechazado", ct);

            if (estadoRechazado is null)
                return BadRequest("No existe el estado 'rechazado'.");

            orden.EstadoId = estadoRechazado.Id;
            orden.Notas = body.Motivo;

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, ComprobanteId = id });
        }
    }
}
