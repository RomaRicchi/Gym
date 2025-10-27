// fileName: ComprobantesController.cs
using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting; 
using System.IO; 
using System; // Agregado para Guid, DateTime.UtcNow
using System.Linq; 
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace Api.Controllers
{
    [ApiController]
    [Route("api/comprobantes")]
    public class ComprobantesController : ControllerBase
    {
        private readonly GymDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ComprobantesController(GymDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [Authorize(Roles = "Administrador, Profesor, Recepción")]
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

        [Authorize(Roles = "Administrador, Profesor, Recepción")]
        [HttpGet("/api/ordenes/{ordenId:int}/comprobante")]
        public async Task<IActionResult> GetByOrden(int ordenId, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago
                .Include(o => o.Comprobante)
                .FirstOrDefaultAsync(o => o.Id == ordenId, ct);

            if (orden == null)
                return NotFound("La orden no existe.");

            if (orden.Comprobante == null)
                return Ok(null); // orden sin comprobante

            var c = orden.Comprobante;
            return Ok(new
            {
                c.Id,
                c.FileUrl,
                c.MimeType,
                c.SubidoEn
            });
        }

        [Authorize(Roles = "Administrador, Profesor, Recepción")]
        [HttpPost]
        public async Task<IActionResult> Subir([FromForm] IFormFile file, [FromForm] int ordenPagoId, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Debe subir un archivo válido.");

            var orden = await _db.OrdenesPago.FindAsync(new object[] { ordenPagoId }, ct);
            if (orden == null)
                return NotFound("Orden no encontrada.");

            var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "comprobantes");
            Directory.CreateDirectory(uploads);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            if (!allowed.Contains(ext))
                return BadRequest("Solo se permiten archivos PDF o imágenes (.jpg, .png).");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("El archivo no puede superar los 5 MB.");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploads, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, ct);
            }

            var comprobante = new Comprobante
            {
                FileUrl = $"uploads/comprobantes/{fileName}",
                MimeType = file.ContentType,
                SubidoEn = DateTime.UtcNow,
            };

            _db.Comprobantes.Add(comprobante);
            await _db.SaveChangesAsync(ct);

            // Asociar comprobante a la orden
            orden.ComprobanteId = comprobante.Id;
            _db.OrdenesPago.Update(orden);
            await _db.SaveChangesAsync(ct);

            return Ok(new
            {
                message = "Comprobante guardado correctamente",
                comprobanteId = comprobante.Id,
                fileUrl = comprobante.FileUrl
            });
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            var comprobante = await _db.Comprobantes
                .Include(c => c.OrdenPago)
                .FirstOrDefaultAsync(c => c.Id == id, ct);

            if (comprobante == null)
                return NotFound("Comprobante no encontrado.");

            // 🧹 Eliminar archivo físico
            var filePath = Path.Combine(_env.WebRootPath ?? "wwwroot", comprobante.FileUrl.Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            if (comprobante.OrdenPago?.ComprobanteId != null)
                {
                    comprobante.OrdenPago.ComprobanteId = null;
                    _db.OrdenesPago.Update(comprobante.OrdenPago);
                }

            _db.Comprobantes.Remove(comprobante);
            await _db.SaveChangesAsync(ct);

            return Ok(new { message = "Comprobante eliminado correctamente." });
        }
    }
}