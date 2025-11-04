using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories
{
    public class RutinaPlantillaEjercicioRepository : IRutinaPlantillaEjercicioRepository
    {
        private readonly GymDbContext _db;

        public RutinaPlantillaEjercicioRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 Obtener todos los registros
        public async Task<IEnumerable<RutinaPlantillaEjercicio>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.RutinaPlantillaEjercicios
                .Include(rpe => rpe.Ejercicio)
                .Include(rpe => rpe.RutinaPlantilla) // ✅ propiedad correcta
                .AsNoTracking()
                .ToListAsync(ct);
        }

        // 🔹 Obtener un registro por ID
        public async Task<RutinaPlantillaEjercicio?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.RutinaPlantillaEjercicios
                .Include(rpe => rpe.Ejercicio)
                .Include(rpe => rpe.RutinaPlantilla) // ✅ propiedad correcta
                .AsNoTracking()
                .FirstOrDefaultAsync(rpe => rpe.Id == id, ct);
        }

        // 🔹 Obtener todos los ejercicios de una rutina
        public async Task<IEnumerable<RutinaPlantillaEjercicio>> GetByRutinaIdAsync(int rutinaId, CancellationToken ct = default)
        {
            return await _db.RutinaPlantillaEjercicios
                .Include(rpe => rpe.Ejercicio)
                .Where(rpe => rpe.RutinaId == rutinaId)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        // 🔹 Agregar un nuevo registro
        public async Task<RutinaPlantillaEjercicio> AddAsync(RutinaPlantillaEjercicio entity, CancellationToken ct = default)
        {
            _db.RutinaPlantillaEjercicios.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // 🔹 Actualizar un registro existente
        public async Task<RutinaPlantillaEjercicio?> UpdateAsync(int id, RutinaPlantillaEjercicio entity, CancellationToken ct = default)
        {
            var existing = await _db.RutinaPlantillaEjercicios.FindAsync(new object[] { id }, ct);
            if (existing == null) return null;

            existing.RutinaId = entity.RutinaId;
            existing.EjercicioId = entity.EjercicioId;
            existing.Orden = entity.Orden;
            existing.Series = entity.Series;
            existing.Repeticiones = entity.Repeticiones;
            existing.DescansoSeg = entity.DescansoSeg;

            await _db.SaveChangesAsync(ct);
            return existing;
        }

        // 🔹 Eliminar un registro
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var existing = await _db.RutinaPlantillaEjercicios.FindAsync(new object[] { id }, ct);
            if (existing == null) return false;

            _db.RutinaPlantillaEjercicios.Remove(existing);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
