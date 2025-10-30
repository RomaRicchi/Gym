// fileName: ComprobantesController.cs
using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/comprobantes")]
    [Authorize(Roles = "Administrador, Profesor, Recepción")]
    public class ComprobantesController : ControllerBase
    {
        private readonly GymDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ComprobantesController> _logger;

        public ComprobantesController(GymDbContext db, IWebHostEnvironment env, ILogger<ComprobantesController> logger)
        {
            _db = db;
            _env = env;
            _logger = logger;
        }

        // 🔍 GET /api/comprobantes/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var comprobante = await _db.Comprobantes
                .Include(c => c.OrdenPago)
                .FirstOrDefaultAsync(c => c.Id == id, ct);

            if (comprobante == null)
                return NotFound("Comprobante no encontrado.");

            return Ok(new
            {
                comprobante.Id,
                comprobante.FileUrl,
                comprobante.MimeType,
                comprobante.SubidoEn,
                Orden = comprobante.OrdenPago != null
                    ? new { comprobante.OrdenPago.Id, comprobante.OrdenPago.Monto }
                    : null
            });
        }

        //  GET /api/ordenes/{ordenId}/comprobante
        [HttpGet("/api/ordenes/{ordenId:int}/comprobante")]
        public async Task<IActionResult> GetByOrden(int ordenId, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago
                .Include(o => o.Comprobante)
                .FirstOrDefaultAsync(o => o.Id == ordenId, ct);

            if (orden == null)
                return NotFound("La orden no existe.");

            if (orden.Comprobante == null)
                return Ok(null);

            var c = orden.Comprobante;
            return Ok(new
            {
                c.Id,
                c.FileUrl,
                c.MimeType,
                c.SubidoEn
            });
        }

        // POST /api/comprobantes
        [HttpPost]
        public async Task<IActionResult> Subir([FromForm] IFormFile file, [FromForm] int ordenPagoId, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Debe subir un archivo válido.");

            // Buscar orden y socio
            var orden = await _db.OrdenesPago
                .Include(o => o.Socio)
                .FirstOrDefaultAsync(o => o.Id == ordenPagoId, ct);

            if (orden == null)
                return NotFound("Orden no encontrada.");

            var socioId = orden.SocioId;

            // Validar extensión y MIME
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExt = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var allowedMime = new[] { "application/pdf", "image/jpeg", "image/png" };

            if (!allowedExt.Contains(ext) || !allowedMime.Contains(file.ContentType.ToLowerInvariant()))
                return BadRequest("Solo se permiten archivos PDF o imágenes (.jpg, .png).");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("El archivo no puede superar los 5 MB.");

            //  Crear carpeta del socio
            var uploads = Path.Combine(
                _env.WebRootPath ?? "wwwroot",
                "uploads", "comprobantes", "socios", socioId.ToString()
            );
            Directory.CreateDirectory(uploads);

            // Guardar archivo
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploads, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true))
                await file.CopyToAsync(stream, ct);

            var comprobante = new Comprobante
            {
                FileUrl = $"uploads/comprobantes/socios/{socioId}/{fileName}",
                MimeType = file.ContentType,
                SubidoEn = DateTime.UtcNow
            };

            await using var trx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                _db.Comprobantes.Add(comprobante);
                await _db.SaveChangesAsync(ct);

                orden.ComprobanteId = comprobante.Id;
                _db.OrdenesPago.Update(orden);
                await _db.SaveChangesAsync(ct);

                await trx.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await trx.RollbackAsync(ct);
                _logger.LogError(ex, "Error al guardar comprobante para orden {OrdenId}", ordenPagoId);
                return StatusCode(500, "Error al guardar el comprobante.");
            }

            _logger.LogInformation("Usuario {User} subió comprobante {File} para socio {SocioId}", User.Identity?.Name, fileName, socioId);

            return Ok(new
            {
                message = "Comprobante guardado correctamente.",
                comprobanteId = comprobante.Id,
                fileUrl = comprobante.FileUrl
            });
        }

        // DELETE /api/comprobantes/{id}
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            var comprobante = await _db.Comprobantes
                .Include(c => c.OrdenPago)
                .FirstOrDefaultAsync(c => c.Id == id, ct);

            if (comprobante == null)
                return NotFound("Comprobante no encontrado.");

            // Eliminar archivo físico (con protección)
            try
            {
                var filePath = Path.Combine(
                    _env.WebRootPath ?? "wwwroot",
                    comprobante.FileUrl.Replace('/', Path.DirectorySeparatorChar)
                );
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                // Si la carpeta del socio queda vacía, la eliminamos
                var carpetaSocio = Path.GetDirectoryName(filePath);
                if (Directory.Exists(carpetaSocio) && !Directory.EnumerateFileSystemEntries(carpetaSocio).Any())
                    Directory.Delete(carpetaSocio);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo eliminar el archivo físico del comprobante {Id}", id);
            }

            // Desvincular de la orden
            if (comprobante.OrdenPago?.ComprobanteId != null)
            {
                comprobante.OrdenPago.ComprobanteId = null;
                _db.OrdenesPago.Update(comprobante.OrdenPago);
            }

            _db.Comprobantes.Remove(comprobante);
            await _db.SaveChangesAsync(ct);

            return Ok(new { message = "Comprobante eliminado correctamente." });
        }

        // GET /api/comprobantes/{id}/download
        [HttpGet("{id:int}/download")]
        public async Task<IActionResult> Descargar(int id, CancellationToken ct)
        {
            var comprobante = await _db.Comprobantes.FindAsync(new object[] { id }, ct);
            if (comprobante == null)
                return NotFound("Comprobante no encontrado.");

            var path = Path.Combine(_env.WebRootPath ?? "wwwroot", comprobante.FileUrl.Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(path))
                return NotFound("Archivo físico no encontrado.");

            var bytes = await System.IO.File.ReadAllBytesAsync(path, ct);
            return File(bytes, comprobante.MimeType, Path.GetFileName(path));
        }
    }
}
