using Api.Data;
using Api.Data.Models;
using Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepción, Socio")]
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

        // PERFIL GENERAL (ADMIN / PERSONAL)
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
                        Estado = usuario.Personal.Estado

                    }
                    : null,

                Avatar = usuario.Avatar != null
                    ? new { usuario.Avatar.Id, usuario.Avatar.Url, Nombre = usuario.Avatar.Nombre ?? string.Empty }
                    : new { Id = 0, Url = "/images/user.png", Nombre = "avatar por defecto" }
            });
        }

        // PERFIL DEL SOCIO LOGUEADO
        [Authorize(Roles = "Socio")]
        [HttpGet("socio")]
        public async Task<IActionResult> GetPerfilSocio(CancellationToken ct)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return BadRequest("Token sin ID de usuario.");

                int userId = int.Parse(userIdClaim);

                var usuario = await _db.Usuarios
                    .Include(u => u.Avatar)
                    .FirstOrDefaultAsync(u => u.Id == userId, ct);

                if (usuario == null)
                    return NotFound("Usuario no encontrado.");

                if (usuario.SocioId == null)
                    return NotFound("Este usuario no está vinculado a ningún socio.");

                var socio = await _db.Socios
                    .Include(s => s.Suscripciones.Where(su => su.Estado))
                    .ThenInclude(su => su.Plan)
                    .FirstOrDefaultAsync(s => s.Id == usuario.SocioId, ct);

                if (socio == null)
                    return NotFound("Socio no encontrado.");

                return Ok(new
                {
                    socio.Id,
                    socio.Nombre,
                    socio.Dni,
                    socio.Telefono,
                    socio.FechaNacimiento,
                    socio.Activo,
                    Usuario = new
                    {
                        usuario.Id,
                        usuario.Alias,
                        usuario.Email,
                        AvatarUrl = usuario.Avatar != null ? usuario.Avatar.Url : "/images/user.png"
                    },
                    PlanActual = socio.Suscripciones.FirstOrDefault()?.Plan.Nombre
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error en GetPerfilSocio: {ex.Message}");
                return StatusCode(500, new { message = "Error interno en el servidor", error = ex.Message });
            }
        }

        // SUBIR / REEMPLAZAR AVATAR
        [Authorize(Roles = "Socio, Administrador")]
        [HttpPost("{id:int}/avatar")]
        public async Task<IActionResult> SubirAvatar(int id, IFormFile archivo, CancellationToken ct)
        {
            var usuario = await _db.Usuarios
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Id == id, ct);

            if (usuario == null)
                return NotFound("Usuario no encontrado");

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
                return BadRequest("Formato de imagen no permitido.");

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
                    Console.WriteLine($"Error al eliminar avatar anterior: {ex.Message}");
                }
            }

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

            return Ok(new { url = nuevoAvatar.Url });
        }

        // CAMBIAR CONTRASEÑA
        [HttpPatch("{id:int}/password")]
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
                        isVerified = true;
                }
                catch (BCrypt.Net.SaltParseException) { }
            }

            if (!isVerified)
            {
                if (VerificarPassword(dto.Actual, storedHash))
                    isVerified = true;
                else if (dto.Actual == storedHash)
                    isVerified = true;
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

        // ACTUALIZAR DATOS BÁSICOS DEL PERFIL DEL SOCIO
        [Authorize(Roles = "Socio")]
        [HttpPatch("{id:int}/socio")]
        public async Task<IActionResult> ActualizarPerfilSocio(int id, [FromBody] PerfilSocioUpdateDto dto, CancellationToken ct)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Cuerpo de solicitud vacío o inválido.");

                var usuario = await _db.Usuarios
                    .Include(u => u.Socio)
                    .FirstOrDefaultAsync(u => u.Id == id, ct);

                if (usuario == null)
                    return NotFound("Usuario no encontrado.");

                if (usuario.Socio == null)
                    return NotFound("El usuario no está vinculado a un socio.");

                // 🔹 Actualiza datos del socio
                if (!string.IsNullOrWhiteSpace(dto.Nombre))
                    usuario.Socio.Nombre = dto.Nombre;

                if (!string.IsNullOrWhiteSpace(dto.Telefono))
                    usuario.Socio.Telefono = dto.Telefono;

                if (!string.IsNullOrWhiteSpace(dto.Dni))
                    usuario.Socio.Dni = dto.Dni; 

                if (dto.FechaNacimiento.HasValue)
                    usuario.Socio.FechaNacimiento = dto.FechaNacimiento.Value;

                // 🔹 Actualiza datos del usuario
                if (!string.IsNullOrWhiteSpace(dto.Alias))
                    usuario.Alias = dto.Alias;

                if (!string.IsNullOrWhiteSpace(dto.Email))
                    usuario.Email = dto.Email;

                await _db.SaveChangesAsync(ct);
                return Ok(new { message = "Perfil del socio actualizado correctamente." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error en ActualizarPerfilSocio: {ex.Message}");
                return StatusCode(500, new { message = "Error interno al actualizar el perfil del socio.", error = ex.Message });
            }
        }

        // ACTUALIZAR PERFIL DE PERSONAL / ADMIN / PROFESOR
        [Authorize(Roles = "Administrador, Profesor, Recepción")]
        [HttpPatch("{id:int}/personal")]
        public async Task<IActionResult> ActualizarPerfilPersonal(int id, [FromBody] PerfilUpdateDto dto, CancellationToken ct)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Cuerpo de solicitud vacío o inválido.");

                var usuario = await _db.Usuarios
                    .Include(u => u.Personal)
                    .FirstOrDefaultAsync(u => u.Id == id, ct);

                if (usuario == null)
                    return NotFound("Usuario no encontrado.");

                // Datos de Personal
                if (usuario.Personal != null)
                {
                    if (!string.IsNullOrWhiteSpace(dto.Nombre))
                        usuario.Personal.Nombre = dto.Nombre;

                    if (!string.IsNullOrWhiteSpace(dto.Telefono))
                        usuario.Personal.Telefono = dto.Telefono;

                    if (!string.IsNullOrWhiteSpace(dto.Direccion))
                        usuario.Personal.Direccion = dto.Direccion;

                    if (!string.IsNullOrWhiteSpace(dto.Especialidad))
                        usuario.Personal.Especialidad = dto.Especialidad;
                }

                // Datos del Usuario
                if (!string.IsNullOrWhiteSpace(dto.Alias))
                    usuario.Alias = dto.Alias;

                if (!string.IsNullOrWhiteSpace(dto.Email))
                    usuario.Email = dto.Email;

                await _db.SaveChangesAsync(ct);
                return Ok(new { message = "Perfil actualizado correctamente." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error en ActualizarPerfilPersonal: {ex.Message}");
                return StatusCode(500, new { message = "Error interno al actualizar el perfil.", error = ex.Message });
            }
        }

    }
}
