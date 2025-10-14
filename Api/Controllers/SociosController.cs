using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Repositories.Interfaces;
using Api.Data.Models;
using Api.Contracts;

namespace Api.Controllers;

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
            s.Id, s.Dni, s.Nombre, s.Email, s.Telefono, s.Activo, s.CreadoEn));

        return Ok(new { total, page, pageSize, items = dto });
    }

    // GET /api/socios/5
    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var s = await _repo.GetByIdAsync((int)id, ct);
        if (s is null) return NotFound();

        var dto = new SocioListItemDto(s.Id, s.Dni, s.Nombre, s.Email, s.Telefono, s.Activo, s.CreadoEn);
        return Ok(dto);
    }

    // POST /api/socios
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SocioCreateDto body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.Dni) ||
            string.IsNullOrWhiteSpace(body.Nombre) ||
            string.IsNullOrWhiteSpace(body.Email))
            return BadRequest("Dni, Nombre y Email son obligatorios");

        if (await _repo.ExistsAsync(body.Dni, body.Email, ct))
            return Conflict("DNI o email ya existen");

        var entity = new Socio
        {
            Dni = body.Dni.Trim(),
            Nombre = body.Nombre.Trim(),
            Email = body.Email.Trim(),
            Telefono = string.IsNullOrWhiteSpace(body.Telefono) ? null : body.Telefono.Trim(),
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };

        try
        {
            var created = await _repo.AddAsync(entity, ct);
            var dto = new SocioListItemDto(
                created.Id, created.Dni, created.Nombre, created.Email,
                created.Telefono, created.Activo, created.CreadoEn);
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
        var ok = await _repo.UpdateAsync((int)id, s =>
        {
            if (!string.IsNullOrWhiteSpace(body.Nombre)) s.Nombre = body.Nombre.Trim();
            if (!string.IsNullOrWhiteSpace(body.Email)) s.Email = body.Email.Trim();
            s.Telefono = string.IsNullOrWhiteSpace(body.Telefono) ? null : body.Telefono!.Trim();
            s.Activo = body.Activo;
        }, ct);

        if (!ok) return NotFound();

        var updated = await _repo.GetByIdAsync((int)id, ct)!;
        var dto = new SocioListItemDto(
            updated!.Id, updated.Dni, updated.Nombre, updated.Email,
            updated.Telefono, updated.Activo, updated.CreadoEn);
        return Ok(dto);
    }

    // PATCH /api/socios/5/activar?value=false   (baja lógica)
    [HttpPatch("{id:int:min(1)}/activar")]
    public async Task<IActionResult> Activar([FromRoute] int id, [FromQuery] bool value = true, CancellationToken ct = default)
    {
        var ok = await _repo.SetActivoAsync((int)id, value, ct);
        return ok ? NoContent() : NotFound();
    }

    // DELETE /api/socios/5?hard=true            (baja física opcional)
    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> Delete([FromRoute] int id, [FromQuery] bool hard = false, CancellationToken ct = default)
    {
        if (hard)
        {
            var ok = await _repo.DeleteAsync((int)id, ct);
            return ok ? NoContent() : NotFound();
        }
        else
        {
            var ok = await _repo.SetActivoAsync((int)id, false, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
