using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Contracts;

namespace Api.Controllers;

[Authorize(Roles = "Administrador, Profesor, Recepcionista")]
[ApiController]
[Route("api/socios")]
public class SociosController : ControllerBase
{
    private readonly ISocioRepository _repo;
    private readonly GymDbContext _db;

    public SociosController(ISocioRepository repo, GymDbContext db)
    {
        _repo = repo;
        _db = db;
    }

    // ✅ GET /api/socios?page=1&pageSize=10&q=roma&activo=true
    [HttpGet]
    public async Task<IActionResult> Get(
        int page = 1,
        int pageSize = 10,
        string? q = null,
        bool? activo = null,
        CancellationToken ct = default)
    {
        // 👇 cambio principal: usar _repo (no _repoSocio)
        var (items, total) = await _repo.GetPagedAsync(page, pageSize, q, activo, ct);

        return Ok(new
        {
            total,
            page,
            pageSize,
            items
        });
    }

    // ✅ GET /api/socios/5
    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var s = await _repo.GetByIdAsync(id, ct);
        if (s is null) return NotFound();

        var dto = new SocioListItemDto(
            s.Id,
            s.Dni,
            s.Nombre,
            s.Email,
            s.Telefono,
            s.Activo,
            s.CreadoEn,
            s.FechaNacimiento,
            s.PlanActual // 👈 muestra plan actual si lo tiene
        );

