using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class SuscripcionTurnoRepository : ISuscripcionTurnoRepository
    {
        private readonly GymDbContext _db;

        public SuscripcionTurnoRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 Obtener todos los turnos asignados
        public async Task<IReadOnlyList<SuscripcionTurno>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.SuscripcionTurnos
                .Include(st => st.Suscripcion)
                    .ThenInclude(s => s.Plan)
                .Include(st => st.Suscripcion)
                    .ThenInclude(s => s.Socio)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.DiaSemana)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        // 🔹 Obtener turnos por suscripción
        public async Task<IReadOnlyList<SuscripcionTurno>> GetBySuscripcionAsync(int suscripcionId, CancellationToken ct = default)
        {
            return await _db.SuscripcionTurnos
                .Where(st => st.SuscripcionId == suscripcionId)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.DiaSemana)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        // 🔹 Obtener turnos por socio
        public async Task<IReadOnlyList<SuscripcionTurno>> GetBySocioAsync(int socioId, CancellationToken ct = default)
        {
            return await _db.SuscripcionTurnos
                .Where(st => st.Suscripcion.SocioId == socioId)
                .Include(st => st.Suscripcion)
                    .ThenInclude(s => s.Plan)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.DiaSemana)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        // 🔹 Obtener un turno específico por ID
        public async Task<SuscripcionTurno?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.SuscripcionTurnos
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.DiaSemana)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .AsNoTracking()
                .FirstOrDefaultAsync(st => st.Id == id, ct);
        }

        // 🔹 Crear nuevo registro
        public async Task<SuscripcionTurno> AddAsync(SuscripcionTurno entity, CancellationToken ct = default)
        {
            _db.SuscripcionTurnos.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // 🔹 Actualizar un registro existente
        public async Task UpdateAsync(SuscripcionTurno entity, CancellationToken ct = default)
        {
            _db.SuscripcionTurnos.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        // 🔹 Eliminar un registro por ID
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.SuscripcionTurnos.FindAsync(id);
            if (entity != null)
            {
                _db.SuscripcionTurnos.Remove(entity);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
