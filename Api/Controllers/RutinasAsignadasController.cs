using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/rutinasasignadas")]
    [Authorize(Roles = "Administrador, Profesor")]
    public class RutinasAsignadasController : ControllerBase
    {
        private readonly IRutinaAsignadaRepository _repo;

        public RutinasAsignadasController(IRutinaAsignadaRepository repo)
        {
            _repo = repo;
        }

        // GET: api/rutinasasignadas
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = await _repo.GetAllAsync(ct);
            return Ok(list);
        }

        // GET: api/rutinasasignadas/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var item = await _repo.GetByIdAsync(id, ct);
            return item == null ? NotFound() : Ok(item);
        }

        // GET: api/rutinasasignadas/socio/{socioId}
        [HttpGet("socio/{socioId:int}")]
        public async Task<IActionResult> GetBySocioId(int socioId, CancellationToken ct)
        {
            var list = await _repo.GetBySocioIdAsync(socioId, ct);
            return Ok(list);
        }

        // POST: api/rutinasasignadas
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RutinaAsignada model, CancellationToken ct)
        {
            var created = await _repo.AddAsync(model, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/rutinasasignadas/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RutinaAsignada model, CancellationToken ct)
        {
            var updated = await _repo.UpdateAsync(id, model, ct);
            return updated == null ? NotFound() : Ok(updated);
        }

        // DELETE: api/rutinasasignadas/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _repo.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
