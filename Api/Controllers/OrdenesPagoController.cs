using Api.Data;
using Api.Data.Models;
using Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System; // Para DateTime, Guid
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Recepción, Socio")]
    [ApiController]
    [Route("api/ordenes")]
    public class OrdenesPagoController : ControllerBase
    {
        private readonly GymDbContext _db;
        private readonly IWebHostEnvironment _env;

        public OrdenesPagoController(GymDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // GET /api/ordenes
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var ordenes = await _db.OrdenesPago
                .Include(o => o.Socio)
                .Include(o => o.Plan)
                .Include(o => o.Estado)
                .Include(o => o.Comprobante)
                .AsNoTracking()
                .OrderByDescending(o => o.CreadoEn)
                .Select(o => new
                {
                    o.Id,
                    Socio = o.Socio != null ? new { o.Socio.Id, o.Socio.Nombre, o.Socio.Email } : null,
                    Plan = o.Plan != null ? new { o.Plan.Id, o.Plan.Nombre } : null,
                    Estado = o.Estado != null ? new { o.Estado.Id, o.Estado.Nombre } : null,
                    o.Monto,
                    o.CreadoEn,
                    o.VenceEn,
                    Comprobante = o.Comprobante != null
                        ? new { o.Comprobante.Id, o.Comprobante.FileUrl }
                        : null
                })
                .ToListAsync(ct);

            return Ok(ordenes);
        }


        // GET /api/ordenes/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            try
            {
                var orden = await _db.OrdenesPago
                    .Include(o => o.Socio)
                    .Include(o => o.Plan)
                    .Include(o => o.Estado)
                    .Include(o => o.Comprobante)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == id, ct);

                if (orden == null)
                    return NotFound(new { message = "Orden de pago no encontrada." });

                return Ok(new
                {
                    orden.Id,
                    Socio = orden.Socio != null ? new { orden.Socio.Id, orden.Socio.Nombre, orden.Socio.Email } : null,
                    Plan = orden.Plan != null ? new { orden.Plan.Id, orden.Plan.Nombre } : null,
                    Estado = orden.Estado != null ? new { orden.Estado.Id, orden.Estado.Nombre } : null,
                    orden.Monto,
                    orden.CreadoEn,
                    orden.VenceEn,
                    Comprobante = orden.Comprobante != null
                        ? new { orden.Comprobante.Id, orden.Comprobante.FileUrl }
                        : null
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener orden de pago por ID: {ex.Message}");
                return StatusCode(500, new { message = "Error al obtener la orden de pago." });
            }
        }

        // PATCH /api/ordenes/{id}/comprobante
        [HttpPatch("{id:int}/comprobante")]
        public async Task<IActionResult> AsignarComprobante(int id, [FromBody] ComprobanteLinkDto dto, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago.FindAsync(new object[] { id }, ct);
            if (orden == null)
                return NotFound("Orden no encontrada.");

            if (dto.ComprobanteId.HasValue && dto.ComprobanteId.Value != 0)
            {
                var comprobante = await _db.Comprobantes.FindAsync(new object[] { dto.ComprobanteId.Value }, ct);
                if (comprobante == null)
                    return BadRequest("El comprobante no existe.");
            }

            orden.ComprobanteId = dto.ComprobanteId;
            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, message = "Comprobante asignado correctamente." });
        }

        
        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] OrdenPagoCreateDto dto, [FromForm] IFormFile? file, CancellationToken ct)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Datos inválidos.");

                // Validaciones básicas
                var socio = await _db.Socios.FindAsync(new object[] { dto.SocioId }, ct);
                if (socio == null)
                    return BadRequest("El socio no existe.");

                var plan = await _db.Planes.FindAsync(new object[] { dto.PlanId }, ct);
                if (plan == null)
                    return BadRequest("El plan no existe.");

                var estado = await _db.EstadoOrdenPago.FindAsync(new object[] { dto.EstadoId }, ct);
                if (estado == null)
                    return BadRequest("El estado no existe.");

                // Crear la orden base
                var fechaInicio = dto.FechaInicio;
                var orden = new OrdenPago
                {
                    SocioId = dto.SocioId,
                    PlanId = dto.PlanId,
                    EstadoId = dto.EstadoId == 0 ? 1 : dto.EstadoId,
                    Monto = plan.Precio,
                    CreadoEn = DateTime.UtcNow,
                    VenceEn = fechaInicio.AddMonths(1),
                    Notas = dto.Notas
                };

                _db.OrdenesPago.Add(orden);
                await _db.SaveChangesAsync(ct);

                // Si hay archivo, subir comprobante
                if (file != null && file.Length > 0)
                {
                    var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "comprobantes");
                    Directory.CreateDirectory(uploads);

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    var allowed = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                    if (!allowed.Contains(ext))
                        return BadRequest("Solo se permiten archivos PDF o imágenes (.jpg, .png).");

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploads, fileName);

                    await using (var stream = new FileStream(filePath, FileMode.Create))
                        await file.CopyToAsync(stream, ct);

                    var comprobante = new Comprobante
                    {
                        FileUrl = $"uploads/comprobantes/{fileName}",
                        MimeType = file.ContentType,
                        SubidoEn = DateTime.UtcNow
                    };

                    _db.Comprobantes.Add(comprobante);
                    await _db.SaveChangesAsync(ct);

                    orden.ComprobanteId = comprobante.Id;
                    _db.OrdenesPago.Update(orden);
                    await _db.SaveChangesAsync(ct);
                }

                // Si la orden fue creada como "Verificada" (EstadoId == 3) → crear suscripción
                if (orden.EstadoId == 3)
                {
                    var inicio = dto.FechaInicio;
                    var fin = inicio.AddDays(30);

                    var suscripcion = new Suscripcion
                    {
                        SocioId = orden.SocioId,
                        PlanId = orden.PlanId,
                        Inicio = inicio,
                        Fin = fin,
                        Estado = true,
                        CreadoEn = DateTime.UtcNow,
                        OrdenPagoId = orden.Id
                    };

                    _db.Suscripciones.Add(suscripcion);
                    await _db.SaveChangesAsync(ct);
                }

                // Recuperar comprobante para devolver al frontend
                var comprobanteData = orden.ComprobanteId.HasValue
                    ? await _db.Comprobantes
                        .Where(c => c.Id == orden.ComprobanteId)
                        .Select(c => new { c.Id, c.FileUrl, c.MimeType })
                        .FirstOrDefaultAsync(ct)
                    : null;

                // Devolver respuesta lista para el SweetAlert
                return CreatedAtAction(nameof(GetById), new { id = orden.Id }, new
                {
                    orden.Id,
                    orden.Monto,
                    orden.EstadoId,
                    orden.VenceEn,
                    comprobante = comprobanteData // clave en minúscula
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR Crear Orden] {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        // PUT /api/ordenes/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] OrdenPagoCreateDto dto, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago.FindAsync(new object[] { id }, ct);
            if (orden == null)
                return NotFound("Orden no encontrada.");

            var plan = await _db.Planes.FindAsync(new object[] { dto.PlanId }, ct);
            if (plan == null)
                return BadRequest("El plan no existe.");

            orden.PlanId = dto.PlanId;
            orden.EstadoId = dto.EstadoId;
            orden.Monto = plan.Precio;
            orden.VenceEn = dto.FechaInicio.AddMonths(1);
            orden.Notas = dto.Notas;

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, message = "Orden actualizada correctamente." });
        }

        [Authorize(Roles = "Socio")]
        [HttpPost("socio")]
        public async Task<IActionResult> CrearPorSocio([FromForm] int PlanId, [FromForm] IFormFile file, CancellationToken ct)
        {
            try
            {
                // ✅ Obtener el email desde el claim correcto del token
                var userEmail = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                if (string.IsNullOrEmpty(userEmail))
                {
                    Console.WriteLine("⚠️ No se pudo obtener el email del token del socio.");
                    return Unauthorized("No se encontró el socio asociado al token.");
                }

                Console.WriteLine($"✅ Socio autenticado: {userEmail}");

                // ✅ Buscar socio por email
                var socio = await _db.Socios.FirstOrDefaultAsync(s => s.Email == userEmail, ct);
                if (socio == null)
                    return Unauthorized("No se encontró el socio en la base de datos.");

                // ✅ Validar el plan
                var plan = await _db.Planes.FindAsync(new object[] { PlanId }, ct);
                if (plan == null)
                    return BadRequest("El plan seleccionado no existe.");

                // ✅ Crear la orden
                var orden = new OrdenPago
                {
                    SocioId = socio.Id,
                    PlanId = PlanId,
                    EstadoId = 1, // Pendiente
                    Monto = plan.Precio,
                    CreadoEn = DateTime.UtcNow,
                    VenceEn = DateTime.UtcNow.AddMonths(1),
                    Notas = "Orden generada por socio desde PlanesSocio"
                };

                _db.OrdenesPago.Add(orden);
                await _db.SaveChangesAsync(ct);

                // ✅ Guardar comprobante si se adjunta
                if (file != null && file.Length > 0)
                {
                    var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "comprobantes");
                    Directory.CreateDirectory(uploads);

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    var allowed = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                    if (!allowed.Contains(ext))
                        return BadRequest("Solo se permiten archivos PDF o imágenes (.jpg, .png).");

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploads, fileName);

                    await using (var stream = new FileStream(filePath, FileMode.Create))
                        await file.CopyToAsync(stream, ct);

                    var comprobante = new Comprobante
                    {
                        FileUrl = $"uploads/comprobantes/{fileName}",
                        MimeType = file.ContentType,
                        SubidoEn = DateTime.UtcNow
                    };

                    _db.Comprobantes.Add(comprobante);
                    await _db.SaveChangesAsync(ct);

                    orden.ComprobanteId = comprobante.Id;
                    _db.OrdenesPago.Update(orden);
                    await _db.SaveChangesAsync(ct);
                }

                Console.WriteLine($"🧾 Orden creada correctamente para socio {socio.Email} (PlanId {PlanId})");

                return Ok(new { ok = true, message = "Orden creada correctamente.", orden.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR CrearPorSocio] {ex.Message}");
                return StatusCode(500, new { message = "Error al crear la orden.", ex.Message });
            }
        }



        // PATCH /api/ordenes/{id}/estado
        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] int nuevoEstadoId, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago.FindAsync(new object[] { id }, ct);
            if (orden == null)
                return NotFound("Orden no encontrada.");

            orden.EstadoId = nuevoEstadoId;
            await _db.SaveChangesAsync(ct);

            // ✅ Si se verifica, crear suscripción
            if (nuevoEstadoId == 3)
            {
                var inicio = DateTime.UtcNow;
                var fin = inicio.AddDays(30);

                var suscripcion = new Suscripcion
                {
                    SocioId = orden.SocioId,
                    PlanId = orden.PlanId,
                    Inicio = inicio,
                    Fin = fin,
                    Estado = true,
                    CreadoEn = DateTime.UtcNow,
                    OrdenPagoId = orden.Id
                };

                _db.Suscripciones.Add(suscripcion);
                await _db.SaveChangesAsync(ct);
            }

            return Ok(new { ok = true, message = "Estado actualizado correctamente." });
        }

        // DELETE /api/ordenes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago
                .Include(o => o.Comprobante)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

            if (orden == null)
                return NotFound("Orden no encontrada.");

            if (orden.Comprobante is not null)
            {
                var filePath = Path.Combine("wwwroot", orden.Comprobante.FileUrl.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _db.Comprobantes.Remove(orden.Comprobante);
            }

            _db.OrdenesPago.Remove(orden);
            await _db.SaveChangesAsync(ct);

            return Ok(new { ok = true, message = "Orden y comprobante eliminados correctamente." });
        }

        // PUT /api/ordenes/{id}/estado/simple 
        [HttpPut("{id:int}/estado/simple")]
        public async Task<IActionResult> ActualizarSoloEstado(int id, [FromBody] EstadoUpdateDto dto, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago.FindAsync(new object[] { id }, ct);
            if (orden == null)
                return NotFound("Orden no encontrada.");

            orden.EstadoId = dto.EstadoId;
            await _db.SaveChangesAsync(ct);

            // ✅ Si el estado pasa a Verificada → crear suscripción
            if (dto.EstadoId == 3)
            {
                var inicio = DateTime.UtcNow;
                var fin = inicio.AddDays(30);

                var suscripcion = new Suscripcion
                {
                    SocioId = orden.SocioId,
                    PlanId = orden.PlanId,
                    Inicio = inicio,
                    Fin = fin,
                    Estado = true,
                    CreadoEn = DateTime.UtcNow,
                    OrdenPagoId = orden.Id
                };

                _db.Suscripciones.Add(suscripcion);
                await _db.SaveChangesAsync(ct);
            }

            return Ok(new { ok = true, message = "Estado actualizado correctamente (con creación de suscripción)." });
        }

        public class EstadoUpdateDto
        {
            public int EstadoId { get; set; }
        }
    }
}
