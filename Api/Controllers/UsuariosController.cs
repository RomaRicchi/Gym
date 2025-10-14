using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _repo;

        public UsuariosController(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        // ✅ GET: api/usuarios (solo activos)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAll(CancellationToken ct)
        {
            var usuarios = await _repo.GetAllAsync(ct);
            return Ok(usuarios);
        }

        // ✅ GET: api/usuarios/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetById(int id, CancellationToken ct)
        {
            var usuario = await _repo.GetByIdAsync(id, ct);
            if (usuario == null)
                return NotFound($"No se encontró el usuario con ID {id}");

            return Ok(usuario);
        }

        // ✅ POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> Create([FromBody] Usuario usuario, CancellationToken ct)
        {
            var created = await _repo.AddAsync(usuario, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ✅ PUT: api/usuarios/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Usuario usuario, CancellationToken ct)
        {
            if (id != usuario.Id)
                return BadRequest("El ID del cuerpo no coincide con el de la URL.");

            var ok = await _repo.UpdateAsync(usuario, ct);
            if (!ok)
                return NotFound($"No se pudo actualizar el usuario con ID {id}");

            return NoContent();
        }

        // ✅ DELETE: api/usuarios/{id}
        // 🔹 Marca el usuario como inactivo (Estado = 0)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _repo.DeleteAsync(id, ct);
            if (!ok)
                return NotFound($"No se encontró el usuario con ID {id}");

            return Ok(new { message = $"Usuario con ID {id} dado de baja (Estado = 0)." });
        }
    }
}
