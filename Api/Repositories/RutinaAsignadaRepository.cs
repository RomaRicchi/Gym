using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories
{
    public class RutinaAsignadaRepository : IRutinaAsignadaRepository
    {
        private readonly GymDbContext _db;

        public RutinaAsignadaRepository(GymDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<RutinaAsignada>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.RutinasAsignadas
                .Include(r => r.Socio)
                .Include(r => r.Plan)
                .Include(r => r.RutinasPlantilla)
                .ToListAsync(ct);
        }

        public async Task<RutinaAsignada?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.RutinasAsignadas
                .Include(r => r.Socio)
                .Include(r => r.Plan)
                .Include(r => r.RutinasPlantilla)
                .FirstOrDefaultAsync(r => r.Id == id, ct);
        }

        public async Task<IEnumerable<RutinaAsignada>> GetBySocioIdAsync(int socioId, CancellationToken ct = default)
        {
            return await _db.RutinasAsignadas
                .Include(r => r.RutinasPlantilla)
                .Where(r => r.SocioId == socioId)
                .ToListAsync(ct);
        }


        public async Task<RutinaAsignada> AddAsync(RutinaAsignada entity, CancellationToken ct = default)
        {
            _db.RutinasAsignadas.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task<RutinaAsignada?> UpdateAsync(int id, RutinaAsignada entity, CancellationToken ct = default)
        {
            var existing = await _db.RutinasAsignadas.FindAsync(new object[] { id }, ct);
            if (existing == null)
                return null;

            _db.Entry(existing).CurrentValues.SetValues(entity);
            await _db.SaveChangesAsync(ct);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.RutinasAsignadas.FindAsync(new object[] { id }, ct);
            if (entity == null)
                return false;

            _db.RutinasAsignadas.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
