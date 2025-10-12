using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class SuscripcionRepository : ISuscripcionRepository
    {
        private readonly GymDbContext _db;

        public SuscripcionRepository(GymDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Suscripcion>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .OrderByDescending(s => s.CreadoEn)
                .ToListAsync(ct);
        }

        // 🔹 Solo las activas (estado = true)
        public async Task<IReadOnlyList<Suscripcion>> GetActivasAsync(CancellationToken ct = default)
        {
            return await _db.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .Where(s => s.Estado)
                .OrderByDescending(s => s.Inicio)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Suscripcion>> GetBySocioAsync(uint socioId, CancellationToken ct = default)
        {
            return await _db.Suscripciones
                .Include(s => s.Plan)
                .Where(s => s.SocioId == socioId)
                .OrderByDescending(s => s.Inicio)
                .ToListAsync(ct);
        }

        public async Task<Suscripcion?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .Include(s => s.SuscripcionesTurno)
                    .ThenInclude(st => st.TurnoPlantilla)
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        // 🔹 Buscar suscripción activa por socio y plan
        public async Task<Suscripcion?> GetActivaByPlanAsync(uint socioId, uint planId, CancellationToken ct = default)
        {
            return await _db.Suscripciones
                .Where(s => s.SocioId == socioId && s.PlanId == planId && s.Estado)
                .OrderByDescending(s => s.Inicio)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<Suscripcion> AddAsync(Suscripcion entity, CancellationToken ct = default)
        {
            _db.Suscripciones.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task UpdateAsync(Suscripcion entity, CancellationToken ct = default)
        {
            _db.Suscripciones.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var sus = await _db.Suscripciones.FindAsync(new object?[] { id }, ct);
            if (sus != null)
            {
                _db.Suscripciones.Remove(sus);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
