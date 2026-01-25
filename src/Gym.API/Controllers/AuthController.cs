using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gym.Application.DTOs.Auth;
using Gym.Application.Interfaces;
using Gym.Domain.Entities;
using Gym.Domain.Interfaces;
using Gym.Infrastructure.Persistence;
using Gym.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Gym.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly GymDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public AuthController(
        GymDbContext db,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IConfiguration config)
    {
        _db = db;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _config = config;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (usuario == null)
            return BadRequest(new { message = "Usuario no encontrado." });

        if (!_passwordHasher.Verify(request.Password, usuario.PasswordHash))
            return BadRequest(new { message = "Credenciales incorrectas." });

        var token = _jwtService.GenerateToken(usuario);

        return Ok(new LoginResponse
        {
            Token = token,
            Usuario = new UsuarioInfoDto
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Alias = usuario.Alias ?? string.Empty,
                Rol = usuario.Rol?.Nombre,
                Avatar = usuario.Avatar?.Url,
                TenantId = usuario.TenantId
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
            RolId = 4, // Rol "Socio"
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
        var reset = new PasswordResetToken
        {
            UsuarioId = user.Id,
            Token = token,
            Expira = DateTime.UtcNow.AddHours(1)
        };

        _db.PasswordResetTokens.Add(reset);
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
}
