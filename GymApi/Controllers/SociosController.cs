using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymApi.Repositories;
using GymApi.Data.Models;
using GymApi.Contracts;

namespace GymApi.Controllers;

[ApiController]
[Route("api/socios")]
public class SociosController : ControllerBase
{
    private readonly ISocioRepository _repo;
    public SociosController(ISocioRepository repo) => _repo = repo;

    // GET /api/socios?page=1&pageSize=10&q=roma&activo=true
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? q = null,
        [FromQuery] bool? activo = null,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var (items, total) = await _repo.GetPagedAsync(page, pageSize, q, activo, ct);

        var dto = items.Select(s => new SocioListItemDto(
            s.id, s.dni, s.nombre, s.email, s.telefono, s.activo, s.creado_en));

        return Ok(new { total, page, pageSize, items = dto });
    }

    // GET /api/socios/5
    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var s = await _repo.GetByIdAsync((uint)id, ct);
        if (s is null) return NotFound();

        var dto = new SocioListItemDto(s.id, s.dni, s.nombre, s.email, s.telefono, s.activo, s.creado_en);
        return Ok(dto);
    }

    // POST /api/socios
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SocioCreateDto body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.dni) ||
            string.IsNullOrWhiteSpace(body.nombre) ||
            string.IsNullOrWhiteSpace(body.email))
            return BadRequest("dni, nombre y email son obligatorios");

        if (await _repo.ExistsAsync(body.dni, body.email, ct))
            return Conflict("DNI o email ya existen");

        var entity = new socio
        {
            dni = body.dni.Trim(),
            nombre = body.nombre.Trim(),
            email = body.email.Trim(),
            telefono = string.IsNullOrWhiteSpace(body.telefono) ? null : body.telefono.Trim(),
            activo = true
        };

        try
        {
            var created = await _repo.AddAsync(entity, ct);
            var dto = new SocioListItemDto(
                created.id, created.dni, created.nombre, created.email,
                created.telefono, created.activo, created.creado_en);
            return Ok(dto);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("Duplicate", StringComparison.OrdinalIgnoreCase) == true)
        {
            return Conflict("DNI o email ya existen");
        }
    }

    // PUT /api/socios/5
    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Put([FromRoute] int id, [FromBody] SocioUpdateDto body, CancellationToken ct)
    {
        var ok = await _repo.UpdateAsync((uint)id, s =>
        {
            if (!string.IsNullOrWhiteSpace(body.nombre)) s.nombre = body.nombre.Trim();
            if (!string.IsNullOrWhiteSpace(body.email))  s.email  = body.email.Trim();
            s.telefono = string.IsNullOrWhiteSpace(body.telefono) ? null : body.telefono!.Trim();
            s.activo   = body.activo;
        }, ct);

        if (!ok) return NotFound();

        var updated = await _repo.GetByIdAsync((uint)id, ct)!;
        var dto = new SocioListItemDto(
            updated!.id, updated.dni, updated.nombre, updated.email,
            updated.telefono, updated.activo, updated.creado_en);
        return Ok(dto);
    }

    // PATCH /api/socios/5/activar?value=false   (baja lógica)
    [HttpPatch("{id:int:min(1)}/activar")]
    public async Task<IActionResult> Activar([FromRoute] int id, [FromQuery] bool value = true, CancellationToken ct = default)
    {
        var ok = await _repo.SetActivoAsync((uint)id, value, ct);
        return ok ? NoContent() : NotFound();
    }

    // DELETE /api/socios/5?hard=true            (baja física opcional)
    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> Delete([FromRoute] int id, [FromQuery] bool hard = false, CancellationToken ct = default)
    {
        if (hard)
        {
            var ok = await _repo.DeleteAsync((uint)id, ct);
            return ok ? NoContent() : NotFound();
        }
        else
        {
            var ok = await _repo.SetActivoAsync((uint)id, false, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
