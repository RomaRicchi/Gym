using Gym.Application.DTOs.Turnos;
using Gym.Domain.Entities;
using Gym.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.API.Controllers;

[Authorize]
[ApiController]
[Route("api/checkin")]
public class CheckinsController : ControllerBase
{
    private readonly GymDbContext _db;

    public CheckinsController(GymDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CheckinCreateRequest request, CancellationToken ct)
    {
        var socio = await _db.Socios.FindAsync(new object[] { request.SocioId }, ct);
        if (socio == null)
            return NotFound(new { message = "Socio no encontrado." });

        var turno = await _db.TurnosPlantilla
            .Include(t => t.Personal)
            .FirstOrDefaultAsync(t => t.Id == request.TurnoPlantillaId, ct);
        if (turno == null)
            return NotFound(new { message = "Turno no encontrado." });

        var hoy = DateTime.UtcNow.Date;
        var asignacion = await _db.SuscripcionTurnos
            .Include(st => st.Suscripcion)
            .FirstOrDefaultAsync(st =>
                st.TurnoPlantillaId == request.TurnoPlantillaId &&
                st.Suscripcion.SocioId == request.SocioId &&
                st.Suscripcion.Estado &&
                st.Suscripcion.Fin >= hoy, ct);

        if (asignacion == null)
            return BadRequest(new { message = "El socio no tiene este turno asignado o su suscripción está inactiva." });

        var existe = await _db.Checkins.AnyAsync(c =>
            c.SocioId == request.SocioId &&
            c.TurnoPlantillaId == request.TurnoPlantillaId &&
            c.FechaHora.Date == hoy, ct);

        if (existe)
            return Conflict(new { message = "El check-in ya fue registrado para hoy." });

        var checkin = new Checkin
        {
            SocioId = request.SocioId,
            TurnoPlantillaId = request.TurnoPlantillaId,
            ProfesorId = request.ProfesorId ?? turno.PersonalId,
            Observaciones = request.Observaciones,
            FechaHora = DateTime.UtcNow
        };

        _db.Checkins.Add(checkin);
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Check-in registrado correctamente.", id = checkin.Id });
    }
}
