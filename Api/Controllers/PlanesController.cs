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
        if (page <= 0 || pageSize <= 0) return BadRequest("page y pageSize deben ser > 0.");
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

    public record PlanCreateDto(string nombre, int dias_por_semana, decimal precio, bool? activo = true);

    // POST /api/planes
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PlanCreateDto dto, CancellationToken ct)
    {
        if (!new[] { 2, 3, 5 }.Contains(dto.dias_por_semana))
            return BadRequest("dias_por_semana debe ser 2, 3 o 5.");
        if (dto.precio < 0) return BadRequest("precio no puede ser negativo.");
        if (await _repo.ExistsByNameAsync(dto.nombre, null, ct)) return Conflict("Ya existe un plan con ese nombre.");

        var entity = new plan
        {
            nombre = dto.nombre,
            dias_por_semana = dto.dias_por_semana,
            precio = dto.precio,
            activo = dto.activo ?? true
        };

        var created = await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.id }, created);
    }

    public record PlanUpdateDto(string? nombre, int? dias_por_semana, decimal? precio, bool? activo);

    // PUT /api/planes/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PlanUpdateDto dto, CancellationToken ct)
    {
        var entity = await _repo.GetAsync((uint)id, ct);
        if (entity is null) return NotFound();

        if (dto.dias_por_semana.HasValue && !new[] { 2, 3, 5 }.Contains(dto.dias_por_semana.Value))
            return BadRequest("dias_por_semana debe ser 2, 3 o 5.");
        if (dto.precio is < 0) return BadRequest("precio no puede ser negativo.");
        if (!string.IsNullOrWhiteSpace(dto.nombre) &&
            await _repo.ExistsByNameAsync(dto.nombre!, (uint)id, ct))
            return Conflict("Ya existe un plan con ese nombre.");

        if (dto.nombre is not null) entity.nombre = dto.nombre;
        if (dto.dias_por_semana.HasValue) entity.dias_por_semana = dto.dias_por_semana.Value;
        if (dto.precio.HasValue) entity.precio = dto.precio.Value;
        if (dto.activo.HasValue) entity.activo = dto.activo;

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
