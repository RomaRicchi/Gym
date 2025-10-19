using Microsoft.AspNetCore.Mvc;
using Api.Repositories.Interfaces;
using Api.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize(Roles = "Administrador, Profesor, Recepcionista")]
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
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? q = null,
        [FromQuery] int[]? dias = null,
        [FromQuery] bool? activo = null,
        CancellationToken ct = default)
    {
        if (page <= 0 || pageSize <= 0)
            return BadRequest("page y pageSize deben ser > 0.");

        var (items, total) = await _repo.GetPagedAsync(page, pageSize, q, dias, activo, ct);

        // 🔹 reconstruimos para incluir DiasPorSemana
        var clean = items.Select(p => new
        {
            p.Id,
            p.Nombre,
            p.DiasPorSemana,   
            p.Precio,
            p.Activo
        });

        return Ok(new { total, page, pageSize, items = clean });
    }


    // GET /api/planes/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var plan = await _repo.GetAsync(id, ct);
        if (plan == null)
            return NotFound();

        return Ok(plan);
    }

    // DTO para creación
    public record PlanCreateDto(string Nombre, int DiasPorSemana, decimal Precio, bool Activo = true);

    // POST /api/planes
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PlanCreateDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return BadRequest("El nombre es obligatorio.");

        if (!new[] { 2, 3, 5 }.Contains(dto.DiasPorSemana))
            return BadRequest("DiasPorSemana debe ser 2, 3 o 5.");

        if (dto.Precio < 0)
            return BadRequest("El precio no puede ser negativo.");

        if (await _repo.ExistsByNameAsync(dto.Nombre, null, ct))
            return Conflict("Ya existe un plan con ese nombre.");

        var entity = new Plan
        {
            Nombre = dto.Nombre.Trim(),
            DiasPorSemana = dto.DiasPorSemana,
            Precio = dto.Precio,
            Activo = dto.Activo
        };

        var created = await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // DTO para actualización
    public record PlanUpdateDto(string? Nombre, int? DiasPorSemana, decimal? Precio, bool? Activo);

    // PUT /api/planes/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PlanUpdateDto dto, CancellationToken ct)
    {
        var plan = await _repo.GetAsync(id, ct);
        if (plan == null)
            return NotFound();

        if (dto.DiasPorSemana.HasValue && !new[] { 2, 3, 5 }.Contains(dto.DiasPorSemana.Value))
            return BadRequest("DiasPorSemana debe ser 2, 3 o 5.");

        if (dto.Precio is < 0)
            return BadRequest("El precio no puede ser negativo.");

        if (!string.IsNullOrWhiteSpace(dto.Nombre) &&
            await _repo.ExistsByNameAsync(dto.Nombre!, id, ct))
            return Conflict("Ya existe un plan con ese nombre.");

        if (dto.Nombre is not null) plan.Nombre = dto.Nombre.Trim();
        if (dto.DiasPorSemana.HasValue) plan.DiasPorSemana = dto.DiasPorSemana.Value;
        if (dto.Precio.HasValue) plan.Precio = dto.Precio.Value;
        if (dto.Activo.HasValue) plan.Activo = dto.Activo.Value;

        var ok = await _repo.UpdateAsync(plan, ct);
        return ok ? NoContent() : StatusCode(500, "No se pudo actualizar el plan.");
    }

    // DELETE /api/planes/5  → Baja lógica
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var plan = await _repo.GetAsync(id, ct);
        if (plan == null)
            return NotFound();

        plan.Activo = false;
        var ok = await _repo.UpdateAsync(plan, ct);

        return ok ? NoContent() : StatusCode(500, "No se pudo dar de baja el plan.");
    }

    [HttpGet("buscar")]
    public async Task<IActionResult> BuscarPorNombre(
        [FromQuery] string? nombre = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return BadRequest("Debe especificar un nombre para buscar.");

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        // 🔎 Búsqueda parcial insensible a mayúsculas
        var query = await _repo.GetPagedAsync(page, pageSize, nombre, null, null, ct);

        // DTO de salida limpio
        var result = new
        {
            total = query.total,
            page,
            pageSize,
            items = query.items.Select(p => new
            {
                p.Id,
                p.Nombre,
                p.DiasPorSemana,
                p.Precio,
                p.Activo
            })
        };

        return Ok(result);
    }
}
