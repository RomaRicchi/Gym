using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class GrupoMuscularRepository : IGrupoMuscularRepository
    {
        private readonly GymDbContext _context;

        public GrupoMuscularRepository(GymDbContext context)
        {
            _context = context;
        }

        // 🔹 Obtener todos los grupos musculares
        public async Task<IEnumerable<GrupoMuscular>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.GruposMusculares
                .AsNoTracking()
                .OrderBy(g => g.Nombre)
                .ToListAsync(ct);
        }

        // 🔹 Obtener un grupo muscular por ID
        public async Task<GrupoMuscular?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.GruposMusculares
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id, ct);
        }

        // 🔹 Crear un nuevo grupo muscular
        public async Task AddAsync(GrupoMuscular entity, CancellationToken ct = default)
        {
            _context.GruposMusculares.Add(entity);
            await _context.SaveChangesAsync(ct);
        }

        // 🔹 Actualizar un grupo muscular existente
        public async Task<GrupoMuscular?> UpdateAsync(int id, GrupoMuscular updatedEntity, CancellationToken ct = default)
        {
            var existing = await _context.GruposMusculares.FindAsync(new object[] { id }, ct);
            if (existing == null)
                return null;

            existing.Nombre = updatedEntity.Nombre;

            _context.GruposMusculares.Update(existing);
            await _context.SaveChangesAsync(ct);

            return existing;
        }

        // 🔹 Eliminar un grupo muscular
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.GruposMusculares.FindAsync(new object[] { id }, ct);
            if (entity == null)
                return false;

            _context.GruposMusculares.Remove(entity);
            await _context.SaveChangesAsync(ct);

            return true;
        }
    }
}
