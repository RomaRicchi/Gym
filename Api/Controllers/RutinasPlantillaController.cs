using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/rutinasplantilla")]
    [Authorize(Roles = "Administrador, Profesor")]
    public class RutinasPlantillaController : ControllerBase
    {
        private readonly IRutinaPlantillaRepository _repo;

        public RutinasPlantillaController(IRutinaPlantillaRepository repo)
        {
            _repo = repo;
        }

        // GET: api/rutinasplantilla
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = await _repo.GetAllAsync(ct);
            return Ok(list);
        }

        // GET: api/rutinasplantilla/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var item = await _repo.GetByIdAsync(id, ct);
            return item == null ? NotFound() : Ok(item);
        }

        // POST: api/rutinasplantilla
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RutinaPlantilla model, CancellationToken ct)
        {
            var created = await _repo.AddAsync(model, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/rutinasplantilla/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RutinaPlantilla model, CancellationToken ct)
        {
            var updated = await _repo.UpdateAsync(id, model, ct);
            return updated == null ? NotFound() : Ok(updated);
        }

        // DELETE: api/rutinasplantilla/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _repo.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
