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

        // 🔹 Obtener todos los turnos
        public async Task<IReadOnlyList<TurnoPlantilla>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .OrderBy(t => t.DiaSemana)
                .ThenBy(t => t.HoraInicio)
                .ToListAsync(ct);
        }

        // 🔹 Solo los turnos activos
        public async Task<IReadOnlyList<TurnoPlantilla>> GetActivosAsync(CancellationToken ct = default)
        {
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Where(t => t.Activo)
                .OrderBy(t => t.DiaSemana)
                .ThenBy(t => t.HoraInicio)
                .ToListAsync(ct);
        }

        // 🔹 Turnos por día de la semana
        public async Task<IReadOnlyList<TurnoPlantilla>> GetByDiaSemanaAsync(sbyte dia, CancellationToken ct = default)
        {
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Where(t => t.DiaSemana == dia)
                .OrderBy(t => t.HoraInicio)
                .ToListAsync(ct);
        }

        // 🔹 Turnos de un profesor/personal específico
        public async Task<IReadOnlyList<TurnoPlantilla>> GetByPersonalAsync(uint personalId, CancellationToken ct = default)
        {
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Where(t => t.PersonalId == personalId)
                .OrderBy(t => t.DiaSemana)
                .ThenBy(t => t.HoraInicio)
                .ToListAsync(ct);
        }

        // 🔹 Obtener por ID
        public async Task<TurnoPlantilla?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        // 🔹 Agregar un nuevo turno
        public async Task<TurnoPlantilla> AddAsync(TurnoPlantilla entity, CancellationToken ct = default)
        {
            _db.TurnosPlantilla.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // 🔹 Actualizar un turno existente
        public async Task UpdateAsync(TurnoPlantilla entity, CancellationToken ct = default)
        {
            _db.TurnosPlantilla.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        // 🔹 Eliminar un turno
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var turno = await _db.TurnosPlantilla.FindAsync(new object?[] { id }, ct);
            if (turno != null)
            {
                _db.TurnosPlantilla.Remove(turno);
                await _db.SaveChangesAsync(ct);
            }
        }


        public async Task<bool> ExisteConflictoAsync(uint salaId, sbyte dia, TimeOnly horaInicio, int duracionMin, CancellationToken ct = default)
        {
            var horaFin = horaInicio.AddMinutes(duracionMin);
            return await _db.TurnosPlantilla.AnyAsync(t =>
                t.SalaId == salaId &&
                t.DiaSemana == dia &&
                t.Activo == true &&
                (
                    (horaInicio >= t.HoraInicio && horaInicio < t.HoraInicio.AddMinutes(t.DuracionMin)) ||
                    (horaFin > t.HoraInicio && horaFin <= t.HoraInicio.AddMinutes(t.DuracionMin))
                ),
                ct);
        }
    }
}
