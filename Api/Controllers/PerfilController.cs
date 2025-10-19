using Api.Data;
using Api.Data.Models;
using Api.Contracts; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepcionista")]
    [ApiController]
    [Route("api/perfil")]
    public class PerfilController : ControllerBase
    {
        private readonly GymDbContext _db;
        private readonly IWebHostEnvironment _env;

        public PerfilController(GymDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // ✅ Obtener perfil completo por ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPerfil(int id, CancellationToken ct)
        {
            var usuario = await _db.Usuarios
                .Include(u => u.Personal)
                .Include(u => u.Avatar)
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id, ct);

            if (usuario == null)
                return NotFound("Usuario no encontrado");

            return Ok(new
            {
                usuario.Id,
                usuario.Alias,
                usuario.Email,
                Rol = usuario.Rol?.Nombre,
                Personal = usuario.Personal != null
                    ? new
                    {
                        usuario.Personal.Nombre,
                        usuario.Personal.Telefono,
                        usuario.Personal.Direccion,
                        usuario.Personal.Especialidad,
                        usuario.Personal.Estado
                    }
                    : null,
                Avatar = usuario.Avatar != null
                    ? new { usuario.Avatar.Id, usuario.Avatar.Url, usuario.Avatar.Nombre }
                    : new { Id = 0, Url = "/images/user.png", Nombre = "avatar por defecto" }
            });
        }

        // ✅ Subir o reemplazar avatar
        [HttpPost("{id:int}/avatar")]
        public async Task<IActionResult> SubirAvatar(int id, IFormFile archivo, CancellationToken ct)
        {
            var usuario = await _db.Usuarios
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Id == id, ct);

            if (usuario == null)
                return NotFound("Usuario no encontrado");

            // Validar tipo de archivo
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
                return BadRequest("Formato de imagen no permitido.");

            // 🗑️ Eliminar avatar anterior si existe
            if (usuario.Avatar != null)
            {
                try
                {
                    var oldPath = Path.Combine(_env.WebRootPath ?? "wwwroot", usuario.Avatar.Url.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);

                    _db.Avatares.Remove(usuario.Avatar);
                    usuario.Avatar = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al eliminar avatar anterior: {ex.Message}");
                }
            }

            // 📸 Guardar nuevo avatar
            var uploadsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "avatars");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await archivo.CopyToAsync(stream);

            var nuevoAvatar = new Avatar
            {
                Nombre = Path.GetFileNameWithoutExtension(archivo.FileName),
                Url = $"/uploads/avatars/{fileName}"
            };

            usuario.Avatar = nuevoAvatar;
            await _db.SaveChangesAsync(ct);

            return Ok(new { message = "✅ Avatar actualizado correctamente.", nuevoAvatar });
        }

        [HttpPatch("{id}/password")]
        public async Task<IActionResult> CambiarPassword(int id, [FromBody] PasswordUpdateDto dto, CancellationToken ct)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Actual) || string.IsNullOrWhiteSpace(dto.Nueva))
                return BadRequest(new { message = "Debe completar todos los campos." });

            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            bool isVerified = false;
            string storedHash = usuario.PasswordHash ?? string.Empty;

            if (storedHash.StartsWith("$2") && storedHash.Length > 20) 
            {
                try
                {
                    if (BCrypt.Net.BCrypt.Verify(dto.Actual, storedHash))
                    {
                        isVerified = true;
                    }
                }
                catch (BCrypt.Net.SaltParseException)
                {
                    
                }
            }

            if (!isVerified)
            {
                if (VerificarPassword(dto.Actual, storedHash))
                {
                    isVerified = true;
                }
                else if (dto.Actual == storedHash) 
                {
                    isVerified = true;
                }
            }


            if (!isVerified)
                return BadRequest(new { message = "La contraseña actual es incorrecta." });

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Nueva);
            await _db.SaveChangesAsync(ct);

            return Ok(new { message = "Contraseña actualizada correctamente." });
        }

        private static bool VerificarPassword(string password, string hash)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashInput = Convert.ToBase64String(sha.ComputeHash(bytes));
            return hashInput == hash;
        }

        private static string HashearPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }

    }
}
