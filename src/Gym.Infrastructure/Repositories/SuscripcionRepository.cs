using Gym.Domain.Entities;
using Gym.Domain.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositories;

public class SuscripcionRepository : RepositoryBase<Suscripcion>, ISuscripcionRepository
{
    public SuscripcionRepository(GymDbContext db) : base(db)
    {
    }

    public async Task<(IReadOnlyList<Suscripcion> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        int? socioId = null,
        bool? activo = null,
        CancellationToken ct = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(s => s.Socio)
            .Include(s => s.Plan)
            .AsQueryable();

        if (socioId.HasValue)
        {
            query = query.Where(s => s.SocioId == socioId.Value);
        }

        if (activo.HasValue)
        {
            query = query.Where(s => s.Estado == activo.Value);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(s => s.Inicio)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Suscripcion?> GetActivaBySocioAsync(int socioId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(s => s.Plan)
            .Where(s => s.SocioId == socioId && s.Estado && s.Inicio <= now && s.Fin >= now)
            .OrderByDescending(s => s.Inicio)
            .FirstOrDefaultAsync(ct);
    }
}
