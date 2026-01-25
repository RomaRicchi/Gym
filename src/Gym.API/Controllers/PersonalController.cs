using Gym.Application.DTOs.Personal;
using Gym.Domain.Entities;
using Gym.Infrastructure.Persistence;
using Gym.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.API.Controllers;

[Authorize]
[ApiController]
[Route("api/personal")]
public class PersonalController : ControllerBase
{
    private readonly GymDbContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public PersonalController(GymDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var personal = await _db.Personal
            .Include(p => p.Usuario)!.ThenInclude(u => u!.Rol)
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .ToListAsync(ct);

        var result = personal.Select(p => new
        {
            id = p.Id,
            nombre = p.Nombre,
            telefono = p.Telefono,
            direccion = p.Direccion,
            especialidad = p.Especialidad,
            estado = p.Estado,
            email = p.Usuario?.Email,
            rol = p.Usuario?.Rol?.Nombre,
            createdAt = p.CreatedAt
        });

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var personal = await _db.Personal
            .Include(p => p.Usuario)!.ThenInclude(u => u!.Rol)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (personal == null)
            return NotFound(new { message = "Personal no encontrado." });

        return Ok(new PersonalDto
        {
            Id = personal.Id,
            Nombre = personal.Nombre,
            Telefono = personal.Telefono,
            Direccion = personal.Direccion,
            Especialidad = personal.Especialidad,
            Estado = personal.Estado,
            Email = personal.Usuario?.Email,
            CreatedAt = personal.CreatedAt
        });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PersonalCreateRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest(new { message = "El nombre es obligatorio." });

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailExists = await _db.Usuarios.AnyAsync(u => u.Email == request.Email, ct);
            if (emailExists)
                return Conflict(new { message = "Ya existe un usuario con ese email." });
        }

        var personal = new Personal
        {
            Nombre = request.Nombre.Trim(),
            Telefono = request.Telefono?.Trim(),
            Direccion = request.Direccion?.Trim(),
            Especialidad = request.Especialidad?.Trim(),
            Estado = request.Activo
        };

        _db.Personal.Add(personal);
        await _db.SaveChangesAsync(ct);

        if (!string.IsNullOrWhiteSpace(request.Email) && request.RolId.HasValue)
        {
            var usuario = new Usuario
            {
                Email = request.Email!.Trim(),
                Alias = request.Email!.Split('@')[0],
                RolId = request.RolId.Value,
                Estado = true,
                PersonalId = personal.Id,
                PasswordHash = _passwordHasher.Hash("fitgym123")
            };

            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync(ct);
        }

        return Ok(new { message = "Personal creado correctamente.", id = personal.Id, tempPassword = "fitgym123" });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PersonalUpdateRequest request, CancellationToken ct)
    {
        var personal = await _db.Personal.Include(p => p.Usuario).FirstOrDefaultAsync(p => p.Id == id, ct);
        if (personal == null)
            return NotFound(new { message = "Personal no encontrado." });

        personal.Nombre = request.Nombre.Trim();
        personal.Telefono = request.Telefono?.Trim();
        personal.Direccion = request.Direccion?.Trim();
        personal.Especialidad = request.Especialidad?.Trim();
        personal.Estado = request.Activo;

        if (personal.Usuario != null)
        {
            if (!string.IsNullOrWhiteSpace(request.Email))
                personal.Usuario.Email = request.Email.Trim();
            if (request.RolId.HasValue)
                personal.Usuario.RolId = request.RolId.Value;
            personal.Usuario.Estado = request.Activo;
        }

        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Registro actualizado correctamente." });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var personal = await _db.Personal.Include(p => p.Usuario).FirstOrDefaultAsync(p => p.Id == id, ct);
        if (personal == null)
            return NotFound(new { message = "Personal no encontrado." });

        personal.Estado = false;
        if (personal.Usuario != null)
        {
            personal.Usuario.Estado = false;
        }

        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Registro desactivado correctamente." });
    }
}
