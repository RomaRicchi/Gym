using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;



namespace Api.Controllers
{
    [Authorize(Roles = "Administrador, Profesor, Recepcionista")]
    [ApiController]
    [Route("api/turnosplantilla")] 
    public class TurnosPlantillaController : ControllerBase
    {
        private readonly ITurnoPlantillaRepository _repo;
         private readonly GymDbContext _db;

        public TurnosPlantillaController(ITurnoPlantillaRepository repo, GymDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos(CancellationToken ct = default)
        {
            var list = await _repo.GetActivosAsync(ct);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var turno = await _repo.GetByIdAsync(id, ct);
            if (turno is null) return NotFound();
            return Ok(turno);
        }

        [HttpGet("buscar-por-dia/{diaSemana}")]
        public async Task<IActionResult> GetByDiaSemana(byte diaSemana, CancellationToken ct = default)
        {
            var turnos = await _repo.GetByDiaSemanaAsync((sbyte)diaSemana, ct);

            if (turnos == null || !turnos.Any())
                return NotFound(new { message = "No se encontraron turnos para ese día." });

            return Ok(turnos);
        }


        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] TurnoPlantilla dto, CancellationToken ct = default)
        {
            var nuevo = await _repo.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }
 

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] TurnoPlantilla dto, CancellationToken ct = default)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "El ID del turno no coincide con el de la URL." });

            var turno = await _repo.GetByIdAsync(id, ct);
            if (turno is null)
                return NotFound(new { message = "Turno no encontrado." });

            turno.SalaId = dto.SalaId;
            turno.PersonalId = dto.PersonalId;
            turno.DiaSemanaId = dto.DiaSemanaId; 
            turno.HoraInicio = dto.HoraInicio;
            turno.DuracionMin = dto.DuracionMin;
            turno.Cupo = dto.Cupo;
            turno.Activo = dto.Activo;

            await _repo.UpdateAsync(turno, ct);
            return Ok(new { ok = true, message = "Turno actualizado correctamente." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            await _repo.DeleteAsync(id, ct);
            return Ok(new { ok = true });
        }

        [HttpGet("dia/{id:int}")]
        public async Task<IActionResult> GetByDia(int id, CancellationToken ct = default)
        {
            var turnos = await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .Where(t => t.DiaSemanaId == id && t.Activo)
                .OrderBy(t => t.HoraInicio)
                .Select(t => new
                {
                    t.Id,
                    Sala = t.Sala != null ? t.Sala.Nombre : "(sin sala)",
                    Profesor = t.Personal != null ? t.Personal.Nombre : "(sin profesor)",
                    Dia = t.DiaSemana.Nombre,
                    t.HoraInicio,
                    t.DuracionMin,
                    t.Cupo,
                    // Si tenés la vista v_cupo_reservado:
                    Reservados = _db.VCupoReservado
                        .Where(v => v.TurnoId == t.Id)
                        .Select(v => (int?)v.Reservados)
                        .FirstOrDefault() ?? 0,
                    Disponibles = t.Cupo - (
                        _db.VCupoReservado
                            .Where(v => v.TurnoId == t.Id)
                            .Select(v => (int?)v.Reservados)
                            .FirstOrDefault() ?? 0
                    )
                })
                .Where(t => t.Disponibles > 0)
                .ToListAsync(ct);

            return Ok(turnos);
        }

    }
}
