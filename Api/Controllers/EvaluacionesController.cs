using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/evaluaciones")]
    [Authorize(Roles = "Administrador, Profesor")]
    public class EvaluacionesController : ControllerBase
    {
        private readonly IEvaluacionRepository _repo;

        public EvaluacionesController(IEvaluacionRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = await _repo.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var item = await _repo.GetByIdAsync(id, ct);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet("profesor/{profesorId:int}")]
        public async Task<IActionResult> GetByProfesorId(int profesorId, CancellationToken ct)
        {
            var list = await _repo.GetByProfesorIdAsync(profesorId, ct);
            return Ok(list);
        }

        [HttpGet("rutina/{rutinaAsignadaId:int}")]
        public async Task<IActionResult> GetByRutinaAsignadaId(int rutinaAsignadaId, CancellationToken ct)
        {
            var list = await _repo.GetByRutinaAsignadaIdAsync(rutinaAsignadaId, ct);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Evaluacion model, CancellationToken ct)
        {
            var created = await _repo.AddAsync(model, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Evaluacion model, CancellationToken ct)
        {
            var updated = await _repo.UpdateAsync(id, model, ct);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _repo.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
