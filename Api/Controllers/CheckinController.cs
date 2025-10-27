using Api.Data;
using Api.Data.Models;
using Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepción")]
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
        public async Task<IActionResult> Registrar([FromBody] CheckinDto dto, CancellationToken ct = default)
        {
            if (dto == null)
                return BadRequest(new { message = "Datos inválidos." });

            bool socioExiste = await _db.Socios.AnyAsync(s => s.Id == dto.SocioId, ct);
            bool turnoExiste = await _db.TurnosPlantilla.AnyAsync(t => t.Id == dto.TurnoPlantillaId, ct);

            if (!socioExiste || !turnoExiste)
                return BadRequest(new { message = "Socio o turno no válidos." });

            // Crear nuevo registro
            var checkin = new Checkin
            {
                SocioId = dto.SocioId,
                TurnoPlantillaId = dto.TurnoPlantillaId,
                FechaHora = DateTime.UtcNow
            };

            _db.Checkins.Add(checkin);
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
