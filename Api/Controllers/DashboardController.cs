using Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly GymDbContext _db;

        public DashboardController(GymDbContext db)
        {
            _db = db;
        }

        // 📅 Suscripciones activas por mes
        [HttpGet("suscripciones-por-mes")]
        public IActionResult GetSuscripcionesPorMes()
        {
            try
            {
                var result = _db.VSuscripcionesAr
                    .AsEnumerable() // ✅ fuerza evaluación en memoria
                    .GroupBy(s => new { s.inicio_ar.Year, s.inicio_ar.Month })
                    .Select(g => new
                    {
                        mes = new DateTime(g.Key.Year, g.Key.Month, 1),
                        cantidad = g.Count()
                    })
                    .OrderBy(g => g.mes)
                    .ToList();

                var formatted = result.Select(r => new
                {
                    mes = r.mes.ToString("yyyy-MM"),
                    nombreMes = r.mes.ToString("MMMM yyyy", new System.Globalization.CultureInfo("es-ES")),
                    cantidad = r.cantidad
                });

                return Ok(formatted);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        // 🏋️‍♀️ Salas con más reservas
        [HttpGet("salas-mas-reservadas")]
        public async Task<IActionResult> GetSalasMasReservadas()
        {
            try
            {
                var result = await _db.VCupoReservado
                    .Join(_db.TurnosPlantilla,
                          v => v.turno_id,
                          t => t.Id,
                          (v, t) => new { v, t })
                    .Join(_db.Salas,
                          vt => vt.t.SalaId,
                          s => s.Id,
                          (vt, s) => new { vt.v, s })
                    .GroupBy(x => x.s.Nombre)
                    .Select(g => new
                    {
                        sala = g.Key,
                        reservas = g.Sum(x => (int?)x.v.reservados ?? 0) // ✅ COALESCE equivalente
                    })
                    .OrderByDescending(g => g.reservas)
                    .Take(10)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        // Ingresos mensuales (solo administrador)
        [HttpGet("ingresos-mensuales")]
        public IActionResult GetIngresosMensuales()
        {
            try
            {
                var result = _db.VSuscripcionesAr
                    .Join(_db.Planes,
                        v => v.plan_id,
                        p => p.Id,
                        (v, p) => new { v, p })
                    .AsEnumerable() 
                    .GroupBy(x => new { x.v.inicio_ar.Year, x.v.inicio_ar.Month })
                    .Select(g => new
                    {
                        mes = new DateTime(g.Key.Year, g.Key.Month, 1),
                        ingresos = g.Sum(x => (decimal)x.p.Precio)
                    })
                    .OrderBy(g => g.mes)
                    .ToList();

                var formatted = result.Select(r => new
                {
                    mes = r.mes.ToString("yyyy-MM"),
                    nombreMes = r.mes.ToString("MMMM yyyy", new System.Globalization.CultureInfo("es-ES")),
                    ingresos = r.ingresos
                });

                return Ok(formatted);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }

    }
}
