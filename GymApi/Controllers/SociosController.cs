using Microsoft.AspNetCore.Mvc;
using GymApi.Repositories;
using GymApi.Data.Models;

namespace GymApi.Controllers;

[ApiController]
[Route("api/socios")]
public class SociosController : ControllerBase
{
    private readonly ISocioRepository _repo;
    public SociosController(ISocioRepository repo) => _repo = repo;

    // GET /api/socios?page=1&pageSize=20&q=roma&activo=true
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? q = null,
        [FromQuery] bool? activo = null,
        CancellationToken ct = default)
    {
        if (page <= 0 || pageSize <= 0) return BadRequest("page y pageSize deben ser > 0.");
        var (items, total) = await _repo.GetPagedAsync(page, pageSize, q, activo, ct);
        return Ok(new { total, page, pageSize, items });
    }

    // GET /api/socios/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        // Si querés un repo GetById, agregalo al ISocioRepository
        // Por ahora, salida rápida usando el paginado:
        var (items, _) = await _repo.GetPagedAsync(1, 1, null, null, ct);
        var socio = items.FirstOrDefault(s => s.id == (uint)id);
        return socio is null ? NotFound() : Ok(socio);
    }

    public record SocioCreateDto(string dni, string nombre, string email, string? telefono);

    // POST /api/socios
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SocioCreateDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.dni) || string.IsNullOrWhiteSpace(dto.nombre) || string.IsNullOrWhiteSpace(dto.email))
            return BadRequest("dni, nombre y email son obligatorios.");

        var exists = await _repo.ExistsAsync(dto.dni, dto.email, ct);
        if (exists) return Conflict("Ya existe un socio con ese DNI o email.");

        var entity = new socio
        {
            dni = dto.dni,
            nombre = dto.nombre,
            email = dto.email,
            telefono = dto.telefono,
            activo = true
        };

        var created = await _repo.AddAsync(entity, ct);
        // Si querés CreatedAt con GetById real, agregale GetById al repo
        return Created($"/api/socios/{created.id}", created);
    }
}
