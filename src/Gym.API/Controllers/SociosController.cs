using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gym.Application.DTOs.Socios;
using Gym.Application.DTOs.Common;
using Gym.Application.Interfaces;
using Gym.Domain.Entities;
using Gym.Domain.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.API.Controllers;

[Authorize]
[ApiController]
[Route("api/socios")]
public class SociosController : ControllerBase
{
    private readonly ISocioRepository _repo;
    private readonly GymDbContext _db;
    private readonly IEmailService _emailService;

    public SociosController(ISocioRepository repo, GymDbContext db, IEmailService emailService)
    {
        _repo = repo;
        _db = db;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1,
        int pageSize = 10,
        string? q = null,
        bool? activo = null,
        CancellationToken ct = default)
    {
        var (items, total) = await _repo.GetPagedAsync(page, pageSize, q, activo, ct);

        var dtos = items.Select(s => new SocioDto(
            s.Id,
            s.Dni,
            s.Nombre,
            s.Email,
            s.Telefono,
            s.Activo,
            s.CreatedAt,
            s.FechaNacimiento,
            s.Suscripciones
                .Where(su => su.Estado && su.Inicio <= DateTime.UtcNow && su.Fin >= DateTime.UtcNow)
                .OrderByDescending(su => su.Inicio)
                .Select(su => su.Plan?.Nombre)
                .FirstOrDefault()
        )).ToList();

        return Ok(new PaginatedResult<SocioDto>
        {
            Items = dtos,
            TotalItems = total,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var socio = await _repo.GetWithSuscripcionesAsync(id, ct);
        if (socio == null)
            return NotFound(new { message = "Socio no encontrado." });

        var planActual = socio.Suscripciones
            .Where(su => su.Estado && su.Inicio <= DateTime.UtcNow && su.Fin >= DateTime.UtcNow)
            .OrderByDescending(su => su.Inicio)
            .Select(su => su.Plan?.Nombre)
            .FirstOrDefault();

        return Ok(new SocioDto(
            socio.Id,
            socio.Dni,
            socio.Nombre,
            socio.Email,
            socio.Telefono,
            socio.Activo,
            socio.CreatedAt,
            socio.FechaNacimiento,
            planActual
        ));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SocioCreateRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Dni) ||
            string.IsNullOrWhiteSpace(request.Nombre) ||
            string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = "Dni, Nombre y Email son obligatorios." });

        if (await _repo.ExistsAsync(request.Dni, request.Email, ct))
            return Conflict(new { message = "DNI o email ya existen." });

        var entity = new Socio
        {
            Dni = request.Dni.Trim(),
            Nombre = request.Nombre.Trim(),
            Email = request.Email.Trim(),
            Telefono = request.Telefono?.Trim(),
            FechaNacimiento = request.FechaNacimiento,
            Activo = true
        };

        var created = await _repo.AddAsync(entity, ct);

        return Ok(new SocioDto(
            created.Id,
            created.Dni,
            created.Nombre,
            created.Email,
            created.Telefono,
            created.Activo,
            created.CreatedAt,
            created.FechaNacimiento,
            null
        ));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SocioUpdateRequest request, CancellationToken ct)
    {
        var socio = await _repo.GetByIdAsync(id, ct);
        if (socio == null)
            return NotFound(new { message = "Socio no encontrado." });

        if (!string.IsNullOrWhiteSpace(request.Nombre))
            socio.Nombre = request.Nombre.Trim();
        if (!string.IsNullOrWhiteSpace(request.Email))
            socio.Email = request.Email.Trim();

        socio.Telefono = request.Telefono?.Trim();
        socio.Activo = request.Activo;
        socio.FechaNacimiento = request.FechaNacimiento;

        await _repo.UpdateAsync(socio, ct);

        return Ok(new { message = "Socio actualizado correctamente." });
    }

    [HttpPatch("{id:int}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromQuery] bool activo, CancellationToken ct)
    {
        var socio = await _repo.GetByIdAsync(id, ct);
        if (socio == null)
            return NotFound(new { message = "Socio no encontrado." });

        socio.Activo = activo;
        await _repo.UpdateAsync(socio, ct);

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("registro-publico")]
    public async Task<IActionResult> RegistroPublico([FromBody] SocioCreateRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Dni) ||
            string.IsNullOrWhiteSpace(request.Nombre) ||
            string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = "Dni, Nombre y Email son obligatorios." });

        if (await _repo.ExistsAsync(request.Dni, request.Email, ct))
            return Conflict(new { message = "DNI o email ya existen." });

        var entity = new Socio
        {
            Dni = request.Dni.Trim(),
            Nombre = request.Nombre.Trim(),
            Email = request.Email.Trim(),
            Telefono = request.Telefono?.Trim(),
            FechaNacimiento = request.FechaNacimiento,
            Activo = true
        };

        await _repo.AddAsync(entity, ct);

        try
        {
            await _emailService.SendEmailAsync(
                entity.Email,
                "¡Bienvenido a FitGym!",
                $"Hola {entity.Nombre},<br>Tu registro como socio fue exitoso.",
                ct);
        }
        catch
        {
            // Log error but don't fail the request
        }

        return Ok(new
        {
            message = "Socio registrado correctamente.",
            id = entity.Id,
            nombre = entity.Nombre,
            email = entity.Email
        });
    }
}
