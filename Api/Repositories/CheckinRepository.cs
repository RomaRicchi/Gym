using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class CheckinRepository : ICheckinRepository
    {
        private readonly GymDbContext _db;

        public CheckinRepository(GymDbContext db)
        {
            _db = db;
        }

        public async Task<List<Checkin>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Checkins
                .AsNoTracking()
                .Include(c => c.Socio)
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync(ct);
        }

        public async Task<Checkin?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Checkins
                .AsNoTracking()
                .Include(c => c.Socio)
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<List<Checkin>> GetBySocioAsync(int socioId, CancellationToken ct = default)
        {
            return await _db.Checkins
                .Where(c => c.SocioId == socioId)
                .AsNoTracking()
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync(ct);
        }

        public async Task<List<Checkin>> GetByTurnoAsync(int turnoPlantillaId, CancellationToken ct = default)
        {
            return await _db.Checkins
                .Where(c => c.TurnoPlantillaId == turnoPlantillaId)
                .AsNoTracking()
                .Include(c => c.Socio)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync(ct);
        }

        public async Task<Checkin> AddAsync(Checkin entity, CancellationToken ct = default)
        {
            _db.Checkins.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task UpdateAsync(Checkin entity, CancellationToken ct = default)
        {
            _db.Checkins.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.Checkins.FindAsync(new object?[] { id }, ct);
            if (entity != null)
            {
                _db.Checkins.Remove(entity);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
