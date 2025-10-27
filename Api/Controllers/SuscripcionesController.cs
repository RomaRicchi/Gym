using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepcionista")]
    [ApiController]
    [Route("api/[controller]")]
    public class SuscripcionesController : ControllerBase
    {
        private readonly ISuscripcionRepository _repo;
        private readonly GymDbContext _db;

        public SuscripcionesController(ISuscripcionRepository repo, GymDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            int page = 1,
            int pageSize = 10,
            int? socioId = null,
            CancellationToken ct = default)
        {
            // 🧩 Base query
            var query = _db.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .AsNoTracking()
                .OrderByDescending(s => s.CreadoEn)
                .AsQueryable();

            // 🔹 Filtro por socio
            if (socioId.HasValue)
                query = query.Where(s => s.SocioId == socioId.Value);

            // 📊 Total de registros antes de paginar
            var totalItems = await query.CountAsync(ct);

            // 🧮 Paginación
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new
                {
                    s.Id,
                    socio = s.Socio != null ? s.Socio.Nombre : "(sin socio)",
                    plan = s.Plan != null ? s.Plan.Nombre : "(sin plan)",
                    s.PlanId,
                    s.Inicio,
                    s.Fin,
                    s.Estado,
                    s.CreadoEn,
                    s.OrdenPagoId,
                    // 🧩 Estos valores se pueden calcular en el frontend si querés
                    TurnosAsignados = _db.SuscripcionTurnos.Count(t => t.SuscripcionId == s.Id),
                    CupoMaximo = _db.Planes
                        .Where(p => p.Id == s.PlanId)
                        .Select(p => p.DiasPorSemana)
                        .FirstOrDefault()
                })
                .ToListAsync(ct);

            // ✅ Devuelve en formato esperado
            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                items
            });
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
        // ✅ GET /api/suscripciones?socioId=5
        public async Task<IActionResult> GetBySocio([FromQuery] int socioId, CancellationToken ct)
        {
            var suscripciones = await _db.Suscripciones
                .Include(s => s.Plan)
                .Where(s => s.SocioId == socioId)
                .OrderByDescending(s => s.CreadoEn)
                .Select(s => new
                {
                    s.Id,
                    s.Inicio,
                    s.Fin,
                    s.Estado,
                    Plan = new { s.Plan.Id, s.Plan.Nombre }
                })
                .ToListAsync(ct);

            return Ok(suscripciones);
        }


        // 🔹 GET: api/suscripciones/por-orden/{ordenId}
        [HttpGet("por-orden/{ordenId:int:min(1)}")]
        public async Task<IActionResult> GetByOrdenPago([FromRoute] int ordenId, CancellationToken ct = default)
        {
            var sus = await _repo.GetByOrdenPagoAsync(ordenId, ct);
            if (sus is null)
                return NotFound($"No se encontró suscripción asociada a la orden #{ordenId}");
            return Ok(sus);
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
        public async Task<IActionResult> Crear([FromBody] SuscripcionCreateDto dto, CancellationToken ct = default)
        {
            if (dto == null)
                return BadRequest("Datos inválidos.");

            // 🧩 Validar existencia del socio
            var socioExiste = await _repo.Query()
                .Include(s => s.Socio)
                .AnyAsync(s => s.SocioId == dto.SocioId, ct);

            if (!socioExiste)
                return BadRequest($"El socio con ID {dto.SocioId} no existe.");

            // 🧩 Validar existencia del plan
            var planExiste = await _repo.Query()
                .Include(s => s.Plan)
                .AnyAsync(s => s.PlanId == dto.PlanId, ct);

            if (!planExiste)
                return BadRequest($"El plan con ID {dto.PlanId} no existe.");

            // 🧩 Validar existencia de la orden de pago
            if (dto.OrdenPagoId == null)
                return BadRequest("Debe especificar una orden de pago válida.");

            var orden = await _repo.Query()
                .Select(s => s.OrdenPago)
                .Where(o => o != null && o.Id == dto.OrdenPagoId)
                .FirstOrDefaultAsync(ct);

            if (orden == null)
                return BadRequest($"La orden de pago #{dto.OrdenPagoId} no existe o fue eliminada.");

            // 🔍 Verificar que no exista una suscripción activa para el mismo socio y plan
            var existe = await _repo.GetActivaByPlanAsync(dto.SocioId, dto.PlanId, ct);
            if (existe is not null)
                return Conflict("Ya existe una suscripción activa para este plan.");

            // ✅ Crear la nueva suscripción
            var entity = new Suscripcion
            {
                SocioId = dto.SocioId,
                PlanId = dto.PlanId,
                OrdenPagoId = dto.OrdenPagoId,
                Inicio = dto.Inicio,
                Fin = dto.Fin,
                Estado = true,
                CreadoEn = DateTime.UtcNow
            };

            var nueva = await _repo.AddAsync(entity, ct);

            return CreatedAtAction(nameof(GetById), new { id = nueva.Id }, new
            {
                nueva.Id,
                nueva.SocioId,
                nueva.PlanId,
                nueva.OrdenPagoId,
                nueva.Inicio,
                nueva.Fin,
                nueva.Estado
            });
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
            sus.OrdenPagoId = dto.OrdenPagoId; // 🆕 actualizar vínculo
            await _repo.UpdateAsync(sus, ct);

            return Ok(new { ok = true, mensaje = "Suscripción actualizada correctamente" });
        }

        // 🔹 PATCH: api/suscripciones/{id}/estado
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

        // 🔹 GET: api/suscripciones/vencen-semana
        [HttpGet("vencen-semana")]
        public async Task<IActionResult> GetSuscripcionesQueVencenEstaSemana(CancellationToken ct = default)
        {
            var hoy = DateTime.UtcNow;
            var finSemana = hoy.AddDays(7);

            var suscripciones = await _repo.Query()
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .Where(s => s.Fin >= hoy && s.Fin <= finSemana)
                .OrderBy(s => s.Fin)
                .Select(s => new
                {
                    s.Id,
                    Socio = s.Socio != null ? s.Socio.Nombre : null,
                    Plan = s.Plan != null ? s.Plan.Nombre : null,
                    s.Inicio,
                    s.Fin,
                    s.Estado,
                    DiasRestantes = EF.Functions.DateDiffDay(hoy, s.Fin),
                    s.CreadoEn
                })
                .ToListAsync(ct);

            if (!suscripciones.Any())
                return NotFound("No hay suscripciones que venzan esta semana.");

            return Ok(new
            {
                Desde = hoy,
                Hasta = finSemana,
                Total = suscripciones.Count,
                Suscripciones = suscripciones
            });
        }
    }
}
