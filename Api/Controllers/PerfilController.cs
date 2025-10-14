using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/perfil")]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilRepository _repo;

        public PerfilController(IPerfilRepository repo)
        {
            _repo = repo;
        }

        // 🔹 GET /api/perfil/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPerfil(int id, CancellationToken ct)
        {
            var user = await _repo.GetPerfilAsync(id, ct);
            if (user is null) return NotFound();

            string? nombre = null;
            string? telefono = null;

            if (user.SocioId.HasValue && user.Socio != null)
            {
                nombre = user.Socio.Nombre;
                telefono = user.Socio.Telefono;
            }
            else if (user.PersonalId.HasValue && user.Personal != null)
            {
                nombre = user.Personal.Nombre;
                telefono = user.Personal.Telefono;
            }

            return Ok(new
            {
                user.Id,
                user.Email,
                user.Alias,
                Nombre = nombre,
                Telefono = telefono,
                Rol = user.Rol?.Nombre,
                Avatar = user.Avatar is null ? null : new
                {
                    user.Avatar.Id,
                    user.Avatar.Nombre,
                    user.Avatar.Url
                }
            });
        }

        // 🔹 PUT /api/perfil/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePerfil(
            int id,
            [FromBody] PerfilUpdateDto dto,
            CancellationToken ct)
        {
            var ok = await _repo.UpdatePerfilAsync(id, dto.Nombre, dto.Email, dto.Telefono, dto.IdAvatar, ct);
            return ok ? NoContent() : NotFound();
        }

        public record PerfilUpdateDto(string? Nombre, string? Email, string? Telefono, int? IdAvatar);

        // 🔹 GET /api/perfil/{id}/detallado
        [HttpGet("{id:int}/detallado")]
        public async Task<IActionResult> GetPerfilDetallado(int id, CancellationToken ct)
        {
            var user = await _repo.GetPerfilDetalladoAsync(id, ct);
            if (user is null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.Alias,
                user.Email,
                user.Estado,
                user.CreadoEn,
                Rol = user.Rol?.Nombre,
                Personal = user.Personal is null ? null : new
                {
                    user.Personal.Nombre,
                    user.Personal.Telefono,
                    user.Personal.Especialidad
                },
                Socio = user.Socio is null ? null : new
                {
                    user.Socio.Nombre,
                    user.Socio.Telefono,
                    user.Socio.Dni,
                    user.Socio.FechaNacimiento
                },
                Avatar = user.Avatar is null ? null : new
                {
                    user.Avatar.Id,
                    user.Avatar.Nombre,
                    user.Avatar.Url
                }
            });
        }

        // 🔹 POST /api/perfil/{id}/avatar
        [HttpPost("{id:int}/avatar")]
        public async Task<IActionResult> SubirAvatar(int id, [FromForm] IFormFile archivo, CancellationToken ct)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Debe enviar un archivo de imagen.");

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Avatares");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(archivo.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await archivo.CopyToAsync(stream, ct);

            var avatar = new Avatar
            {
                Url = $"uploads/avatares/{fileName}",
                Nombre = fileName,
                EsPredeterminado = false
            };

            var nuevoAvatar = await _repo.SubirAvatarAsync(id, avatar, ct);
            return Ok(new { message = "Avatar actualizado correctamente.", nuevoAvatar.Url });
        }

        public record CambiarPasswordDto(string PasswordActual, string NuevaPassword);

        // 🔹 PATCH /api/perfil/{id}/password
        [HttpPatch("{id:int}/password")]
        public async Task<IActionResult> CambiarPassword(int id, [FromBody] CambiarPasswordDto dto, CancellationToken ct)
        {
            var ok = await _repo.CambiarPasswordAsync(id, dto.PasswordActual, dto.NuevaPassword, ct);
            return ok
                ? Ok(new { ok = true, message = "Contraseña actualizada correctamente." })
                : BadRequest(new { ok = false, message = "Contraseña actual incorrecta." });
        }
    }
}
