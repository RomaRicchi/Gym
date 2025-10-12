using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class TurnoPlantillaRepository : ITurnoPlantillaRepository
    {
        private readonly GymDbContext _db;

        public TurnoPlantillaRepository(GymDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<TurnoPlantilla>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .AsNoTracking()
                .OrderBy(t => t.DiaSemanaId)
                .ThenBy(t => t.HoraInicio)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<TurnoPlantilla>> GetActivosAsync(CancellationToken ct = default)
        {
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .AsNoTracking()
                .Where(t => t.Activo)
                .OrderBy(t => t.DiaSemanaId)
                .ThenBy(t => t.HoraInicio)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<TurnoPlantilla>> GetByDiaSemanaAsync(sbyte diaSemana, CancellationToken ct = default)
        {
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .AsNoTracking()
                .Where(t => t.DiaSemanaId == diaSemana && t.Activo)
                .OrderBy(t => t.HoraInicio)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<TurnoPlantilla>> GetByPersonalAsync(uint personalId, CancellationToken ct = default)
        {
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.DiaSemana)
                .AsNoTracking()
                .Where(t => t.PersonalId == personalId && t.Activo)
                .OrderBy(t => t.DiaSemanaId)
                .ThenBy(t => t.HoraInicio)
                .ToListAsync(ct);
        }

        public async Task<TurnoPlantilla?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<TurnoPlantilla> AddAsync(TurnoPlantilla turno, CancellationToken ct = default)
        {
            _db.TurnosPlantilla.Add(turno);
            await _db.SaveChangesAsync(ct);
            return turno;
        }

        public async Task<bool> UpdateAsync(TurnoPlantilla updated, CancellationToken ct = default)
        {
            var turno = await _db.TurnosPlantilla.FindAsync(new object[] { updated.Id }, ct);
            if (turno == null) return false;

            turno.SalaId = updated.SalaId;
            turno.PersonalId = updated.PersonalId;
            turno.DiaSemanaId = updated.DiaSemanaId;
            turno.HoraInicio = updated.HoraInicio;
            turno.DuracionMin = updated.DuracionMin;
            turno.Cupo = updated.Cupo;
            turno.Activo = updated.Activo;

            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var turno = await _db.TurnosPlantilla.FindAsync(new object[] { id }, ct);
            if (turno == null) return false;

            _db.TurnosPlantilla.Remove(turno);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> ExisteSolapamientoAsync(uint salaId, byte diaSemana, TimeSpan horaInicio, int duracionMin, CancellationToken ct = default)
        {
            var horaFin = horaInicio + TimeSpan.FromMinutes(duracionMin);

            return await _db.TurnosPlantilla
                .AsNoTracking()
                .AnyAsync(t =>
                    t.SalaId == salaId &&
                    t.DiaSemanaId == diaSemana &&
                    t.Activo &&
                    (
                        (t.HoraInicio <= horaInicio && (t.HoraInicio + TimeSpan.FromMinutes(t.DuracionMin)) > horaInicio) ||
                        (t.HoraInicio < horaFin && (t.HoraInicio + TimeSpan.FromMinutes(t.DuracionMin)) >= horaFin)
                    ),
                    ct
                );
        }
    }
}
