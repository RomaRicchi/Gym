using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // 🔹 GET /api/ordenes/{id}/comprobantes
        [HttpGet("/api/ordenes/{ordenId:int}/comprobantes")]
        public async Task<IActionResult> GetByOrden(int ordenId, CancellationToken ct)
        {
            var comprobantes = await _db.Comprobantes
                .Where(c => c.OrdenPagoId == ordenId)
                .OrderByDescending(c => c.SubidoEn)
                .Select(c => new
                {
                    c.Id,
                    c.FileUrl,
                    c.MimeType,
                    c.SubidoEn
                })
                .ToListAsync(ct);

            return Ok(comprobantes);
        }

        // 🔹 POST /api/comprobantes
        // Solo guarda el archivo vinculado a una orden (no cambia estados)
        [HttpPost]
        public async Task<IActionResult> Subir(
            [FromForm] IFormFile archivo,
            [FromForm] int ordenPagoId,
            CancellationToken ct)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Debe enviar un archivo válido.");

            var orden = await _db.OrdenesPago.FindAsync(new object[] { ordenPagoId }, ct);
            if (orden == null)
                return NotFound("La orden asociada no existe.");

            var uploadsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "comprobantes");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(archivo.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await archivo.CopyToAsync(stream, ct);

            var comprobante = new Comprobante
            {
                OrdenPagoId = ordenPagoId,
                FileUrl = $"uploads/comprobantes/{fileName}",
                MimeType = archivo.ContentType,
                SubidoEn = DateTime.UtcNow
            };

            _db.Comprobantes.Add(comprobante);
            await _db.SaveChangesAsync(ct);

            return Ok(new
            {
                message = "Comprobante cargado correctamente.",
                comprobante.Id,
                comprobante.FileUrl,
                comprobante.MimeType
            });
        }

        // 🔹 DELETE /api/comprobantes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            var comprobante = await _db.Comprobantes.FindAsync(new object[] { id }, ct);
            if (comprobante == null)
                return NotFound();

            var filePath = Path.Combine(_env.WebRootPath ?? "wwwroot", comprobante.FileUrl.Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _db.Comprobantes.Remove(comprobante);
            await _db.SaveChangesAsync(ct);

            return Ok(new { message = "Comprobante eliminado correctamente." });
        }
    }
}
