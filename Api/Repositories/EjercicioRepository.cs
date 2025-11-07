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
    public class EjercicioRepository : IEjercicioRepository
    {
        private readonly GymDbContext _db;

        public EjercicioRepository(GymDbContext db)
        {
            _db = db;
        }

        // ===============================
        // 🔹 GET paginado + filtro (nombre / grupo muscular)
        // ===============================
        public async Task<(IEnumerable<Ejercicio> items, int totalItems)> GetPagedAsync(
            int page, int pageSize, string? q, CancellationToken ct)
        {
            var query = _db.Ejercicios
                .Include(e => e.GrupoMuscular)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(e =>
                    e.Nombre.ToLower().Contains(term) ||
                    e.GrupoMuscular.Nombre.ToLower().Contains(term));
            }

            var totalItems = await query.CountAsync(ct);

            var items = await query
                .OrderBy(e => e.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(ct);

            return (items, totalItems);
        }

        // ===============================
        // 🔹 GET by ID
        // ===============================
        public async Task<Ejercicio?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _db.Ejercicios
                .Include(e => e.GrupoMuscular)
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        // ===============================
        // 🔹 POST
        // ===============================
        public async Task<Ejercicio> AddAsync(Ejercicio model, CancellationToken ct)
        {
            _db.Ejercicios.Add(model);
            await _db.SaveChangesAsync(ct);
            return model;
        }

        // ===============================
        // 🔹 PUT
        // ===============================
        public async Task<Ejercicio?> UpdateAsync(int id, Ejercicio model, CancellationToken ct)
        {
            var existing = await _db.Ejercicios.FindAsync(new object?[] { id }, ct);
            if (existing == null) return null;

            existing.Nombre = model.Nombre;
            existing.Tips = model.Tips;
            existing.MediaUrl = model.MediaUrl;
            existing.GrupoMuscularId = model.GrupoMuscularId;

            await _db.SaveChangesAsync(ct);
            return existing;
        }

        // ===============================
        // 🔹 DELETE
        // ===============================
        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var existing = await _db.Ejercicios.FindAsync(new object?[] { id }, ct);
            if (existing == null) return false;

            _db.Ejercicios.Remove(existing);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
