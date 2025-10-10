using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class SocioRepository : ISocioRepository
{
    private readonly GymDbContext _db;
    public SocioRepository(GymDbContext db) => _db = db;

    public async Task<(IReadOnlyList<socio> items, int total)> GetPagedAsync(
        int page, int pageSize, string? q = null, bool? activo = null, CancellationToken ct = default)
    {
        var qry = _db.Socio.AsNoTracking().AsQueryable();

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

    public Task<socio?> GetByIdAsync(uint id, CancellationToken ct = default) =>
        _db.Socio.AsNoTracking().FirstOrDefaultAsync(s => s.id == id, ct);

    public Task<bool> ExistsAsync(string dni, string email, CancellationToken ct = default) =>
        _db.Socio.AnyAsync(s => s.dni == dni || s.email == email, ct);

    public async Task<socio> AddAsync(socio entity, CancellationToken ct = default)
    {
        _db.Socio.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(uint id, Action<socio> apply, CancellationToken ct = default)
    {
        var e = await _db.Socio.FirstOrDefaultAsync(s => s.id == id, ct);
        if (e is null) return false;

        apply(e);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SetActivoAsync(uint id, bool value, CancellationToken ct = default)
    {
        var e = await _db.Socio.FirstOrDefaultAsync(s => s.id == id, ct);
        if (e is null) return false;

        e.activo = value;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(uint id, CancellationToken ct = default)
    {
        var e = await _db.Socio.FirstOrDefaultAsync(s => s.id == id, ct);
        if (e is null) return false;

        _db.Socio.Remove(e);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
