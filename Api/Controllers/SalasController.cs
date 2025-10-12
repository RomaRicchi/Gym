using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalasController : ControllerBase
    {
        private readonly ISalaRepository _repo;

        public SalasController(ISalaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var sala = await _repo.GetByIdAsync(id, ct);
            if (sala is null) return NotFound();
            return Ok(sala);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Sala dto, CancellationToken ct = default)
        {
            var nueva = await _repo.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = nueva.Id }, nueva);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Sala dto, CancellationToken ct = default)
        {
            var sala = await _repo.GetByIdAsync(id, ct);
            if (sala is null) return NotFound();

            sala.Nombre = dto.Nombre;
            sala.Capacidad = dto.Capacidad;
            sala.Activa = dto.Activa;
            await _repo.UpdateAsync(sala, ct);

            return Ok(new { ok = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            await _repo.DeleteAsync(id, ct);
            return Ok(new { ok = true });
        }
    }
}
