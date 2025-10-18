using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
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

        // ✅ Obtener perfil por ID
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
                Nombre = usuario.Personal?.Nombre,
                Telefono = usuario.Personal?.Telefono,
                Especialidad = usuario.Personal?.Especialidad,
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

            // 🗑️ Eliminar avatar anterior si existe
            if (usuario.Avatar != null)
            {
                try
                {
                    var oldPath = Path.Combine(_env.WebRootPath ?? "wwwroot", usuario.Avatar.Url.TrimStart('/'));

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                        Console.WriteLine($"🧹 Avatar anterior eliminado: {oldPath}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Archivo de avatar anterior no encontrado en: {oldPath}");
                    }

                    // Eliminar también de la base de datos
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

            var fileName = $"{Guid.NewGuid()}_{archivo.FileName}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var nuevoAvatar = new Avatar
            {
                Nombre = archivo.FileName,
                Url = $"/uploads/avatars/{fileName}"
            };

            usuario.Avatar = nuevoAvatar;
            await _db.SaveChangesAsync(ct);

            return Ok(new
            {
                message = "✅ Avatar actualizado correctamente.",
                nuevoAvatar
            });
        }


        // ✅ Restablecer avatar por defecto
        [HttpPost("{id:int}/avatar/default")]
        public async Task<IActionResult> AsignarAvatarPorDefecto(int id, CancellationToken ct)
        {
            var usuario = await _db.Usuarios
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Id == id, ct);

            if (usuario == null)
                return NotFound("Usuario no encontrado");

            // 🗑️ Eliminar avatar actual si existe
            if (usuario.Avatar != null)
            {
                try
                {
                    var oldPath = Path.Combine(_env.WebRootPath ?? "wwwroot", usuario.Avatar.Url.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                        Console.WriteLine($"🧹 Avatar anterior eliminado: {oldPath}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Archivo de avatar anterior no encontrado en: {oldPath}");
                    }

                    _db.Avatares.Remove(usuario.Avatar);
                    usuario.Avatar = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al eliminar avatar anterior: {ex.Message}");
                }
            }

            // 🧩 Asignar avatar por defecto (imagen genérica)
            var defaultAvatar = await _db.Avatares.FirstOrDefaultAsync(a => a.Nombre == "user", ct);
            if (defaultAvatar == null)
            {
                defaultAvatar = new Avatar
                {
                    Nombre = "user",
                    Url = "/images/user.png"
                };
                _db.Avatares.Add(defaultAvatar);
            }

            usuario.Avatar = defaultAvatar;
            await _db.SaveChangesAsync(ct);

            return Ok(new
            {
                message = "✅ Avatar por defecto asignado correctamente.",
                avatar = new { defaultAvatar.Id, defaultAvatar.Url }
            });
        }

    }
}
