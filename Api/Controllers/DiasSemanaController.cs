using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepcionista")]
    [ApiController]
    [Route("api/[controller]")]
    public class DiasSemanaController : ControllerBase
    {
        private readonly IDiaSemanaRepository _repo;

        public DiasSemanaController(IDiaSemanaRepository repo)
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
        public async Task<IActionResult> GetById(byte id, CancellationToken ct = default)
        {
            var dia = await _repo.GetByIdAsync(id, ct);
            if (dia is null) return NotFound();
            return Ok(dia);
        }
    }
}
