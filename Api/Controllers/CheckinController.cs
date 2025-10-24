using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepcionista")]
    [ApiController]
    [Route("api/[controller]")]
    public class CheckinController : ControllerBase
    {
        private readonly GymDbContext _db;

        public CheckinController(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 POST: api/Checkin
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] Checkin dto, CancellationToken ct = default)
        {
            if (dto == null)
                return BadRequest(new { message = "Datos inválidos." });

            // 🔸 Validar existencia de socio y turno
            bool socioExiste = await _db.Socios.AnyAsync(s => s.Id == dto.SocioId, ct);
            bool turnoExiste = await _db.TurnosPlantilla.AnyAsync(t => t.Id == dto.TurnoPlantillaId, ct);

            if (!socioExiste || !turnoExiste)
                return BadRequest(new { message = "Socio o turno no válidos." });

            // 🔸 Evitar duplicados en el mismo día
            var yaRegistrado = await _db.Checkins.AnyAsync(
                c => c.SocioId == dto.SocioId
                   && c.TurnoPlantillaId == dto.TurnoPlantillaId
                   && c.FechaHora.Date == DateTime.UtcNow.Date,
                ct
            );

            if (yaRegistrado)
                return Conflict(new { message = "El check-in ya fue registrado hoy." });

            // 🔸 Registrar check-in
            dto.FechaHora = DateTime.UtcNow;
            dto.Origen ??= "recepcion"; // por defecto si no viene especificado

            _db.Checkins.Add(dto);
            await _db.SaveChangesAsync(ct);

            return Ok(new { ok = true, message = "✅ Check-in registrado correctamente." });
        }

        // 🔹 GET: api/Checkin/socio/{id}
        [HttpGet("socio/{id:int}")]
        public async Task<IActionResult> GetBySocio(int id, CancellationToken ct = default)
        {
            var registros = await _db.Checkins
                .AsNoTracking()
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(t => t.Sala)
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(t => t.DiaSemana)
                .Where(c => c.SocioId == id)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync(ct);

            return Ok(new
            {
                ok = true,
                total = registros.Count,
                items = registros
            });
        }
    }
}
