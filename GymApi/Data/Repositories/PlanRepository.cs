using GymApi.Data;
using GymApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly GymDbContext _db;
    public PlanRepository(GymDbContext db) => _db = db;

    public async Task<(IReadOnlyList<plan> items, int total)> GetPagedAsync(
        int page, int pageSize, string? q = null, int[]? dias = null, bool? activo = null, CancellationToken ct = default)
    {
        var qry = _db.plan.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            qry = qry.Where(p => p.nombre.Contains(q));
        if (dias is { Length: > 0 })
            qry = qry.Where(p => dias.Contains(p.dias_por_semana));
        if (activo.HasValue)
            qry = qry.Where(p => p.activo == activo);

        var total = await qry.CountAsync(ct);
        var items = await qry.OrderBy(p => p.id)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync(ct);
        return (items, total);
    }

    public Task<plan?> GetAsync(uint id, CancellationToken ct = default)
        => _db.plan.AsNoTracking().FirstOrDefaultAsync(p => p.id == id, ct);

    public Task<bool> ExistsByNameAsync(string nombre, uint? excludeId = null, CancellationToken ct = default)
        => _db.plan.AnyAsync(p => p.nombre == nombre && (!excludeId.HasValue || p.id != excludeId.Value), ct);

    public async Task<plan> AddAsync(plan entity, CancellationToken ct = default)
    {
        _db.plan.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(plan entity, CancellationToken ct = default)
    {
        // Asumiendo entity.id válido
        _db.plan.Update(entity);
        return await _db.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(uint id, CancellationToken ct = default)
    {
        var entity = await _db.plan.FindAsync(new object?[] { id }, ct);
        if (entity is null) return false;
        _db.plan.Remove(entity);
        return await _db.SaveChangesAsync(ct) > 0;
    }
}
