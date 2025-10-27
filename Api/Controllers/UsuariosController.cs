using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Contracts;
using Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authorization;


namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _repo;
        private readonly IConfiguration _config;
        private readonly GymDbContext _db;
        private readonly IEmailService _emailService;

        public UsuariosController(IUsuarioRepository repo, IConfiguration config, GymDbContext db, IEmailService emailService)
        {
            _repo = repo;
            _config = config;
            _db = db;
            _emailService = emailService;
        }

        // GET: /api/usuarios
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            int page = 1,
            int pageSize = 10,
            string? q = null,
            CancellationToken ct = default)
        {
            // Base query
            var query = _db.Usuarios
                .Include(u => u.Rol)
                .AsNoTracking()
                .OrderBy(u => u.Alias)
                .AsQueryable();

            // Filtro por búsqueda (alias o email)
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(u =>
                    EF.Functions.Like(u.Alias, $"%{q}%") ||
                    EF.Functions.Like(u.Email, $"%{q}%"));
            }

            // Total antes de paginar
            var totalItems = await query.CountAsync(ct);

            // Paginación
            var usuarios = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Alias,
                    Rol = u.Rol != null ? u.Rol.Nombre : "(Sin rol)",
                    Estado = u.Estado
                })
                .ToListAsync(ct);

            // Devuelve formato estándar para frontend
            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                items = usuarios
            });
        }

        // 🔹 GET: /api/usuarios/{id}
        [Authorize(Roles = "Administrador")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var usuario = await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Personal)
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Id == id, ct);

            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            return Ok(new
            {
                usuario.Id,
                usuario.Email,
                usuario.Alias,
                Rol = usuario.Rol?.Nombre,
                Personal = usuario.Personal?.Nombre,
                Avatar = usuario.Avatar?.Url,
                usuario.Estado
            });
        }

        // 🔹 POST: /api/usuarios
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Usuario dto, CancellationToken ct)
        {
            dto.CreadoEn = DateTime.UtcNow;

            // ✅ Hash automático si la contraseña está en texto plano
            if (!string.IsNullOrWhiteSpace(dto.PasswordHash) && !dto.PasswordHash.StartsWith("$2"))
            {
                dto.PasswordHash = HashPassword(dto.PasswordHash);
            }

            _db.Usuarios.Add(dto);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] UsuarioUpdateDto dto, CancellationToken ct)
        {
            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            // 🧩 Validar rol antes de asignar
            if (dto.RolId <= 0 || !await _db.Roles.AnyAsync(r => r.Id == dto.RolId, ct))
                return BadRequest(new { message = "Rol inválido o no existente." });

            usuario.Email = dto.Email;
            usuario.Alias = dto.Alias;
            usuario.RolId = dto.RolId;
            usuario.Estado = dto.Estado;

            await _db.SaveChangesAsync(ct);

            return Ok(new { message = "✅ Usuario actualizado correctamente." });
        }

        // 🔹 DELETE lógico: /api/usuarios/{id}
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            var usuario = await _db.Usuarios.FindAsync(new object[] { id }, ct);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            usuario.Estado = false; // baja lógica
            await _db.SaveChangesAsync(ct);

            return Ok(new { message = "🗑️ Usuario marcado como inactivo." });
        }

        // 🔹 POST: /api/usuarios/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto, CancellationToken ct)
        {
            var usuario = await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Email == dto.Email || u.Alias == dto.Email, ct);

            if (usuario == null)
                return BadRequest(new { message = "Usuario no encontrado." });

            bool esValida = false;

            if (!string.IsNullOrEmpty(usuario.PasswordHash))
            {
                if (usuario.PasswordHash.StartsWith("$2"))
                    esValida = Verify(dto.Password, usuario.PasswordHash);
                else
                    esValida = usuario.PasswordHash == dto.Password;
            }

            if (!esValida)
                return BadRequest(new { message = "Credenciales incorrectas." });

            // 🔐 Generar token JWT con rol
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "Usuario")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new LoginResponseDto
            {
                Token = jwt,
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    Alias = usuario.Alias,
                    Rol = usuario.Rol?.Nombre,
                    Avatar = usuario.Avatar?.Url
                }
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return NotFound("No existe un usuario con ese correo.");

            var token = Guid.NewGuid().ToString();
            var reset = new PasswordResetToken
            {
                UsuarioId = user.Id,
                Token = token,
                Expira = DateTime.UtcNow.AddHours(1)
            };

            _db.PasswordResetTokens.Add(reset);
            await _db.SaveChangesAsync();

            // ✉️ Enviar correo con el token
            var frontendUrl = _config["FrontendUrl"];
            var resetLink = $"{frontendUrl}/reset-password?token={token}";
            await _emailService.SendEmailAsync(user.Email, "Recuperar contraseña",
                $"Haz clic en el siguiente enlace para restablecer tu contraseña:<br><a href='{resetLink}'>Restablecer contraseña</a>");

            return Ok("Correo de recuperación enviado.");
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var reset = await _db.PasswordResetTokens
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Token == dto.Token && x.Expira > DateTime.UtcNow);

            if (reset == null) return BadRequest("Token inválido o expirado.");

            reset.Usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            _db.PasswordResetTokens.Remove(reset);
            await _db.SaveChangesAsync();

            return Ok("Contraseña restablecida correctamente.");
        }
    }
}
