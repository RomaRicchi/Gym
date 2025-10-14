using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuscripcionesController : ControllerBase
    {
        private readonly ISuscripcionRepository _repo;

        public SuscripcionesController(ISuscripcionRepository repo)
        {
            _repo = repo;
        }

        // 🔹 GET: api/suscripciones
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return Ok(list);
        }

        // 🔹 GET: api/suscripciones/activas
        [HttpGet("activas")]
        public async Task<IActionResult> GetActivas(CancellationToken ct = default)
        {
            var list = await _repo.GetActivasAsync(ct);
            return Ok(list);
        }

        // 🔹 GET: api/suscripciones/socio/{id}
        [HttpGet("socio/{id}")]
        public async Task<IActionResult> GetBySocio([FromRoute] int id, CancellationToken ct = default)
        {
            var list = await _repo.GetBySocioAsync(id, ct);
            return Ok(list);
        }

        // 🔹 GET: api/suscripciones/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct = default)
        {
            var sus = await _repo.GetByIdAsync(id, ct);
            if (sus is null)
                return NotFound("Suscripción no encontrada");
            return Ok(sus);
        }

        // 🔹 POST: api/suscripciones
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Suscripcion dto, CancellationToken ct = default)
        {
            if (dto == null)
                return BadRequest("Datos inválidos");

            // 🔸 Validar duplicado
            var existe = await _repo.GetActivaByPlanAsync(dto.SocioId, dto.PlanId, ct);
            if (existe is not null)
                return Conflict("Ya existe una suscripción activa para este plan.");

            dto.Estado = true; // ✅ activa
            dto.CreadoEn = DateTime.UtcNow;

            var nueva = await _repo.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = nueva.Id }, nueva);
        }


        // 🔹 PUT: api/suscripciones/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar([FromRoute] int id, [FromBody] Suscripcion dto, CancellationToken ct = default)
        {
            var sus = await _repo.GetByIdAsync(id, ct);
            if (sus is null)
                return NotFound("Suscripción no encontrada");

            sus.PlanId = dto.PlanId;
            sus.Inicio = dto.Inicio;
            sus.Fin = dto.Fin;
            sus.Estado = dto.Estado;
            await _repo.UpdateAsync(sus, ct);

            return Ok(new { ok = true, mensaje = "Suscripción actualizada correctamente" });
        }

        // PATCH: api/suscripciones/{id}/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado([FromRoute] int id, [FromBody] bool nuevoEstado, CancellationToken ct = default)
        {
            var sus = await _repo.GetByIdAsync(id, ct);
            if (sus is null)
                return NotFound("Suscripción no encontrada");

            sus.Estado = nuevoEstado;
            await _repo.UpdateAsync(sus, ct);

            return Ok(new { ok = true, id, nuevoEstado });
        }


        // 🔹 DELETE: api/suscripciones/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar([FromRoute] int id, CancellationToken ct = default)
        {
            var sus = await _repo.GetByIdAsync(id, ct);
            if (sus is null)
                return NotFound("Suscripción no encontrada");

            await _repo.DeleteAsync(id, ct);
            return Ok(new { ok = true, mensaje = "Suscripción eliminada correctamente" });
        }
    }
}
