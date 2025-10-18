using Api.Data;
using Api.Data.Models;
using Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Api.Repositories.Interfaces; 
using Microsoft.EntityFrameworkCore;

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

        // 🔹 GET: /api/usuarios
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            try
            {
                var usuarios = await _repo.GetAllAsync(ct);
                return Ok(new { items = usuarios });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en GetAll: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener los usuarios." });
            }
        }

        // 🔹 GET: /api/usuarios/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            try
            {
                var usuario = await _repo.GetByIdAsync(id, ct);
                if (usuario == null)
                    return NotFound(new { message = "Usuario no encontrado." });

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en GetById: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener el usuario." });
            }
        }

        // 🔹 POST: /api/usuarios
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Usuario dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                dto.CreadoEn = DateTime.UtcNow;
                var nuevo = await _repo.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Crear: {ex.Message}");
                return StatusCode(500, new { error = "Error al crear el usuario." });
            }
        }

        // 🔹 PUT: /api/usuarios/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Usuario dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var ok = await _repo.UpdateAsync(id, dto, ct);
                if (!ok)
                    return NotFound(new { message = "Usuario no encontrado." });

                return Ok(new { message = "Usuario actualizado correctamente." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Actualizar: {ex.Message}");
                return StatusCode(500, new { error = "Error al actualizar el usuario." });
            }
        }

        // 🔹 DELETE lógico: /api/usuarios/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            try
            {
                var ok = await _repo.DeleteAsync(id, ct);
                if (!ok)
                    return NotFound(new { message = "Usuario no encontrado." });

                return Ok(new { message = "Usuario eliminado correctamente." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Eliminar: {ex.Message}");
                return StatusCode(500, new { error = "Error al eliminar el usuario." });
            }
        }
    }
}
