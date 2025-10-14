using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadoOrdenPagoController : ControllerBase
    {
        private readonly IEstadoOrdenPagoRepository _repo;

        public EstadoOrdenPagoController(IEstadoOrdenPagoRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var estado = await _repo.GetByIdAsync(id, ct);
            return estado is null ? NotFound() : Ok(estado);
        }

        // 🔹 Nuevo endpoint para buscar por las primeras letras del nombre
        [HttpGet("nombre/{nombre}")]
        public async Task<IActionResult> GetByNombre(string nombre, CancellationToken ct = default)
        {
            var estado = await _repo.GetByNombreAsync(nombre, ct);
            return estado is null ? NotFound() : Ok(estado);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] EstadoOrdenPago dto, CancellationToken ct = default)
        {
            var nuevo = await _repo.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] EstadoOrdenPago dto, CancellationToken ct = default)
        {
            var estado = await _repo.GetByIdAsync(id, ct);
            if (estado is null) return NotFound();

            estado.Nombre = dto.Nombre;
            estado.Descripcion = dto.Descripcion;

            await _repo.UpdateAsync(estado, ct);
            return Ok(new { ok = true });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            await _repo.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
