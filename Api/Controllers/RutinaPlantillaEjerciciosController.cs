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

            // 🔍 Filtro opcional por nombre de rutina o ejercicio
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(rpe =>
                    rpe.RutinaPlantilla.Nombre.ToLower().Contains(term) ||
                    rpe.Ejercicio.Nombre.ToLower().Contains(term));
            }

            // 📊 Total antes de paginar
            var totalItems = await query.CountAsync(ct);

            // 📄 Paginado
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
                    rpe.DescansoSeg
                })
                .AsNoTracking()
                .ToListAsync(ct);

            return Ok(new
            {
                items,
                totalItems
            });
        }


        // ✅ GET: api/rutinasplantillaejercicios/5
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

        // ✅ POST: api/rutinasplantillaejercicios
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

        // ✅ PUT: api/rutinasplantillaejercicios/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RutinaPlantillaEjercicio model, CancellationToken ct)
        {
            var existing = await _context.RutinasPlantillaEjercicios.FindAsync(new object[] { id }, ct);
            if (existing == null)
                return NotFound();

            _context.Entry(existing).CurrentValues.SetValues(model);
            await _context.SaveChangesAsync(ct);
            return Ok(existing);
        }

        // ✅ DELETE: api/rutinasplantillaejercicios/5
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
