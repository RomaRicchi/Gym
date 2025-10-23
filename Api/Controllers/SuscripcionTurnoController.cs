using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepcionista")]
    [ApiController]
    [Route("api/[controller]")]
    public class SuscripcionesTurnoController : ControllerBase
    {
        private readonly ISuscripcionTurnoRepository _repo;
        private readonly GymDbContext _db;

        public SuscripcionesTurnoController(ISuscripcionTurnoRepository repo, GymDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        // 🔹 GET: api/SuscripcionesTurno
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var list = await _db.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .OrderByDescending(s => s.CreadoEn)
                .Select(s => new
                {
                    s.Id,
                    Socio = s.Socio != null ? s.Socio.Nombre : "(sin socio)",
                    Plan = s.Plan != null ? s.Plan.Nombre : "(sin plan)",
                    DiasPorSemana = s.Plan != null ? s.Plan.DiasPorSemana : 1,
                    s.Inicio,
                    s.Fin,
                    s.Estado
                })
                .ToListAsync(ct);

            return Ok(list);
        }


        // 🔹 GET: api/SuscripcionesTurno/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null)
                return NotFound(new { message = $"No se encontró el turno con ID {id}" });

            return Ok(entity);
        }

        // 🔹 GET: api/SuscripcionesTurno/suscripcion/{id}
        [HttpGet("suscripcion/{id:int}")]
        public async Task<IActionResult> GetBySuscripcion(int id, CancellationToken ct = default)
        {
            var list = await _repo.GetBySuscripcionAsync(id, ct);
            return Ok(list);
        }

        // 🔹 GET: api/SuscripcionesTurno/socio/{id}
        [HttpGet("socio/{id:int}")]
        public async Task<IActionResult> GetBySocio(int id, CancellationToken ct = default)
        {
            var list = await _repo.GetBySocioAsync(id, ct);
            return Ok(list);
        }

        // 🔹 POST: api/SuscripcionesTurno
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] SuscripcionTurno dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var nuevo = await _repo.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }

        // 🔹 PUT: api/SuscripcionesTurno/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] SuscripcionTurno dto, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null)
                return NotFound(new { message = $"No se encontró el turno con ID {id}" });

            entity.SuscripcionId = dto.SuscripcionId;
            entity.TurnoPlantillaId = dto.TurnoPlantillaId;

            await _repo.UpdateAsync(entity, ct);
            return Ok(new { ok = true, message = "Turno actualizado correctamente" });
        }

        // 🔹 DELETE: api/SuscripcionesTurno/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            await _repo.DeleteAsync(id, ct);
            return Ok(new { ok = true, message = "Turno eliminado correctamente" });
        }

        
    }
}
