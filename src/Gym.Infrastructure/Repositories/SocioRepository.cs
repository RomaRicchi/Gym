using Gym.Domain.Entities;
using Gym.Domain.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositories;

public class SocioRepository : RepositoryBase<Socio>, ISocioRepository
{
    public SocioRepository(GymDbContext db) : base(db)
    {
    }

    public async Task<(IReadOnlyList<Socio> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null,
        bool? activo = null,
        CancellationToken ct = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(s => s.Suscripciones)
                .ThenInclude(su => su.Plan)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                s.Nombre.Contains(search) ||
                s.Dni.Contains(search) ||
                s.Email.Contains(search));
        }

        if (activo.HasValue)
        {
            query = query.Where(s => s.Activo == activo.Value);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(s => s.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<bool> ExistsAsync(string dni, string email, CancellationToken ct = default)
    {
        return await _dbSet.AnyAsync(s => s.Dni == dni || s.Email == email, ct);
    }

    public async Task<Socio?> GetWithSuscripcionesAsync(int id, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(s => s.Suscripciones)
                .ThenInclude(su => su.Plan)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }
}
