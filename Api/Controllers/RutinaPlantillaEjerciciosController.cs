using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/rutinasplantillaejercicios")]
    [Authorize(Roles = "Administrador, Profesor")]
    public class RutinasPlantillaEjerciciosController : ControllerBase
    {
        private readonly GymDbContext _context;

        public RutinasPlantillaEjerciciosController(GymDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? q = null,
            CancellationToken ct = default)
        {
            var query = _context.RutinasPlantillaEjercicios
                .Include(rpe => rpe.RutinaPlantilla)
                .Include(rpe => rpe.Ejercicio)
                .AsQueryable();

            // Filtro opcional por nombre de rutina o ejercicio
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(rpe =>
                    rpe.RutinaPlantilla.Nombre.ToLower().Contains(term) ||
                    rpe.Ejercicio.Nombre.ToLower().Contains(term));
            }

            //  Total antes de paginar
            var totalItems = await query.CountAsync(ct);

            // Paginado + selección con imagen
            var items = await query
                .OrderBy(rpe => rpe.RutinaId)
                .ThenBy(rpe => rpe.Orden)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(rpe => new
                {
                    rpe.Id,
                    Rutina = rpe.RutinaPlantilla.Nombre,
                    Ejercicio = rpe.Ejercicio.Nombre,
                    rpe.Orden,
                    rpe.Series,
                    rpe.Repeticiones,
                    rpe.DescansoSeg,
                    ImagenUrl = rpe.Ejercicio.MediaUrl
                })
                .AsNoTracking()
                .ToListAsync(ct);

            return Ok(new
            {
                items,
                totalItems
            });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllSinPaginacion(CancellationToken ct = default)
        {
            var rutinas = await _context.RutinasPlantillaEjercicios
                .Include(rpe => rpe.RutinaPlantilla)
                .Include(rpe => rpe.Ejercicio)
                .Select(rpe => new
                {
                    rpe.Id,
                    Rutina = rpe.RutinaPlantilla.Nombre,
                    Ejercicio = rpe.Ejercicio.Nombre,
                    rpe.Orden,
                    rpe.Series,
                    rpe.Repeticiones,
                    rpe.DescansoSeg,
                    ImagenUrl = rpe.Ejercicio.MediaUrl 
                })
                .OrderBy(rpe => rpe.Rutina)
                .ThenBy(rpe => rpe.Orden)
                .AsNoTracking()
                .ToListAsync(ct);

            return Ok(rutinas);
        }


        // GET: api/rutinasplantillaejercicios/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var item = await _context.RutinasPlantillaEjercicios
                .Include(rpe => rpe.RutinaPlantilla)
                .Include(rpe => rpe.Ejercicio)
                .FirstOrDefaultAsync(rpe => rpe.Id == id, ct);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST: api/rutinasplantillaejercicios
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RutinaPlantillaEjercicio model, CancellationToken ct)
        {
            // Validar que la rutina y el ejercicio existan
            var rutinaExists = await _context.RutinasPlantilla.AnyAsync(r => r.Id == model.RutinaId, ct);
            var ejercicioExists = await _context.Ejercicios.AnyAsync(e => e.Id == model.EjercicioId, ct);

            if (!rutinaExists || !ejercicioExists)
                return BadRequest("Rutina o Ejercicio inexistente.");

            _context.RutinasPlantillaEjercicios.Add(model);
            await _context.SaveChangesAsync(ct);
            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RutinaPlantillaEjercicio model, CancellationToken ct)
        {
            if (model == null)
                return BadRequest(new { message = "Datos vacíos o inválidos." });

            var existing = await _context.RutinasPlantillaEjercicios
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (existing == null)
                return NotFound(new { message = "Ejercicio de rutina no encontrado." });

            // Validar existencia de FK
            bool rutinaOk = await _context.RutinasPlantilla.AnyAsync(r => r.Id == model.RutinaId, ct);
            bool ejercicioOk = await _context.Ejercicios.AnyAsync(e => e.Id == model.EjercicioId, ct);
            if (!rutinaOk || !ejercicioOk)
                return BadRequest(new { message = "Rutina o ejercicio inexistente." });

            // Actualizar campos editables
            existing.RutinaId = model.RutinaId;
            existing.EjercicioId = model.EjercicioId;
            existing.Orden = model.Orden;
            existing.Series = model.Series;
            existing.Repeticiones = model.Repeticiones;
            existing.DescansoSeg = model.DescansoSeg;

            await _context.SaveChangesAsync(ct);
            return Ok(new { ok = true, message = "Registro actualizado correctamente." });
        }


        // DELETE: api/rutinasplantillaejercicios/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var existing = await _context.RutinasPlantillaEjercicios.FindAsync(new object[] { id }, ct);
            if (existing == null)
                return NotFound();

            _context.RutinasPlantillaEjercicios.Remove(existing);
            await _context.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
