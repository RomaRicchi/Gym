using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories
{
    public class EjercicioRepository : IEjercicioRepository
    {
        private readonly GymDbContext _db;

        public EjercicioRepository(GymDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Ejercicio>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Ejercicios
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<Ejercicio?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Ejercicios
                .Include(e => e.RutinaPlantillaEjercicios)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<Ejercicio> AddAsync(Ejercicio entity, CancellationToken ct = default)
        {
            _db.Ejercicios.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task<Ejercicio?> UpdateAsync(int id, Ejercicio entity, CancellationToken ct = default)
        {
            var existing = await _db.Ejercicios.FindAsync(new object[] { id }, ct);
            if (existing == null) return null;

            existing.Nombre = entity.Nombre;
            existing.Grupo = entity.Grupo;
            existing.Tips = entity.Tips;
            existing.MediaUrl = entity.MediaUrl;

            await _db.SaveChangesAsync(ct);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var existing = await _db.Ejercicios.FindAsync(new object[] { id }, ct);
            if (existing == null) return false;

            _db.Ejercicios.Remove(existing);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
