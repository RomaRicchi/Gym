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

        // Obtener todos
        public async Task<List<Checkin>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Checkins
                .AsNoTracking()
                .Include(c => c.Socio)
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .Include(c => c.Profesor) // 👈 nuevo: carga el profesor del check-in
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync(ct);
        }

        //  Obtener por ID
        public async Task<Checkin?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Checkins
                .AsNoTracking()
                .Include(c => c.Socio)
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .Include(c => c.Profesor) // 👈 profesor del checkin
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        //  Por socio
        public async Task<List<Checkin>> GetBySocioAsync(int socioId, CancellationToken ct = default)
        {
            return await _db.Checkins
                .Where(c => c.SocioId == socioId)
                .AsNoTracking()
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(c => c.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .Include(c => c.Profesor)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync(ct);
        }

        //  Por turno
        public async Task<List<Checkin>> GetByTurnoAsync(int turnoPlantillaId, CancellationToken ct = default)
        {
            return await _db.Checkins
                .Where(c => c.TurnoPlantillaId == turnoPlantillaId)
                .AsNoTracking()
                .Include(c => c.Socio)
                .Include(c => c.Profesor)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync(ct);
        }

        //  Crear nuevo Check-in
        public async Task<Checkin> AddAsync(Checkin entity, CancellationToken ct = default)
        {
            entity.FechaHora = DateTime.UtcNow; // 🔧 asegura fecha automática
            _db.Checkins.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        //  Actualizar (p. ej. agregar observaciones)
        public async Task UpdateAsync(Checkin entity, CancellationToken ct = default)
        {
            var existing = await _db.Checkins.FindAsync(new object?[] { entity.Id }, ct);
            if (existing == null) return;

            existing.ProfesorId = entity.ProfesorId;
            existing.Observaciones = entity.Observaciones;
            existing.FechaHora = entity.FechaHora;

            _db.Checkins.Update(existing);
            await _db.SaveChangesAsync(ct);
        }

        //  Eliminar
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
