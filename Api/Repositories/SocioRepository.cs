using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class SocioRepository : ISocioRepository
    {
        private readonly GymDbContext _db;
        public SocioRepository(GymDbContext db) => _db = db;

        public async Task<(IReadOnlyList<Socio> items, int total)> GetPagedAsync(
            int page, int pageSize, string? q = null, bool? activo = null, CancellationToken ct = default)
        {
            var qry = _db.Socios.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                qry = qry.Where(s => s.Nombre.Contains(q) || s.Dni.Contains(q) || s.Email.Contains(q));

            if (activo.HasValue)
                qry = qry.Where(s => s.Activo == activo.Value); 

            var total = await qry.CountAsync(ct);
            var items = await qry.OrderBy(s => s.Id)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync(ct);

            return (items, total);
        }


        public Task<Socio?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _db.Socios.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);

        public Task<bool> ExistsAsync(string dni, string email, CancellationToken ct = default) =>
            _db.Socios.AnyAsync(s => s.Dni == dni || s.Email == email, ct);

        public async Task<Socio> AddAsync(Socio entity, CancellationToken ct = default)
        {
            _db.Socios.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task<bool> UpdateAsync(int id, Action<Socio> apply, CancellationToken ct = default)
        {
            var e = await _db.Socios.FirstOrDefaultAsync(s => s.Id == id, ct);
            if (e is null) return false;

            apply(e);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> SetActivoAsync(int id, bool value, CancellationToken ct = default)
        {
            var e = await _db.Socios.FirstOrDefaultAsync(s => s.Id == id, ct);
            if (e is null) return false;

            e.Activo = value;
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var e = await _db.Socios.FirstOrDefaultAsync(s => s.Id == id, ct);
            if (e is null) return false;

            _db.Socios.Remove(e);
            await _db.SaveChangesAsync(ct);
            return true;
        }
        public IQueryable<Socio> Query()
        {
            return _db.Socios
                .Include(s => s.Suscripciones)
                .ThenInclude(su => su.Plan)
                .AsQueryable();
        }

    }
}
