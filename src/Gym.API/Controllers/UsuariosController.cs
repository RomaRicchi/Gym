using System.Security.Claims;
using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Usuarios;
using Gym.Application.DTOs.Auth;
using Gym.Application.Interfaces;
using Gym.Domain.Entities;
using Gym.Domain.Interfaces;
using Gym.Infrastructure.Persistence;
using Gym.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.API.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly GymDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IFileStorageService _fileStorage;

    public UsuariosController(
        GymDbContext db,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IConfiguration config,
        IUsuarioRepository usuarioRepo,
        IFileStorageService fileStorage)
    {
        _db = db;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _config = config;
        _usuarioRepo = usuarioRepo;
        _fileStorage = fileStorage;
    }

    // --- Autenticación pública ---
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (usuario == null || !_passwordHasher.Verify(request.Password, usuario.PasswordHash))
            return BadRequest(new { message = "Credenciales incorrectas." });

        if (!usuario.Estado)
            return Unauthorized(new { message = "Usuario desactivado." });

        var token = _jwtService.GenerateToken(usuario);

        return Ok(new
        {
            token,
            usuario = new
            {
                id = usuario.Id,
                email = usuario.Email,
                alias = usuario.Alias,
                rol = usuario.Rol?.Nombre,
                rolId = usuario.RolId,
                estado = usuario.Estado,
                avatar = usuario.Avatar?.Url,
                socioId = usuario.SocioId,
                personalId = usuario.PersonalId,
                tenantId = usuario.TenantId
            }
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        if (await _db.Usuarios.AnyAsync(u => u.Email == request.Email, ct))
            return BadRequest(new { message = "Ya existe un usuario con ese correo." });

        var usuario = new Usuario
        {
            Email = request.Email,
            Alias = request.Alias,
            PasswordHash = _passwordHasher.Hash(request.Password),
            RolId = 4, // Socio
            Estado = true,
            SocioId = request.SocioId
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Usuario registrado correctamente." });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
    {
        var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        if (user == null)
            return NotFound(new { message = "No existe un usuario con ese correo." });

        var token = Guid.NewGuid().ToString();
        _db.PasswordResetTokens.Add(new PasswordResetToken
        {
            UsuarioId = user.Id,
            Token = token,
            Expira = DateTime.UtcNow.AddHours(1)
        });
        await _db.SaveChangesAsync(ct);

        var frontendUrl = _config["FrontendUrl"] ?? "http://localhost:5173";
        var resetLink = $"{frontendUrl}/reset-password?token={token}";

        await _emailService.SendEmailAsync(
            user.Email,
            "Recuperar contraseña",
            $"Haz clic en el siguiente enlace para restablecer tu contraseña:<br><a href='{resetLink}'>Restablecer contraseña</a>",
            ct);

        return Ok(new { message = "Correo de recuperación enviado." });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
    {
        var reset = await _db.PasswordResetTokens
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.Token == request.Token && x.Expira > DateTime.UtcNow, ct);

        if (reset == null)
            return BadRequest(new { message = "Token inválido o expirado." });

        reset.Usuario.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        _db.PasswordResetTokens.Remove(reset);
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Contraseña restablecida correctamente." });
    }

    // --- Gestión de usuarios ---
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1,
        int pageSize = 10,
        string? q = null,
        CancellationToken ct = default)
    {
        var query = _db.Usuarios
            .Include(u => u.Rol)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(u => u.Email.Contains(q) || (u.Alias != null && u.Alias.Contains(q)));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var dtos = items.Select(u => new UsuarioDto
        {
            Id = u.Id,
            Email = u.Email,
            Alias = u.Alias,
            RolId = u.RolId,
            RolNombre = u.Rol?.Nombre,
            Estado = u.Estado,
            Avatar = null,
            CreatedAt = u.CreatedAt
        }).ToList();

        return Ok(new PaginatedResult<UsuarioDto>
        {
            Items = dtos,
            TotalItems = total,
            Page = page,
            PageSize = pageSize
        });
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var usuario = await _usuarioRepo.GetWithDetailsAsync(id, ct);
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado." });

        return Ok(new UsuarioDto
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Alias = usuario.Alias,
            RolId = usuario.RolId,
            RolNombre = usuario.Rol?.Nombre,
            Estado = usuario.Estado,
            Avatar = usuario.Avatar?.Url,
            CreatedAt = usuario.CreatedAt
        });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpdateRequest request, CancellationToken ct)
    {
        var usuario = await _usuarioRepo.GetByIdAsync(id, ct);
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado." });

        if (!string.IsNullOrWhiteSpace(request.Email))
            usuario.Email = request.Email.Trim();
        usuario.Alias = request.Alias;
        usuario.RolId = request.RolId;
        usuario.Estado = request.Estado;

        await _usuarioRepo.UpdateAsync(usuario, ct);
        return Ok(new { message = "Usuario actualizado correctamente." });
    }

    [Authorize(Roles = "Administrador,Recepcion")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var usuario = await _usuarioRepo.GetByIdAsync(id, ct);
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado." });

        usuario.Estado = false;
        await _usuarioRepo.UpdateAsync(usuario, ct);
        return Ok(new { message = "Usuario desactivado correctamente." });
    }

    // --- Perfil del usuario ---
    private async Task<Usuario?> GetUsuarioActualAsync(CancellationToken ct)
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                  User.FindFirstValue(ClaimTypes.Name) ??
                  User.FindFirstValue("sub");
        if (sub == null) return null;
        return await _usuarioRepo.GetWithDetailsAsync(int.Parse(sub), ct);
    }

    [Authorize]
    [HttpGet("perfil")]
    public async Task<IActionResult> GetPerfilActual(CancellationToken ct)
    {
        var usuario = await GetUsuarioActualAsync(ct);
        if (usuario == null)
            return Unauthorized();

        return Ok(new
        {
            id = usuario.Id,
            email = usuario.Email,
            alias = usuario.Alias,
            rol = usuario.Rol?.Nombre,
            avatar = usuario.Avatar?.Url,
            socio = usuario.Socio == null ? null : new { id = usuario.Socio.Id, usuario.Socio.Nombre, usuario.Socio.Dni },
            socio_id = usuario.SocioId,
            personal_id = usuario.PersonalId
        });
    }

    [Authorize]
    [HttpGet("/api/perfil/{id:int}")]
    public async Task<IActionResult> GetPerfil(int id, CancellationToken ct)
    {
        var usuario = await _usuarioRepo.GetWithDetailsAsync(id, ct);
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado." });

        return Ok(new PerfilDto
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Alias = usuario.Alias,
            RolNombre = usuario.Rol?.Nombre,
            Avatar = usuario.Avatar?.Url,
            Nombre = usuario.Personal?.Nombre,
            Telefono = usuario.Personal?.Telefono,
            Direccion = usuario.Personal?.Direccion,
            Especialidad = usuario.Personal?.Especialidad,
            Dni = usuario.Socio?.Dni,
            FechaNacimiento = usuario.Socio?.FechaNacimiento
        });
    }

    [Authorize]
    [HttpPatch("/api/perfil/{id:int}/personal")]
    public async Task<IActionResult> UpdatePersonal(int id, [FromBody] PerfilUpdateRequest request, CancellationToken ct)
    {
        var usuario = await _db.Usuarios.Include(u => u.Personal).FirstOrDefaultAsync(u => u.Id == id, ct);
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado." });

        if (usuario.Personal == null)
        {
            usuario.Personal = new Personal
            {
                Nombre = request.Nombre ?? usuario.Alias ?? usuario.Email,
                Telefono = request.Telefono,
                Direccion = request.Direccion,
                Especialidad = request.Especialidad,
                Estado = request.Estado
            };
        }
        else
        {
            usuario.Personal.Nombre = request.Nombre ?? usuario.Personal.Nombre;
            usuario.Personal.Telefono = request.Telefono;
            usuario.Personal.Direccion = request.Direccion;
            usuario.Personal.Especialidad = request.Especialidad;
            usuario.Personal.Estado = request.Estado;
        }

        if (!string.IsNullOrWhiteSpace(request.Alias))
            usuario.Alias = request.Alias;
        if (!string.IsNullOrWhiteSpace(request.Email))
            usuario.Email = request.Email.Trim();

        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Perfil actualizado correctamente." });
    }

    public class ChangePasswordRequest
    {
        public string Actual { get; set; } = string.Empty;
        public string Nueva { get; set; } = string.Empty;
    }

    [Authorize]
    [HttpPatch("/api/perfil/{id:int}/password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var usuario = await _db.Usuarios.FindAsync(new object[] { id }, ct);
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado." });

        if (!_passwordHasher.Verify(request.Actual, usuario.PasswordHash))
            return BadRequest(new { message = "Contraseña actual incorrecta." });

        usuario.PasswordHash = _passwordHasher.Hash(request.Nueva);
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Contraseña actualizada correctamente." });
    }

    [Authorize]
    [HttpPost("/api/perfil/{id:int}/avatar")]
    public async Task<IActionResult> UploadAvatar(int id, IFormFile archivo, CancellationToken ct)
    {
        var usuario = await _db.Usuarios.Include(u => u.Avatar).FirstOrDefaultAsync(u => u.Id == id, ct);
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado." });

        if (archivo == null || archivo.Length == 0)
            return BadRequest(new { message = "Archivo no válido." });

        await using var stream = archivo.OpenReadStream();
        var url = await _fileStorage.SaveAsync(stream, archivo.FileName, "avatars", ct);

        var avatar = new Avatar
        {
            Url = url,
            Nombre = archivo.FileName,
            EsPredeterminado = false
        };

        _db.Avatares.Add(avatar);
        await _db.SaveChangesAsync(ct);

        usuario.AvatarId = avatar.Id;
        await _db.SaveChangesAsync(ct);

        return Ok(new { url = avatar.Url });
    }

    // --- Roles ---
    [Authorize]
    [HttpGet("/api/roles")]
    public async Task<IActionResult> GetRoles(CancellationToken ct)
    {
        var roles = await _db.Roles.AsNoTracking().ToListAsync(ct);
        return Ok(roles);
    }

    [Authorize]
    [HttpGet("/api/roles/{id:int}")]
    public async Task<IActionResult> GetRol(int id, CancellationToken ct)
    {
        var rol = await _db.Roles.FindAsync(new object[] { id }, ct);
        if (rol == null)
            return NotFound(new { message = "Rol no encontrado." });
        return Ok(rol);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost("/api/roles")]
    public async Task<IActionResult> CreateRol([FromBody] Rol rol, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(rol.Nombre))
            return BadRequest(new { message = "El nombre es obligatorio." });

        _db.Roles.Add(rol);
        await _db.SaveChangesAsync(ct);
        return Ok(rol);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("/api/roles/{id:int}")]
    public async Task<IActionResult> UpdateRol(int id, [FromBody] Rol request, CancellationToken ct)
    {
        var rol = await _db.Roles.FindAsync(new object[] { id }, ct);
        if (rol == null)
            return NotFound(new { message = "Rol no encontrado." });

        rol.Nombre = request.Nombre;
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Rol actualizado correctamente." });
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("/api/roles/{id:int}")]
    public async Task<IActionResult> DeleteRol(int id, CancellationToken ct)
    {
        var rol = await _db.Roles.FindAsync(new object[] { id }, ct);
        if (rol == null)
            return NotFound(new { message = "Rol no encontrado." });

        _db.Roles.Remove(rol);
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Rol eliminado correctamente." });
    }
}
