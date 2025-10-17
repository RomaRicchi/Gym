using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("por-plan/{planId:int:min(1)}")]
        public async Task<IActionResult> GetByPlanPaginado(
            [FromRoute] int planId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? activo = true,
            CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _repo.Query()
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .Where(s => s.PlanId == planId);

            // ✅ ahora sí, el compilador reconoce “activo”
            if (activo.HasValue)
                query = query.Where(s => s.Estado == activo.Value);

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(s => s.CreadoEn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new
                {
                    s.Id,
                    Socio = s.Socio != null ? s.Socio.Nombre : null,
                    Plan = s.Plan != null ? s.Plan.Nombre : null,
                    s.Inicio,
                    s.Fin,
                    s.Estado,
                    s.CreadoEn
                })
                .ToListAsync(ct);

            if (!items.Any())
                return NotFound($"No hay suscripciones asociadas al plan con ID {planId}.");

            return Ok(new
            {
                planId,
                activo,
                total,
                page,
                pageSize,
                items
            });
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
