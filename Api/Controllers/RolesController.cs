using Microsoft.AspNetCore.Mvc;
using Api.Repositories.Interfaces;
using Api.Data.Models;

namespace Api.Controllers;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly IRolRepository _repo;
    public RolesController(IRolRepository repo) => _repo = repo;

    // GET /api/roles
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var roles = await _repo.GetAllAsync(ct);
        return Ok(roles);
    }

    // GET /api/roles/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var rol = await _repo.GetByIdAsync(id, ct);
        return rol is null ? NotFound() : Ok(rol);
    }

    public record RolCreateDto(string Nombre);

    // POST /api/roles
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RolCreateDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return BadRequest("El nombre es obligatorio.");

        var entity = new Rol { Nombre = dto.Nombre.Trim() };
        var created = await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    public record RolUpdateDto(string Nombre);

    // PUT /api/roles/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RolUpdateDto dto, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing == null)
            return NotFound();

        existing.Nombre = dto.Nombre.Trim();
        var updated = await _repo.UpdateAsync(id, existing, ct);
        return Ok(updated);
    }

    // DELETE /api/roles/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _repo.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
