using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly GymDbContext _db;
    public PlanRepository(GymDbContext db) => _db = db;

    public async Task<(IReadOnlyList<Plan> items, int total)> GetPagedAsync(
        int page, int pageSize, string? q = null, int[]? dias = null, bool? activo = null, CancellationToken ct = default)
    {
        var qry = _db.Planes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            qry = qry.Where(p => p.Nombre.Contains(q));
        if (dias is { Length: > 0 })
            qry = qry.Where(p => dias.Contains(p.DiasPorSemana));
        if (activo.HasValue)
            qry = qry.Where(p => p.Activo == activo);

        var total = await qry.CountAsync(ct);
        var items = await qry.OrderBy(p => p.Id)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync(ct);
        return (items, total);
    }

    public Task<Plan?> GetAsync(uint id, CancellationToken ct = default)
        => _db.Planes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<bool> ExistsByNameAsync(string nombre, uint? excludeId = null, CancellationToken ct = default)
        => _db.Planes.AnyAsync(p => p.Nombre == nombre && (!excludeId.HasValue || p.Id != excludeId.Value), ct);

    public async Task<Plan> AddAsync(Plan entity, CancellationToken ct = default)
    {
        _db.Planes.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(Plan entity, CancellationToken ct = default)
    {
        _db.Planes.Update(entity);
        return await _db.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(uint id, CancellationToken ct = default)
    {
        var entity = await _db.Planes.FindAsync(new object?[] { id }, ct);
        if (entity is null) return false;
        _db.Planes.Remove(entity);
        return await _db.SaveChangesAsync(ct) > 0;
    }
}
