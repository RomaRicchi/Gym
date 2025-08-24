using GymApi.Data;
using GymApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Repositories;

public class SocioRepository : ISocioRepository
{
    private readonly GymDbContext _db;
    public SocioRepository(GymDbContext db) => _db = db;

    public async Task<(IReadOnlyList<socio> items, int total)> GetPagedAsync(
        int page, int pageSize, string? q = null, bool? activo = null, CancellationToken ct = default)
    {
        var qry = _db.socio.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            qry = qry.Where(s => s.nombre.Contains(q) || s.dni.Contains(q) || s.email.Contains(q));
        if (activo.HasValue)
            qry = qry.Where(s => s.activo == activo);

        var total = await qry.CountAsync(ct);
        var items = await qry.OrderBy(s => s.id)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync(ct);
        return (items, total);
    }

    public Task<bool> ExistsAsync(string dni, string email, CancellationToken ct = default)
        => _db.socio.AnyAsync(s => s.dni == dni || s.email == email, ct);

    public async Task<socio> AddAsync(socio entity, CancellationToken ct = default)
    {
        _db.socio.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }
}
