using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/rutinaplantillaejercicios")]
    [Authorize(Roles = "Administrador, Profesor")]
    public class RutinaPlantillaEjerciciosController : ControllerBase
    {
        private readonly IRutinaPlantillaEjercicioRepository _repo;

        public RutinaPlantillaEjerciciosController(IRutinaPlantillaEjercicioRepository repo)
        {
            _repo = repo;
        }

        // GET: api/rutinaplantillaejercicios
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = await _repo.GetAllAsync(ct);
            return Ok(list);
        }

        // GET: api/rutinaplantillaejercicios/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var item = await _repo.GetByIdAsync(id, ct);
            return item == null ? NotFound() : Ok(item);
        }

        // GET: api/rutinaplantillaejercicios/rutina/{rutinaId}
        [HttpGet("rutina/{rutinaId:int}")]
        public async Task<IActionResult> GetByRutinaId(int rutinaId, CancellationToken ct)
        {
            var items = await _repo.GetByRutinaIdAsync(rutinaId, ct);
            return Ok(items);
        }

        // POST: api/rutinaplantillaejercicios
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RutinaPlantillaEjercicio model, CancellationToken ct)
        {
            var created = await _repo.AddAsync(model, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/rutinaplantillaejercicios/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RutinaPlantillaEjercicio model, CancellationToken ct)
        {
            var updated = await _repo.UpdateAsync(id, model, ct);
            return updated == null ? NotFound() : Ok(updated);
        }

        // DELETE: api/rutinaplantillaejercicios/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _repo.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
