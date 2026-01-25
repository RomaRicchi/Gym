using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize(Roles = "Socio, Administrador, Profesor")]
    [ApiController]
    [Route("api/evolucionfisica")]
    public class EvolucionFisicaController : ControllerBase
    {
        private readonly GymDbContext _db;

        public EvolucionFisicaController(GymDbContext db)
        {
            _db = db;
        }

        // ✅ POST: api/evolucionfisica
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] EvolucionFisica dto, CancellationToken ct)
        {
            try
            {
                if (dto == null || dto.SocioId == 0)
                    return BadRequest(new { message = "Datos incompletos o socio inválido." });

                // 🔹 Cálculos automáticos
                var alturaM = dto.Altura / 100; // altura en metros
                var imc = alturaM > 0 ? dto.Peso / (alturaM * alturaM) : 0;
                var pesoIdeal = 22 * (alturaM * alturaM);

                var nuevo = new EvolucionFisica
                {
                    SocioId = dto.SocioId,
                    Fecha = DateTime.UtcNow,
                    Peso = dto.Peso,
                    Altura = dto.Altura,
                    Pecho = dto.Pecho,
                    Cintura = dto.Cintura,
                    Cadera = dto.Cadera,
                    Brazo = dto.Brazo,
                    Pierna = dto.Pierna,
                    Gemelo = dto.Gemelo,
                    Observacion = dto.Observacion,
                    IMC = Math.Round((decimal)imc, 2),
                    PesoIdeal = Math.Round((decimal)pesoIdeal, 2)
                };

                _db.EvolucionFisica.Add(nuevo);
                await _db.SaveChangesAsync(ct);

                return Ok(nuevo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al registrar evolución física: " + ex.Message);
                return StatusCode(500, new { message = "Error interno al registrar." });
            }
        }

        // ✅ GET: api/evolucionfisica/socio/{id}
        [HttpGet("socio/{socioId:int}")]
        public async Task<IActionResult> GetBySocio(int socioId, CancellationToken ct)
        {
            try
            {
                var socio = await _db.Socios.FindAsync(new object[] { socioId }, ct);
                if (socio == null)
                    return NotFound(new { message = "Socio no encontrado." });

                var list = await _db.EvolucionFisica
                    .Where(e => e.SocioId == socioId)
                    .OrderByDescending(e => e.Fecha)
                    .ToListAsync(ct);

                return Ok(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al obtener evolución física: " + ex.Message);
                return StatusCode(500, new { message = "Error interno al obtener los registros." });
            }
        }

        // ✅ PUT: api/evolucionfisica/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Editar(int id, [FromBody] EvolucionFisica dto, CancellationToken ct)
        {
            try
            {
                var entidad = await _db.EvolucionFisica.FindAsync(new object[] { id }, ct);
                if (entidad == null)
                    return NotFound(new { message = "Registro no encontrado." });

                // Actualizar campos
                entidad.Peso = dto.Peso;
                entidad.Altura = dto.Altura;
                entidad.Pecho = dto.Pecho;
                entidad.Cintura = dto.Cintura;
                entidad.Cadera = dto.Cadera;
                entidad.Brazo = dto.Brazo;
                entidad.Pierna = dto.Pierna;
                entidad.Gemelo = dto.Gemelo;
                entidad.Observacion = dto.Observacion;

                // Recalcular valores
                var alturaM = entidad.Altura / 100;
                var imc = alturaM > 0 ? entidad.Peso / (alturaM * alturaM) : 0;
                var pesoIdeal = 22 * (alturaM * alturaM);

                entidad.IMC = Math.Round((decimal)imc, 2);
                entidad.PesoIdeal = Math.Round((decimal)pesoIdeal, 2);

                await _db.SaveChangesAsync(ct);

                return Ok(entidad);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al editar evolución física: " + ex.Message);
                return StatusCode(500, new { message = "Error interno al editar." });
            }
        }

        // ✅ DELETE: api/evolucionfisica/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            try
            {
                var entidad = await _db.EvolucionFisica.FindAsync(new object[] { id }, ct);
                if (entidad == null)
                    return NotFound(new { message = "Registro no encontrado." });

                _db.EvolucionFisica.Remove(entidad);
                await _db.SaveChangesAsync(ct);

                return Ok(new { message = "Registro eliminado correctamente." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al eliminar evolución física: " + ex.Message);
                return StatusCode(500, new { message = "Error interno al eliminar." });
            }
        }
    }
}
