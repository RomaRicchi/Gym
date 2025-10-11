using Microsoft.AspNetCore.Mvc;
using Api.Repositories.Interfaces;
using Api.Data.Models;

namespace Api.Controllers;

[ApiController]
[Route("api/planes")]
public class PlanesController : ControllerBase
{
    private readonly IPlanRepository _repo;
    public PlanesController(IPlanRepository repo) => _repo = repo;

    // GET /api/planes?page=1&pageSize=20&q=gym&dias=2&dias=3&activo=true
    [HttpGet]
    public async Task<IActionResult> Get(
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
        return Ok(new { total, page, pageSize, items });
    }

    // GET /api/planes/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var entity = await _repo.GetAsync((uint)id, ct);
        return entity is null ? NotFound() : Ok(entity);
    }

    public record PlanCreateDto(string Nombre, int DiasPorSemana, decimal Precio, bool? Activo = true);

    // POST /api/planes
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PlanCreateDto dto, CancellationToken ct)
    {
        if (!new[] { 2, 3, 5 }.Contains(dto.DiasPorSemana))
            return BadRequest("DiasPorSemana debe ser 2, 3 o 5.");
        if (dto.Precio < 0)
            return BadRequest("Precio no puede ser negativo.");
        if (await _repo.ExistsByNameAsync(dto.Nombre, null, ct))
            return Conflict("Ya existe un plan con ese nombre.");

        var entity = new Plan
        {
            Nombre = dto.Nombre,
            DiasPorSemana = dto.DiasPorSemana,
            Precio = dto.Precio,
            Activo = dto.Activo ?? true
        };

        var created = await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    public record PlanUpdateDto(string? Nombre, int? DiasPorSemana, decimal? Precio, bool? Activo);

    // PUT /api/planes/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PlanUpdateDto dto, CancellationToken ct)
    {
        var entity = await _repo.GetAsync((uint)id, ct);
        if (entity is null)
            return NotFound();

        if (dto.DiasPorSemana.HasValue && !new[] { 2, 3, 5 }.Contains(dto.DiasPorSemana.Value))
            return BadRequest("DiasPorSemana debe ser 2, 3 o 5.");
        if (dto.Precio is < 0)
            return BadRequest("Precio no puede ser negativo.");
        if (!string.IsNullOrWhiteSpace(dto.Nombre) &&
            await _repo.ExistsByNameAsync(dto.Nombre!, (uint)id, ct))
            return Conflict("Ya existe un plan con ese nombre.");

        if (dto.Nombre is not null) entity.Nombre = dto.Nombre;
        if (dto.DiasPorSemana.HasValue) entity.DiasPorSemana = dto.DiasPorSemana.Value;
        if (dto.Precio.HasValue) entity.Precio = dto.Precio.Value;
        if (dto.Activo.HasValue) entity.Activo = dto.Activo;

        var ok = await _repo.UpdateAsync(entity, ct);
        return ok ? NoContent() : StatusCode(500, "No se pudo actualizar el plan.");
    }

    // DELETE /api/planes/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _repo.DeleteAsync((uint)id, ct);
        return ok ? NoContent() : NotFound();
    }
}
