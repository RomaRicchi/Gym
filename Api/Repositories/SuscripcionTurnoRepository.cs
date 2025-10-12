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

        // 🔹 Listar todas las relaciones suscripción-turno
        public async Task<IReadOnlyList<SuscripcionTurno>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.SuscripcionesTurno
                .Include(st => st.Suscripcion)
                .ThenInclude(s => s.Socio)
                .Include(st => st.TurnoPlantilla)
                .ThenInclude(tp => tp.Sala)
                .Include(st => st.TurnoPlantilla.Personal)
                .OrderBy(st => st.TurnoPlantilla.DiaSemana)
                .ThenBy(st => st.TurnoPlantilla.HoraInicio)
                .ToListAsync(ct);
        }

        // 🔹 Obtener una asignación por ID
        public async Task<SuscripcionTurno?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.SuscripcionesTurno
                .Include(st => st.Suscripcion)
                .ThenInclude(s => s.Socio)
                .Include(st => st.TurnoPlantilla)
                .ThenInclude(tp => tp.Sala)
                .FirstOrDefaultAsync(st => st.Id == id, ct);
        }

        // 🔹 Obtener todos los turnos de una suscripción
        public async Task<IReadOnlyList<SuscripcionTurno>> GetBySuscripcionAsync(uint suscripcionId, CancellationToken ct = default)
        {
            return await _db.SuscripcionesTurno
                .Include(st => st.TurnoPlantilla)
                .ThenInclude(tp => tp.Sala)
                .Where(st => st.SuscripcionId == suscripcionId)
                .OrderBy(st => st.TurnoPlantilla.DiaSemana)
                .ThenBy(st => st.TurnoPlantilla.HoraInicio)
                .ToListAsync(ct);
        }

        // 🔹 Obtener todos los turnos asignados a un socio (vía suscripción)
        public async Task<IReadOnlyList<SuscripcionTurno>> GetBySocioAsync(uint socioId, CancellationToken ct = default)
        {
            return await _db.SuscripcionesTurno
                .Include(st => st.Suscripcion)
                .ThenInclude(s => s.Socio)
                .Include(st => st.TurnoPlantilla)
                .ThenInclude(tp => tp.Sala)
                .Where(st => st.Suscripcion.SocioId == socioId)
                .OrderBy(st => st.TurnoPlantilla.DiaSemana)
                .ThenBy(st => st.TurnoPlantilla.HoraInicio)
                .ToListAsync(ct);
        }

        // 🔹 Agregar una nueva relación suscripción-turno
        public async Task<SuscripcionTurno> AddAsync(SuscripcionTurno entity, CancellationToken ct = default)
        {
            _db.SuscripcionesTurno.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // 🔹 Actualizar una relación existente
        public async Task UpdateAsync(SuscripcionTurno entity, CancellationToken ct = default)
        {
            _db.SuscripcionesTurno.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        // 🔹 Eliminar una relación
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.SuscripcionesTurno.FindAsync(new object?[] { id }, ct);
            if (entity != null)
            {
                _db.SuscripcionesTurno.Remove(entity);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
