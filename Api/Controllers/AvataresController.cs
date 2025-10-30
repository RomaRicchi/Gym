using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepción, Socio")]
    [ApiController]
    [Route("api/[controller]")]
    public class AvataresController : ControllerBase
    {
        private readonly IAvatarRepository _repo;
        private readonly GymDbContext _db;

        public AvataresController(IAvatarRepository repo, GymDbContext db)
        {
            _repo = repo;
            _db = db;
        }

       
        [HttpPost("upload")]
[Authorize(Roles = "Socio, Administrador, Profesor, Recepción")]
public async Task<IActionResult> Upload([FromForm] IFormFile archivo)
{
    if (archivo == null || archivo.Length == 0)
        return BadRequest("Debe seleccionar una imagen.");

    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
    if (!Directory.Exists(uploadsPath))
        Directory.CreateDirectory(uploadsPath);

    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
    var filePath = Path.Combine(uploadsPath, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
        await archivo.CopyToAsync(stream);

    var avatar = new Avatar
    {
        Url = $"/uploads/avatars/{fileName}",
        Nombre = fileName,
        EsPredeterminado = false
    };

    _db.Avatares.Add(avatar);
    await _db.SaveChangesAsync();

    var idUsuario = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
    var usuario = await _db.Usuarios.FindAsync(idUsuario);
    if (usuario == null)
        return NotFound("Usuario no encontrado.");

    usuario.IdAvatar = avatar.Id;
    await _db.SaveChangesAsync();

    return Ok(new { message = "Avatar subido y asociado correctamente", id = avatar.Id, url = avatar.Url });
}

        // GET: api/avatares
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Avatar>>> GetAll(CancellationToken ct)
        {
            var avatares = await _repo.GetAllAsync(ct);
            return Ok(avatares);
        }

        // GET: api/avatares/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Avatar>> GetById(int id, CancellationToken ct)
        {
            var avatar = await _repo.GetByIdAsync(id, ct);
            if (avatar == null)
                return NotFound($"No se encontró el avatar con ID {id}");

            return Ok(avatar);
        }

        // POST: api/avatares
        [HttpPost]
        public async Task<ActionResult<Avatar>> Create([FromBody] Avatar avatar, CancellationToken ct)
        {
            var created = await _repo.AddAsync(avatar, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/avatares/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Avatar avatar, CancellationToken ct)
        {
            if (id != avatar.Id)
                return BadRequest("El ID del cuerpo no coincide con el de la URL.");

            var ok = await _repo.UpdateAsync(avatar, ct);
            if (!ok)
                return NotFound($"No se pudo actualizar el avatar con ID {id}");

            return NoContent();
        }

        // DELETE: api/avatares/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _repo.DeleteAsync(id, ct);
            if (!ok)
                return NotFound($"No se encontró el avatar con ID {id}");

            return NoContent();
        }
    }
}
