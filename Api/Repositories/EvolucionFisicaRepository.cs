using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class EvolucionFisicaRepository : IEvolucionFisicaRepository
    {
        private readonly GymDbContext _db;

        public EvolucionFisicaRepository(GymDbContext db)
        {
            _db = db;
        }

        // ✅ Crear registro con cálculo automático de IMC y peso ideal
        public async Task<EvolucionFisica> CrearAsync(EvolucionFisica entidad, CancellationToken ct)
        {
            // Conversión cm → metros para fórmulas
            var alturaMetros = entidad.Altura / 100;

            entidad.IMC = Math.Round(alturaMetros > 0 ? 
                entidad.Peso / (decimal)Math.Pow((double)alturaMetros, 2) : 0, 2);

            entidad.PesoIdeal = Math.Round(22 * (decimal)Math.Pow((double)alturaMetros, 2), 2);

            _db.EvolucionFisica.Add(entidad);
            await _db.SaveChangesAsync(ct);
            return entidad;
        }

        // ✅ Obtener registros de un socio
        public async Task<List<EvolucionFisica>> GetBySocioAsync(int socioId, CancellationToken ct)
        {
            return await _db.EvolucionFisica
                .Where(e => e.SocioId == socioId)
                .OrderByDescending(e => e.Fecha)
                .ToListAsync(ct);
        }

        // ✅ Obtener registro por ID
        public async Task<EvolucionFisica?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _db.EvolucionFisica.FindAsync(new object[] { id }, ct);
        }

        // ✅ Actualizar con recálculo automático de IMC y peso ideal
        public async Task<bool> ActualizarAsync(EvolucionFisica dto, CancellationToken ct)
        {
            var entidad = await _db.EvolucionFisica.FindAsync(new object[] { dto.Id }, ct);
            if (entidad == null)
                return false;

            entidad.Peso = dto.Peso;
            entidad.Altura = dto.Altura;
            entidad.Pecho = dto.Pecho;
            entidad.Cintura = dto.Cintura;
            entidad.Cadera = dto.Cadera;
            entidad.Brazo = dto.Brazo;
            entidad.Pierna = dto.Pierna;
            entidad.Gemelo = dto.Gemelo;
            entidad.Observacion = dto.Observacion;

            // 🔥 Recalcular IMC y peso ideal
            var alturaMetros = entidad.Altura / 100;
            entidad.IMC = Math.Round(alturaMetros > 0 ?
                entidad.Peso / (decimal)Math.Pow((double)alturaMetros, 2) : 0, 2);

            entidad.PesoIdeal = Math.Round(22 * (decimal)Math.Pow((double)alturaMetros, 2), 2);

            await _db.SaveChangesAsync(ct);
            return true;
        }

        // ✅ Eliminar registro
        public async Task<bool> EliminarAsync(int id, CancellationToken ct)
        {
            var entidad = await _db.EvolucionFisica.FindAsync(new object[] { id }, ct);
            if (entidad == null)
                return false;

            _db.EvolucionFisica.Remove(entidad);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
