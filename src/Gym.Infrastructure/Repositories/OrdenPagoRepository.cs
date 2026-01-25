using Gym.Domain.Entities;
using Gym.Domain.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositories;

public class OrdenPagoRepository : RepositoryBase<OrdenPago>, IOrdenPagoRepository
{
    public OrdenPagoRepository(GymDbContext db) : base(db)
    {
    }

    public async Task<(IReadOnlyList<OrdenPago> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        int? socioId = null,
        int? estadoId = null,
        CancellationToken ct = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(o => o.Socio)
            .Include(o => o.Plan)
            .Include(o => o.Estado)
            .Include(o => o.Comprobante)
            .AsQueryable();

        if (socioId.HasValue)
        {
            query = query.Where(o => o.SocioId == socioId.Value);
        }

        if (estadoId.HasValue)
        {
            query = query.Where(o => o.EstadoId == estadoId.Value);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<OrdenPago?> GetWithDetailsAsync(int id, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(o => o.Socio)
            .Include(o => o.Plan)
            .Include(o => o.Estado)
            .Include(o => o.Comprobante)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }
}
