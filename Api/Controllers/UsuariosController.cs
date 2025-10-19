using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Contracts;
using static BCrypt.Net.BCrypt;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _repo;
        private readonly GymDbContext _db;

        public UsuariosController(IUsuarioRepository repo, GymDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        // 🔹 GET: /api/usuarios
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var usuarios = await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Personal)
                .Include(u => u.Avatar)
                .AsNoTracking()
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Alias,
                    Rol = u.Rol != null ? u.Rol.Nombre : "(Sin rol)",
                    Personal = u.Personal != null ? u.Personal.Nombre : "(Sin asignar)",
                    Avatar = u.Avatar != null ? u.Avatar.Url : "/images/user.png",
                    u.Estado
                })
                .ToListAsync(ct);

            return Ok(new { items = usuarios });
        }

        // 🔹 GET: /api/usuarios/{id}
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

        // 🔹 PATCH: /api/usuarios/{id}/password
        [HttpPatch("{id:int}/password")]
        public async Task<IActionResult> CambiarPassword(int id, [FromBody] CambiarPasswordRequest dto, CancellationToken ct)
        {
            var usuario = await _db.Usuarios.FindAsync(new object[] { id }, ct);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            if (!Verify(dto.Actual, usuario.PasswordHash))
                return BadRequest(new { message = "La contraseña actual es incorrecta." });

            usuario.PasswordHash = HashPassword(dto.Nueva);
            await _db.SaveChangesAsync(ct);

            return Ok(new { message = "🔒 Contraseña actualizada correctamente." });
        }

        // 🔹 POST: /api/usuarios/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto, CancellationToken ct)
        {
            // 🔍 Buscar por alias o email indistintamente
            var usuario = await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u =>
                    u.Email == dto.Email || u.Alias == dto.Email, ct);

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

            // 🔐 Token temporal (puedes luego cambiar a JWT)
            var token = "fake-jwt-token-demo";

            return Ok(new
            {
                token,
                usuario = new
                {
                    usuario.Id,
                    usuario.Email,
                    usuario.Alias,
                    Rol = usuario.Rol?.Nombre,
                    Avatar = usuario.Avatar?.Url
                }
            });
        }
    }

    // 📦 Modelos auxiliares del contrato
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty; // puede ser email o alias
        public string Password { get; set; } = string.Empty;
    }

    public class CambiarPasswordRequest
    {
        public string Actual { get; set; } = string.Empty;
        public string Nueva { get; set; } = string.Empty;
    }
}
