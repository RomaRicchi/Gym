using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Contracts;
using System.Security.Claims;
using Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize(Roles = "Administrador, Profesor, Recepción, Socio")]
[ApiController]
[Route("api/socios")]
public class SociosController : ControllerBase
{
    private readonly ISocioRepository _repo;
    private readonly GymDbContext _db;
    private readonly IEmailService _emailService;

    public SociosController(ISocioRepository repo, GymDbContext db, IEmailService emailService)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    // ================================
    // 🔹 GET /api/socios
    // ================================
    [HttpGet]
    public async Task<IActionResult> Get(
        int page = 1,
        int pageSize = 10,
        string? q = null,
        bool? activo = null,
        CancellationToken ct = default)
    {
        var query = _db.Socios
            .AsNoTracking()
            .OrderBy(s => s.Nombre)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(s =>
                EF.Functions.Like(s.Nombre, $"%{q}%") ||
                EF.Functions.Like(s.Dni, $"%{q}%"));

        if (activo.HasValue)
            query = query.Where(s => s.Activo == activo.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new
            {
                s.Id,
                s.Dni,
                s.Nombre,
                s.Email,
                s.Telefono,
                s.Activo,
                s.CreadoEn,
                s.FechaNacimiento,
                PlanActual = _db.Suscripciones
                    .Where(sub => sub.SocioId == s.Id && sub.Estado == true)
                    .OrderByDescending(sub => sub.CreadoEn)
                    .Select(sub => sub.Plan.Nombre)
                    .FirstOrDefault()
            })
            .ToListAsync(ct);

        return Ok(new
        {
            items,
            totalItems = total,
            totalPages = (int)Math.Ceiling((double)total / pageSize),
            page
        });
    }

    // ================================
    // 🔹 GET /api/socios/{id}
    // ================================
    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var s = await _repo.GetByIdAsync(id, ct);
        if (s is null) return NotFound();

        var dto = new SocioListItemDto(
            s.Id, s.Dni, s.Nombre, s.Email, s.Telefono,
            s.Activo, s.CreadoEn, s.FechaNacimiento, s.PlanActual
        );

        return Ok(dto);
    }

    // ================================
    // 🔹 POST /api/socios
    // ================================
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
                created.Id, created.Dni, created.Nombre, created.Email,
                created.Telefono, created.Activo, created.CreadoEn,
                created.FechaNacimiento, created.PlanActual
            );
            return Ok(dto);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("Duplicate", StringComparison.OrdinalIgnoreCase) == true)
        {
            return Conflict("DNI o email ya existen.");
        }
    }

    // ================================
    // 🔹 PUT /api/socios/{id}
    // ================================
    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> Put([FromRoute] int id, [FromBody] SocioUpdateDto body, CancellationToken ct)
    {
        if (body.FechaNacimiento.HasValue && body.FechaNacimiento > DateTime.UtcNow)
            return BadRequest("La fecha de nacimiento no puede ser futura.");

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
            updated.Id, updated.Dni, updated.Nombre, updated.Email,
            updated.Telefono, updated.Activo, updated.CreadoEn,
            updated.FechaNacimiento, updated.PlanActual
        );

        return Ok(dto);
    }

    // ================================
    // 🔹 PATCH /api/socios/{id}/bajaLogica
    // ================================
    [HttpPatch("{id:int:min(1)}/bajaLogica")]
    public async Task<IActionResult> BajaLogica([FromRoute] int id, [FromQuery] bool value = true, CancellationToken ct = default)
    {
        var ok = await _repo.SetActivoAsync(id, value, ct);
        return ok ? NoContent() : NotFound();
    }

    // ================================
    // 🔹 GET /api/socios/buscar
    // ================================
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
                s.Id, s.Dni, s.Nombre, s.Email,
                s.Telefono, s.Activo, s.CreadoEn,
                s.FechaNacimiento, s.PlanActual))
            .ToListAsync(ct);

        return Ok(new { total, page, pageSize, items });
    }

    // ================================
    // 🔹 GET /api/socios/cumpleanios
    // ================================
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
                Edad = DateTime.UtcNow.Year - s.FechaNacimiento.Value.Year -
                        (DateTime.UtcNow.DayOfYear < s.FechaNacimiento.Value.DayOfYear ? 1 : 0)
            })
            .ToListAsync(ct);

        return Ok(new { mes = mesActual, total = socios.Count, socios });
    }

    // ================================
    // 🔹 GET /api/socios/por-plan/{planId}
    // ================================
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
                Socio = s.Socio != null ? s.Socio.Nombre : "(Sin socio)",
                Plan = s.Plan != null ? s.Plan.Nombre : "(Sin plan)",
                s.Inicio,
                s.Fin,
                s.Estado,
                s.CreadoEn
            })
            .ToListAsync(ct);

        if (items.Count == 0)
            return NotFound($"No hay socios asociados al plan con ID {planId}.");

        return Ok(new { planId, activo, total, page, pageSize, items });
    }

    // ================================
    // 🔹 POST /api/socios/registro-publico
    // ================================
    [AllowAnonymous]
    [HttpPost("registro-publico")]
    public async Task<IActionResult> RegistroPublico([FromBody] SocioCreateDto body, CancellationToken ct)
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

        await _repo.AddAsync(entity, ct);

        try
        {
            await _emailService.SendEmailAsync(
                entity.Email,
                "¡Bienvenido a FitGym!",
                $"Hola {entity.Nombre},<br>🎉 Tu registro como socio fue exitoso.<br><br>" +
                "Ya podés crear tu cuenta de acceso para iniciar sesión en el sistema."
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando correo de bienvenida: {ex.Message}");
        }

        return Ok(new
        {
            message = "Socio registrado correctamente.",
            id = entity.Id,
            nombre = entity.Nombre,
            email = entity.Email
        });
    }

    // ================================
    // 🔹 GET /api/socios/perfil
    // ================================
    [HttpGet("perfil")]
    [Authorize(Roles = "Socio")]
    public async Task<IActionResult> GetPerfilSocio()
    {
        try
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idStr, out int idUsuario))
                return Unauthorized(new { message = "Token inválido o sin identificador." });

            var usuario = await _db.Usuarios
                .Include(u => u.Socio)
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Id == idUsuario);

            if (usuario?.SocioId is null)
                return NotFound(new { message = "No se encontró el socio vinculado al usuario." });

            var socio = await _db.Socios.FirstOrDefaultAsync(s => s.Id == usuario.SocioId);
            if (socio == null)
                return NotFound(new { message = "No se encontró el socio." });

            var dto = new
            {
                socio.Id,
                socio.Nombre,
                socio.Dni,
                socio.Telefono,
                socio.FechaNacimiento,
                socio.Activo,
                usuario = new
                {
                    usuario.Id,
                    usuario.Alias,
                    usuario.Email,
                    Avatar = usuario.Avatar?.Url
                }
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error al obtener el perfil del socio.", error = ex.Message });
        }
    }
}
