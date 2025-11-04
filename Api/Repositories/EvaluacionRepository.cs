using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class EvaluacionRepository : IEvaluacionRepository
    {
        private readonly GymDbContext _context;

        public EvaluacionRepository(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Evaluacion>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Evaluaciones
                .Include(e => e.RutinasAsignadas)
                .Include(e => e.Profesor)
                .ToListAsync(ct);
        }

        public async Task<Evaluacion?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Evaluaciones
                .Include(e => e.RutinasAsignadas)
                .Include(e => e.Profesor)
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<Evaluacion> AddAsync(Evaluacion model, CancellationToken ct = default)
        {
            _context.Evaluaciones.Add(model);
            await _context.SaveChangesAsync(ct);
            return model;
        }

        public async Task<Evaluacion?> UpdateAsync(int id, Evaluacion model, CancellationToken ct = default)
        {
            var existing = await _context.Evaluaciones.FindAsync(new object[] { id }, ct);
            if (existing == null) return null;

            existing.Fecha = model.Fecha;
            existing.Observaciones = model.Observaciones;
            existing.ProfesorId = model.ProfesorId;

            await _context.SaveChangesAsync(ct);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.Evaluaciones.FindAsync(new object[] { id }, ct);
            if (entity == null) return false;

            _context.Evaluaciones.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
