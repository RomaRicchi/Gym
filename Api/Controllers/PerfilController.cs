using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/perfil")]
    public class PerfilController : ControllerBase
    {
        private readonly IUsuarioRepository _repo;

        public PerfilController(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        // 🔹 GET /api/perfil/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPerfil(int id, CancellationToken ct)
        {
            var user = await _repo.GetByIdAsync(id, ct);
            if (user is null) return NotFound();

            // 🔹 Determinar nombre y teléfono según tipo de usuario
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
                Rol = user.Rol != null ? user.Rol.Nombre : null,
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
    }
}


