using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gym.Application.DTOs.Pagos;
using Gym.Application.DTOs.Common;
using Gym.Domain.Entities;
using Gym.Domain.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.API.Controllers;

[Authorize]
[ApiController]
[Route("api/ordenes-pago")]
public class OrdenesPagoController : ControllerBase
{
    private readonly IOrdenPagoRepository _repo;
    private readonly GymDbContext _db;

    public OrdenesPagoController(IOrdenPagoRepository repo, GymDbContext db)
    {
        _repo = repo;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1,
        int pageSize = 10,
        int? socioId = null,
        int? estadoId = null,
        CancellationToken ct = default)
    {
        var (items, total) = await _repo.GetPagedAsync(page, pageSize, socioId, estadoId, ct);

        var dtos = items.Select(o => new OrdenPagoDto
        {
            Id = o.Id,
            SocioId = o.SocioId,
            SocioNombre = o.Socio?.Nombre,
            PlanId = o.PlanId,
            PlanNombre = o.Plan?.Nombre,
            EstadoId = o.EstadoId,
            EstadoNombre = o.Estado?.Nombre,
            Monto = o.Monto,
            CreatedAt = o.CreatedAt,
            VenceEn = o.VenceEn,
            Notas = o.Notas,
            ComprobanteId = o.ComprobanteId,
            ComprobanteUrl = o.Comprobante?.FileUrl
        }).ToList();

        return Ok(new PaginatedResult<OrdenPagoDto>
        {
            Items = dtos,
            TotalItems = total,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var orden = await _repo.GetWithDetailsAsync(id, ct);
        if (orden == null)
            return NotFound(new { message = "Orden de pago no encontrada." });

        return Ok(new OrdenPagoDto
        {
            Id = orden.Id,
            SocioId = orden.SocioId,
            SocioNombre = orden.Socio?.Nombre,
            PlanId = orden.PlanId,
            PlanNombre = orden.Plan?.Nombre,
            EstadoId = orden.EstadoId,
            EstadoNombre = orden.Estado?.Nombre,
            Monto = orden.Monto,
            CreatedAt = orden.CreatedAt,
            VenceEn = orden.VenceEn,
            Notas = orden.Notas,
            ComprobanteId = orden.ComprobanteId,
            ComprobanteUrl = orden.Comprobante?.FileUrl
        });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrdenPagoCreateRequest request, CancellationToken ct)
    {
        var plan = await _db.Planes.FindAsync(new object[] { request.PlanId }, ct);
        if (plan == null)
            return BadRequest(new { message = "Plan no encontrado." });

        var orden = new OrdenPago
        {
            SocioId = request.SocioId,
            PlanId = request.PlanId,
            EstadoId = request.EstadoId,
            Monto = plan.Precio,
            VenceEn = request.FechaInicio.AddMonths(1),
            Notas = request.Notas
        };

        await _repo.AddAsync(orden, ct);

        return Ok(new OrdenPagoDto
        {
            Id = orden.Id,
            SocioId = orden.SocioId,
            PlanId = orden.PlanId,
            EstadoId = orden.EstadoId,
            Monto = orden.Monto,
            CreatedAt = orden.CreatedAt,
            VenceEn = orden.VenceEn,
            Notas = orden.Notas
        });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpPut("{id:int}/estado")]
    public async Task<IActionResult> UpdateEstado(int id, [FromBody] OrdenPagoUpdateEstadoRequest request, CancellationToken ct)
    {
        var orden = await _repo.GetByIdAsync(id, ct);
        if (orden == null)
            return NotFound(new { message = "Orden de pago no encontrada." });

        orden.EstadoId = request.EstadoId;
        if (!string.IsNullOrWhiteSpace(request.Notas))
            orden.Notas = request.Notas;

        await _repo.UpdateAsync(orden, ct);

        // Si se aprueba la orden, crear suscripción automáticamente
        if (request.EstadoId == 2) // Aprobada
        {
            var plan = await _db.Planes.FindAsync(new object[] { orden.PlanId }, ct);
            if (plan != null)
            {
                var suscripcion = new Suscripcion
                {
                    SocioId = orden.SocioId,
                    PlanId = orden.PlanId,
                    OrdenPagoId = orden.Id,
                    Inicio = DateTime.UtcNow,
                    Fin = DateTime.UtcNow.AddMonths(1),
                    Estado = true,
                    TenantId = orden.TenantId
                };

                _db.Suscripciones.Add(suscripcion);
                await _db.SaveChangesAsync(ct);
            }
        }

        return Ok(new { message = "Estado de orden actualizado correctamente." });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpPost("{id:int}/aprobar")]
    public async Task<IActionResult> Aprobar(int id, CancellationToken ct)
    {
        return await UpdateEstado(id, new OrdenPagoUpdateEstadoRequest { EstadoId = 2 }, ct);
    }
}
