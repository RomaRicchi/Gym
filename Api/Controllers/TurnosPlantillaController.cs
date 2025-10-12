using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TurnosPlantillaController : ControllerBase
    {
        private readonly ITurnoPlantillaRepository _repo;

        public TurnosPlantillaController(ITurnoPlantillaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos(CancellationToken ct = default)
        {
            var list = await _repo.GetActivosAsync(ct);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var turno = await _repo.GetByIdAsync(id, ct);
            if (turno is null) return NotFound();
            return Ok(turno);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] TurnoPlantilla dto, CancellationToken ct = default)
        {
            var nuevo = await _repo.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] TurnoPlantilla dto, CancellationToken ct = default)
        {
            var turno = await _repo.GetByIdAsync(id, ct);
            if (turno is null) return NotFound();

            turno.SalaId = dto.SalaId;
            turno.PersonalId = dto.PersonalId;
            turno.DiaSemana = dto.DiaSemana;
            turno.HoraInicio = dto.HoraInicio;
            turno.DuracionMin = dto.DuracionMin;
            turno.Cupo = dto.Cupo;
            turno.Activo = dto.Activo;

            await _repo.UpdateAsync(turno, ct);
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
