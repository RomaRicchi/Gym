using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gym.Application.DTOs.Suscripciones;
using Gym.Application.DTOs.Common;
using Gym.Domain.Entities;
using Gym.Domain.Interfaces;

namespace Gym.API.Controllers;

[Authorize]
[ApiController]
[Route("api/suscripciones")]
public class SuscripcionesController : ControllerBase
{
    private readonly ISuscripcionRepository _repo;

    public SuscripcionesController(ISuscripcionRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1,
        int pageSize = 10,
        int? socioId = null,
        bool? activo = null,
        CancellationToken ct = default)
    {
        var (items, total) = await _repo.GetPagedAsync(page, pageSize, socioId, activo, ct);

        var dtos = items.Select(s => new SuscripcionDto
        {
            Id = s.Id,
            SocioId = s.SocioId,
            SocioNombre = s.Socio?.Nombre,
            PlanId = s.PlanId,
            PlanNombre = s.Plan?.Nombre,
            Inicio = s.Inicio,
            Fin = s.Fin,
            Estado = s.Estado,
            OrdenPagoId = s.OrdenPagoId,
            CreatedAt = s.CreatedAt
        }).ToList();

        return Ok(new PaginatedResult<SuscripcionDto>
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
        var suscripcion = await _repo.GetByIdAsync(id, ct);
        if (suscripcion == null)
            return NotFound(new { message = "Suscripción no encontrada." });

        return Ok(new SuscripcionDto
        {
            Id = suscripcion.Id,
            SocioId = suscripcion.SocioId,
            SocioNombre = suscripcion.Socio?.Nombre,
            PlanId = suscripcion.PlanId,
            PlanNombre = suscripcion.Plan?.Nombre,
            Inicio = suscripcion.Inicio,
            Fin = suscripcion.Fin,
            Estado = suscripcion.Estado,
            OrdenPagoId = suscripcion.OrdenPagoId,
            CreatedAt = suscripcion.CreatedAt
        });
    }

    [HttpGet("socio/{socioId:int}/activa")]
    public async Task<IActionResult> GetActivaBySocio(int socioId, CancellationToken ct)
    {
        var suscripcion = await _repo.GetActivaBySocioAsync(socioId, ct);
        if (suscripcion == null)
            return NotFound(new { message = "No hay suscripción activa para este socio." });

        return Ok(new SuscripcionDto
        {
            Id = suscripcion.Id,
            SocioId = suscripcion.SocioId,
            PlanId = suscripcion.PlanId,
            PlanNombre = suscripcion.Plan?.Nombre,
            Inicio = suscripcion.Inicio,
            Fin = suscripcion.Fin,
            Estado = suscripcion.Estado,
            CreatedAt = suscripcion.CreatedAt
        });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SuscripcionCreateRequest request, CancellationToken ct)
    {
        var suscripcion = new Suscripcion
        {
            SocioId = request.SocioId,
            PlanId = request.PlanId,
            OrdenPagoId = request.OrdenPagoId,
            Inicio = request.Inicio,
            Fin = request.Fin,
            Estado = true
        };

        await _repo.AddAsync(suscripcion, ct);

        return Ok(new SuscripcionDto
        {
            Id = suscripcion.Id,
            SocioId = suscripcion.SocioId,
            PlanId = suscripcion.PlanId,
            Inicio = suscripcion.Inicio,
            Fin = suscripcion.Fin,
            Estado = suscripcion.Estado,
            OrdenPagoId = suscripcion.OrdenPagoId,
            CreatedAt = suscripcion.CreatedAt
        });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SuscripcionUpdateRequest request, CancellationToken ct)
    {
        var suscripcion = await _repo.GetByIdAsync(id, ct);
        if (suscripcion == null)
            return NotFound(new { message = "Suscripción no encontrada." });

        suscripcion.Inicio = request.Inicio;
        suscripcion.Fin = request.Fin;
        suscripcion.Estado = request.Estado;

        await _repo.UpdateAsync(suscripcion, ct);

        return Ok(new { message = "Suscripción actualizada correctamente." });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id:int}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromQuery] bool estado, CancellationToken ct)
    {
        var suscripcion = await _repo.GetByIdAsync(id, ct);
        if (suscripcion == null)
            return NotFound(new { message = "Suscripción no encontrada." });

        suscripcion.Estado = estado;
        await _repo.UpdateAsync(suscripcion, ct);

        return NoContent();
    }
}
