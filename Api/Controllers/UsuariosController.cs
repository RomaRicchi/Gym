using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Contracts;
using BCrypt.Net;
using static BCrypt.Net.BCrypt; // ✅ habilita HashPassword() y Verify()

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
            var usuarios = await _repo.GetAllAsync(ct);
            return Ok(new { items = usuarios });
        }

        // 🔹 GET: /api/usuarios/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var usuario = await _repo.GetByIdAsync(id, ct);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            return Ok(usuario);
        }

        // 🔹 POST: /api/usuarios
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Usuario dto, CancellationToken ct)
        {
            dto.CreadoEn = DateTime.UtcNow;

            // ✅ Hash automático si se envía contraseña en texto plano
            if (!string.IsNullOrWhiteSpace(dto.PasswordHash) && !dto.PasswordHash.StartsWith("$2"))
            {
                dto.PasswordHash = HashPassword(dto.PasswordHash);
            }

            var nuevo = await _repo.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }

        // 🔹 PUT: /api/usuarios/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Usuario dto, CancellationToken ct)
        {
            var ok = await _repo.UpdateAsync(id, dto, ct);
            if (!ok)
                return NotFound(new { message = "Usuario no encontrado." });

            return Ok(new { message = "Usuario actualizado correctamente." });
        }

        // 🔹 DELETE lógico: /api/usuarios/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            var ok = await _repo.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(new { message = "Usuario no encontrado." });

            return Ok(new { message = "Usuario eliminado correctamente." });
        }

        // 🔹 POST: /api/usuarios/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto, CancellationToken ct)
        {
            var usuario = await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Email == dto.Email, ct);

            if (usuario == null)
                return BadRequest(new { message = "Usuario no encontrado." });

            // ⚙️ Verificación de contraseña segura
            bool esValida = false;

            if (!string.IsNullOrEmpty(usuario.PasswordHash))
            {
                // ✅ Si la contraseña está hasheada con BCrypt
                if (usuario.PasswordHash.StartsWith("$2"))
                    esValida = Verify(dto.Password, usuario.PasswordHash);
                else
                    esValida = usuario.PasswordHash == dto.Password;
            }

            if (!esValida)
                return BadRequest(new { message = "Credenciales incorrectas." });

            // 🔐 Token temporal (luego reemplazar por JWT)
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
}
