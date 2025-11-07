using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories
{
    public class GrupoMuscularRepository : IGrupoMuscularRepository
    {
        private readonly GymDbContext _db;

        public GrupoMuscularRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 Obtener todos los grupos musculares (paginado + búsqueda)
        public async Task<(IEnumerable<GrupoMuscular> items, int totalItems)> GetPagedAsync(
            int page, int pageSize, string? q, CancellationToken ct)
        {
            var query = _db.GruposMusculares.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(g =>
                    g.Nombre.ToLower().Contains(term) ||
                    (g.Descripcion != null && g.Descripcion.ToLower().Contains(term)));
            }

            var totalItems = await query.CountAsync(ct);

            var items = await query
                .OrderBy(g => g.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(ct);

            return (items, totalItems);
        }

        // 🔹 Obtener un grupo muscular por ID
        public async Task<GrupoMuscular?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _db.GruposMusculares
                .Include(g => g.Ejercicios)
                .Include(g => g.RutinasPlantilla)
                .FirstOrDefaultAsync(g => g.Id == id, ct);
        }

        // 🔹 Crear nuevo grupo muscular
        public async Task<GrupoMuscular> AddAsync(GrupoMuscular model, CancellationToken ct)
        {
            _db.GruposMusculares.Add(model);
            await _db.SaveChangesAsync(ct);
            return model;
        }

        // 🔹 Actualizar grupo muscular existente
        public async Task<GrupoMuscular?> UpdateAsync(int id, GrupoMuscular model, CancellationToken ct)
        {
            var existing = await _db.GruposMusculares.FindAsync(new object?[] { id }, ct);
            if (existing == null) return null;

            existing.Nombre = model.Nombre;
            existing.Descripcion = model.Descripcion;
            existing.ImagenUrl = model.ImagenUrl;

            await _db.SaveChangesAsync(ct);
            return existing;
        }

        // 🔹 Eliminar grupo muscular
        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var existing = await _db.GruposMusculares.FindAsync(new object?[] { id }, ct);
            if (existing == null) return false;

            _db.GruposMusculares.Remove(existing);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
