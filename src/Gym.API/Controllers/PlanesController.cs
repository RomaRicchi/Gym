using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gym.Application.DTOs.Planes;
using Gym.Domain.Entities;
using Gym.Domain.Interfaces;

namespace Gym.API.Controllers;

[Authorize]
[ApiController]
[Route("api/planes")]
public class PlanesController : ControllerBase
{
    private readonly IPlanRepository _repo;

    public PlanesController(IPlanRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var planes = await _repo.GetAllAsync(ct);

        var dtos = planes.Select(p => new PlanDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            DiasPorSemana = p.DiasPorSemana,
            Precio = p.Precio,
            Activo = p.Activo,
            CreatedAt = p.CreatedAt
        }).ToList();

        return Ok(dtos);
    }

    [HttpGet("activos")]
    public async Task<IActionResult> GetActivos(CancellationToken ct)
    {
        var planes = await _repo.GetActivosAsync(ct);

        var dtos = planes.Select(p => new PlanDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            DiasPorSemana = p.DiasPorSemana,
            Precio = p.Precio,
            Activo = p.Activo,
            CreatedAt = p.CreatedAt
        }).ToList();

        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var plan = await _repo.GetByIdAsync(id, ct);
        if (plan == null)
            return NotFound(new { message = "Plan no encontrado." });

        return Ok(new PlanDto
        {
            Id = plan.Id,
            Nombre = plan.Nombre,
            DiasPorSemana = plan.DiasPorSemana,
            Precio = plan.Precio,
            Activo = plan.Activo,
            CreatedAt = plan.CreatedAt
        });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PlanCreateRequest request, CancellationToken ct)
    {
        var plan = new Plan
        {
            Nombre = request.Nombre,
            DiasPorSemana = request.DiasPorSemana,
            Precio = request.Precio,
            Activo = request.Activo
        };

        await _repo.AddAsync(plan, ct);

        return Ok(new PlanDto
        {
            Id = plan.Id,
            Nombre = plan.Nombre,
            DiasPorSemana = plan.DiasPorSemana,
            Precio = plan.Precio,
            Activo = plan.Activo,
            CreatedAt = plan.CreatedAt
        });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PlanUpdateRequest request, CancellationToken ct)
    {
        var plan = await _repo.GetByIdAsync(id, ct);
        if (plan == null)
            return NotFound(new { message = "Plan no encontrado." });

        plan.Nombre = request.Nombre;
        plan.DiasPorSemana = request.DiasPorSemana;
        plan.Precio = request.Precio;
        plan.Activo = request.Activo;

        await _repo.UpdateAsync(plan, ct);

        return Ok(new { message = "Plan actualizado correctamente." });
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var plan = await _repo.GetByIdAsync(id, ct);
        if (plan == null)
            return NotFound(new { message = "Plan no encontrado." });

        plan.Activo = false;
        await _repo.UpdateAsync(plan, ct);

        return Ok(new { message = "Plan desactivado correctamente." });
    }
}
