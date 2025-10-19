using Api.Data;
using Api.Data.Models;
using Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System; // Agregado para DateTime.UtcNow
using System.IO; 
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Recepcionista")]
    [ApiController]
    [Route("api/ordenes")]
    public class OrdenesPagoController : ControllerBase
    {
        private readonly GymDbContext _db;

        public OrdenesPagoController(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 GET /api/ordenes
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            try
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
                        Socio = o.Socio != null
                            ? new { o.Socio.Id, o.Socio.Nombre, o.Socio.Email }
                            : null,
                        Plan = o.Plan != null
                            ? new { o.Plan.Id, o.Plan.Nombre }
                            : null,
                        Estado = o.Estado != null
                            ? new { o.Estado.Id, o.Estado.Nombre }
                            : null,
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
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener órdenes de pago: {ex.Message}");
                return StatusCode(500, new { message = "Error al obtener las órdenes de pago." });
            }
        }


        // 🔹 GET /api/ordenes/{id}
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
                    .Where(o => o.Id == id)
                    .Select(o => new
                    {
                        o.Id,
                        Socio = o.Socio != null
                            ? new { o.Socio.Id, o.Socio.Nombre, o.Socio.Email }
                            : null,
                        Plan = o.Plan != null
                            ? new { o.Plan.Id, o.Plan.Nombre }
                            : null,
                        Estado = o.Estado != null
                            ? new { o.Estado.Id, o.Estado.Nombre }
                            : null,
                        o.Monto,
                        o.CreadoEn,
                        o.VenceEn,
                        Comprobante = o.Comprobante != null
                            ? new { o.Comprobante.Id, o.Comprobante.FileUrl }
                            : null
                    })
                    .FirstOrDefaultAsync(ct);

                if (orden == null)
                    return NotFound(new { message = "Orden de pago no encontrada." });

                return Ok(orden);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener orden de pago por ID: {ex.Message}");
                return StatusCode(500, new { message = "Error al obtener la orden de pago." });
            }
        }

  // 🔹 PATCH /api/ordenes/{id}/comprobante
        [HttpPatch("{id:int}/comprobante")]
        public async Task<IActionResult> AsignarComprobante(int id, [FromBody] ComprobanteLinkDto dto, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago.FindAsync(new object[] { id }, ct);
            if (orden == null)
                return NotFound("Orden no encontrada.");

            // Check if ComprobanteId is valid if provided
            if (dto.ComprobanteId.HasValue && dto.ComprobanteId.Value != 0)
            {
                var comprobante = await _db.Comprobantes.FindAsync(new object[] { dto.ComprobanteId.Value }, ct);
                if (comprobante == null)
                    return BadRequest("El comprobante no existe.");
            }
            
            // Assign or clear the ComprobanteId
            orden.ComprobanteId = dto.ComprobanteId;

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, message = "Comprobante asignado correctamente." });
        }


        // [MÉTODO CREAR COMPLETO AQUÍ]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] OrdenPagoCreateDto dto, CancellationToken ct)
        {
            try
            {
                // ✅ Verifica que el modelo sea válido y muestra el detalle si algo está mal
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validaciones de existencia
                var socio = await _db.Socios.FindAsync(new object[] { dto.SocioId }, ct);
                if (socio == null)
                    return BadRequest("El socio no existe.");

                var plan = await _db.Planes.FindAsync(new object[] { dto.PlanId }, ct);
                if (plan == null)
                    return BadRequest("El plan no existe.");

                var estado = await _db.EstadoOrdenPago.FindAsync(new object[] { dto.EstadoId }, ct);
                if (estado == null)
                    return BadRequest("El estado no existe.");

                // Crear la orden
                var orden = new OrdenPago
                {
                    SocioId = dto.SocioId,
                    PlanId = dto.PlanId,
                    EstadoId = dto.EstadoId == 0 ? 1 : dto.EstadoId,
                    Monto = plan.Precio,
                    VenceEn = dto.VenceEn,
                    Notas = dto.Notas,
                    CreadoEn = DateTime.UtcNow,
                    ComprobanteId = dto.ComprobanteId
                };

                _db.OrdenesPago.Add(orden);
                await _db.SaveChangesAsync(ct);

                // Si está verificada → crear suscripción
                if (orden.EstadoId == 3)
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

                return CreatedAtAction(nameof(GetById), new { id = orden.Id }, orden);
            }
            catch (Exception ex)
            {
                // 🔍 Te muestra el error real en consola y devuelve mensaje descriptivo
                Console.WriteLine($"[ERROR Crear Orden] {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }


        // 🔹 PUT /api/ordenes/{id}
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
            orden.VenceEn = dto.VenceEn;
            orden.Notas = dto.Notas;
            orden.ComprobanteId = dto.ComprobanteId; // ✅ mantener relación 1:1

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, message = "Orden actualizada correctamente." });
        }

        // 🔹 PATCH /api/ordenes/{id}/estado
        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] int nuevoEstadoId, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago.FindAsync(new object[] { id }, ct);
            if (orden == null)
                return NotFound("Orden no encontrada.");

            orden.EstadoId = nuevoEstadoId;
            await _db.SaveChangesAsync(ct);

            // 🧩 Si cambió a "Verificado", crear suscripción automáticamente
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
                var filePath = Path.Combine("wwwroot", orden.Comprobante!.FileUrl.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _db.Comprobantes.Remove(orden.Comprobante);
            }


            _db.OrdenesPago.Remove(orden);
            await _db.SaveChangesAsync(ct);

            return Ok(new { ok = true, message = "Orden y comprobante eliminados correctamente." });
        }

        //  PUT /api/ordenes/{id}/estado/simple
        [HttpPut("{id:int}/estado/simple")]
        public async Task<IActionResult> ActualizarSoloEstado(int id, [FromBody] EstadoUpdateDto dto, CancellationToken ct)
        {
            var orden = await _db.OrdenesPago.FindAsync(new object[] { id }, ct);
            if (orden == null)
                return NotFound("Orden no encontrada.");

            orden.EstadoId = dto.EstadoId;
            await _db.SaveChangesAsync(ct);

            return Ok(new { ok = true, message = "Estado actualizado correctamente (sin crear suscripción)." });
        }

        public class EstadoUpdateDto
            {
                public int EstadoId { get; set; }
            }

    }
}
