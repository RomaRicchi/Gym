using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class EvaluacionRepository : IEvaluacionRepository
    {
        private readonly GymDbContext _db;

        public EvaluacionRepository(GymDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Evaluacion>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Evaluaciones
                .Include(e => e.RutinaAsignada)
                .Include(e => e.Profesor)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<Evaluacion?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Evaluaciones
                .Include(e => e.RutinaAsignada)
                .Include(e => e.Profesor)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<IReadOnlyList<Evaluacion>> GetByProfesorIdAsync(int profesorId, CancellationToken ct = default)
        {
            return await _db.Evaluaciones
                .Where(e => e.ProfesorId == profesorId)
                .Include(e => e.RutinaAsignada)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Evaluacion>> GetByRutinaAsignadaIdAsync(int rutinaAsignadaId, CancellationToken ct = default)
        {
            return await _db.Evaluaciones
                .Where(e => e.RutinaAsignadaId == rutinaAsignadaId)
                .Include(e => e.Profesor)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<Evaluacion> AddAsync(Evaluacion model, CancellationToken ct = default)
        {
            _db.Evaluaciones.Add(model);
            await _db.SaveChangesAsync(ct);
            return model;
        }

        public async Task<Evaluacion?> UpdateAsync(int id, Evaluacion model, CancellationToken ct = default)
        {
            var existing = await _db.Evaluaciones.FindAsync(new object[] { id }, ct);
            if (existing == null) return null;

            existing.ProfesorId = model.ProfesorId;
            existing.RutinaAsignadaId = model.RutinaAsignadaId;
            existing.Fecha = model.Fecha;
            existing.Observaciones = model.Observaciones;

            await _db.SaveChangesAsync(ct);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var existing = await _db.Evaluaciones.FindAsync(new object[] { id }, ct);
            if (existing == null) return false;

            _db.Evaluaciones.Remove(existing);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
