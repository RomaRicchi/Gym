using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories
{
    public class RutinaPlantillaRepository : IRutinaPlantillaRepository
    {
        private readonly GymDbContext _db;

        public RutinaPlantillaRepository(GymDbContext db)
        {
            _db = db;
        }

    public async Task<IEnumerable<RutinaPlantilla>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.RutinasPlantilla
            .Include(r => r.RutinaPlantillaEjercicios)
                .ThenInclude(rpe => rpe.Ejercicio)
            .ToListAsync(ct);
    }

    public async Task<RutinaPlantilla?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.RutinasPlantilla
            .Include(r => r.RutinaPlantillaEjercicios)
                .ThenInclude(rpe => rpe.Ejercicio)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

        public async Task<RutinaPlantilla> AddAsync(RutinaPlantilla entity, CancellationToken ct = default)
        {
            _db.RutinasPlantilla.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task<RutinaPlantilla?> UpdateAsync(int id, RutinaPlantilla entity, CancellationToken ct = default)
        {
            var existing = await _db.RutinasPlantilla.FindAsync(new object[] { id }, ct);
            if (existing == null)
                return null;

            _db.Entry(existing).CurrentValues.SetValues(entity);
            await _db.SaveChangesAsync(ct);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.RutinasPlantilla.FindAsync(new object[] { id }, ct);
            if (entity == null)
                return false;

            _db.RutinasPlantilla.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