        return Ok(dto);
    }

    // ✅ POST /api/socios
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SocioCreateDto body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.Dni) ||
            string.IsNullOrWhiteSpace(body.Nombre) ||
            string.IsNullOrWhiteSpace(body.Email))
            return BadRequest("Dni, Nombre y Email son obligatorios.");

        if (await _repo.ExistsAsync(body.Dni, body.Email, ct))
            return Conflict("DNI o email ya existen.");

        if (body.FechaNacimiento.HasValue && body.FechaNacimiento > DateTime.UtcNow)
            return BadRequest("La fecha de nacimiento no puede ser futura.");

        var entity = new Socio
        {
            Dni = body.Dni.Trim(),
            Nombre = body.Nombre.Trim(),
            Email = body.Email.Trim(),
            Telefono = string.IsNullOrWhiteSpace(body.Telefono) ? null : body.Telefono.Trim(),
            FechaNacimiento = body.FechaNacimiento,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };

        try
        {
            var created = await _repo.AddAsync(entity, ct);
            var dto = new SocioListItemDto(
                created.Id,
                created.Dni,
                created.Nombre,
                created.Email,
                created.Telefono,
                created.Activo,
                created.CreadoEn,
                created.FechaNacimiento,
                created.PlanActual
            );
            return Ok(dto);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("Duplicate", StringComparison.OrdinalIgnoreCase) == true)
        {
            return Conflict("DNI o email ya existen.");
        }
    }

    // ✅ PUT /api/socios/5
    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Put([FromRoute] int id, [FromBody] SocioUpdateDto body, CancellationToken ct)
    {
        if (body.FechaNacimiento.HasValue && body.FechaNacimiento > DateTime.UtcNow)
            return BadRequest("La fecha de nacimiento no puede ser futura.");

        // 👇 UpdateAsync ahora devuelve bool, no void
        var ok = await _repo.UpdateAsync(id, s =>
        {
            if (!string.IsNullOrWhiteSpace(body.Nombre)) s.Nombre = body.Nombre.Trim();
            if (!string.IsNullOrWhiteSpace(body.Email)) s.Email = body.Email.Trim();
            s.Telefono = string.IsNullOrWhiteSpace(body.Telefono) ? null : body.Telefono!.Trim();
            s.Activo = body.Activo;
            s.FechaNacimiento = body.FechaNacimiento;
        }, ct);

        if (!ok) return NotFound();

        var updated = await _repo.GetByIdAsync(id, ct);
        if (updated is null) return NotFound();

        var dto = new SocioListItemDto(
            updated.Id,
            updated.Dni,
            updated.Nombre,
            updated.Email,
            updated.Telefono,
            updated.Activo,
            updated.CreadoEn,
            updated.FechaNacimiento,
            updated.PlanActual
        );

        return Ok(dto);
    }

    // ✅ PATCH /api/socios/5/bajaLogica?value=false
    [HttpPatch("{id:int:min(1)}/bajaLogica")]
    public async Task<IActionResult> BajaLogica([FromRoute] int id, [FromQuery] bool value = true, CancellationToken ct = default)
    {
        var ok = await _repo.SetActivoAsync(id, value, ct);
        return ok ? NoContent() : NotFound();
    }

    // ✅ Buscar por nombre o dni (sin cambios)
    [HttpGet("buscar")]
    public async Task<IActionResult> BuscarPorNombreYDni(
        [FromQuery] string? nombre = null,
        [FromQuery] string? dni = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(nombre) && string.IsNullOrWhiteSpace(dni))
            return BadRequest("Debe especificar al menos un criterio: nombre o dni.");

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _db.Socios.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(s => EF.Functions.Like(s.Nombre, $"%{nombre}%"));

        if (!string.IsNullOrWhiteSpace(dni))
            query = query.Where(s => EF.Functions.Like(s.Dni, $"%{dni}%"));

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(s => s.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SocioListItemDto(
                s.Id,
                s.Dni,
                s.Nombre,
                s.Email,
                s.Telefono,
                s.Activo,
                s.CreadoEn,
                s.FechaNacimiento,
                s.PlanActual
            ))
            .ToListAsync(ct);

        return Ok(new { total, page, pageSize, items });
    }

    // ✅ Cumpleaños del mes (sin cambios)
    [HttpGet("cumpleanios")]
    public async Task<IActionResult> GetCumpleaniosMes([FromQuery] int? mes = null, CancellationToken ct = default)
    {
        int mesActual = mes ?? DateTime.UtcNow.Month;
        if (mesActual < 1 || mesActual > 12)
            return BadRequest("El mes debe estar entre 1 y 12.");

        var socios = await _db.Socios
            .AsNoTracking()
            .Where(s => s.FechaNacimiento.HasValue && s.FechaNacimiento.Value.Month == mesActual)
            .OrderBy(s => s.FechaNacimiento.Value.Day)
            .Select(s => new
            {
                s.Id,
                s.Nombre,
                s.Email,
                s.Telefono,
                FechaNacimiento = s.FechaNacimiento,
                Dia = s.FechaNacimiento!.Value.Day,
                Edad = DateTime.UtcNow.Year - s.FechaNacimiento!.Value.Year -
                        (DateTime.UtcNow.DayOfYear < s.FechaNacimiento!.Value.DayOfYear ? 1 : 0)
            })
            .ToListAsync(ct);

        return Ok(new { mes = mesActual, total = socios.Count, socios });
    }

    // ✅ Socios por plan (sin cambios)
    [HttpGet("por-plan/{planId:int:min(1)}")]
    public async Task<IActionResult> GetByPlanPaginado(
        [FromRoute] int planId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? activo = true,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _db.Suscripciones
            .AsNoTracking()
            .Include(s => s.Socio)
            .Include(s => s.Plan)
            .Where(s => s.PlanId == planId);

        if (activo.HasValue)
            query = query.Where(s => s.Estado == activo.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(s => s.CreadoEn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new
            {
                s.Id,
                Socio = s.Socio != null ? s.Socio.Nombre : null,
                Plan = s.Plan != null ? s.Plan.Nombre : null,
                s.Inicio,
                s.Fin,
                s.Estado,
                s.CreadoEn
            })
            .ToListAsync(ct);

        if (!items.Any())
            return NotFound($"No hay socios asociados al plan con ID {planId}.");

        return Ok(new { planId, activo, total, page, pageSize, items });
    }
}
