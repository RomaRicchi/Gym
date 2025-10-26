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
            var turnos = await _repo.GetByDiaSemanaAsync((byte)diaSemana, ct);

            if (turnos == null || !turnos.Any())
                return NotFound(new { message = "No se encontraron turnos para ese día." });

            return Ok(turnos);
        }

     
[HttpPost]
public async Task<IActionResult> Crear([FromBody] TurnoPlantilla dto, CancellationToken ct = default)
{
    try
    {
        Console.WriteLine($"➡️ Recibido turno:");
        Console.WriteLine($"   SalaId={dto.SalaId}, PersonalId={dto.PersonalId}, DiaSemanaId={dto.DiaSemanaId}");
        Console.WriteLine($"   HoraInicio={dto.HoraInicio}, DuracionMin={dto.DuracionMin}, Cupo={dto.Cupo}, Activo={dto.Activo}");

        // 🔹 Ajuste de compatibilidad para hora_inicio si falla el binder
        if (dto.HoraInicio == TimeSpan.Zero && Request.Body.CanSeek)
        {
            Request.Body.Position = 0;
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            if (body.Contains("hora_inicio"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(body, "\"hora_inicio\"\\s*:\\s*\"([0-9:\\-T]+)\"");
                if (match.Success)
                {
                    var hora = match.Groups[1].Value;
                    if (TimeSpan.TryParse(hora, out var ts))
                    {
                        dto.HoraInicio = ts;
                        Console.WriteLine($"🕒 HoraInicio parseada manualmente: {dto.HoraInicio}");
                    }
                }
            }
        }

        // ✅ Validaciones FK con logs
        Console.WriteLine($"🧩 Validando IDs -> Sala: {dto.SalaId}, Personal: {dto.PersonalId}, Día: {dto.DiaSemanaId}");

        if (!await _db.Salas.AnyAsync(s => s.Id == dto.SalaId))
        {
            Console.WriteLine("🚫 Sala no encontrada");
            return BadRequest(new { message = "La sala no existe." });
        }

        if (!await _db.Personales.AnyAsync(p => p.Id == dto.PersonalId && p.Estado == true))
            return BadRequest(new { message = "El personal seleccionado no está activo o no es válido para turnos." });


        // 🔹 Cast explícito para coincidir con tinyint unsigned (byte en el modelo)
        // El DiaSemanaId 3 (Miércoles) que se ve en el log existe en la DB.
        if (!await _db.DiasSemana.AnyAsync(d => d.Id == (byte)dto.DiaSemanaId))
        {
            Console.WriteLine("🚫 Día de semana no encontrado");
            return BadRequest(new { message = "El día de semana no existe." }); // Este error es engañoso si la FK existe
        }

        // === 🛑 NUEVA VALIDACIÓN: Solapamiento (Resuelve el HTTP 400 por duplicidad/UNIQUE KEY) ===
        // El repositorio ya tiene la lógica de solapamiento. La implementamos aquí.
        var existeSolapamiento = await _repo.ExisteSolapamientoAsync(
            dto.SalaId, 
            dto.DiaSemanaId, // DiaSemanaId es byte, coincide con el repositorio
            dto.HoraInicio, 
            dto.DuracionMin, 
            ct
        );

        if (existeSolapamiento)
        {
            Console.WriteLine("🚫 Error: Solapamiento detectado.");
            // Devolvemos un mensaje claro al usuario.
            return BadRequest(new { 
                message = "Ya existe un turno activo que se solapa con el horario solicitado en esta sala y día. Revise Sala, Día y Horario." 
            });
        }
        // =======================================================================================

        Console.WriteLine($"✅ Validaciones completadas. Insertando turno...");

        var nuevo = await _repo.AddAsync(dto, ct);
        Console.WriteLine($"✅ Turno creado con ID: {nuevo.Id}");

        return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
    }
    catch (DbUpdateException dbEx)
    {
        // ❌ Este catch ahora sólo se debería ejecutar para otros errores DB (no solapamiento/duplicidad)
        Console.WriteLine("❌ Error DB (detallado): " + dbEx.ToString());
        return BadRequest(new { message = dbEx.InnerException?.Message ?? dbEx.Message });
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error General: " + ex.Message);
        return BadRequest(new { message = ex.Message });
    }
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
