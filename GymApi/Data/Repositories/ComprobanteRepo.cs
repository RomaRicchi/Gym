using GymApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Data.Repositories;

public class ComprobanteRepository : IComprobanteRepository
{
    private readonly GymDbContext _db;

    public ComprobanteRepository(GymDbContext db)
    {
        _db = db;
    }

    public async Task<comprobante?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.comprobante.FirstOrDefaultAsync(c => c.id == id, ct);
    }

    public async Task<IReadOnlyList<comprobante>> GetByOrdenAsync(int ordenId, CancellationToken ct = default)
    {
        return await _db.comprobante
            .Where(c => c.orden_id == ordenId)
            .OrderByDescending(c => c.subido_en)
            .ToListAsync(ct);
    }

    public async Task<comprobante> AddAsync(comprobante entity, CancellationToken ct = default)
    {
        _db.comprobante.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var comp = await _db.comprobante.FindAsync(new object?[] { id }, ct);
        if (comp != null)
        {
            _db.comprobante.Remove(comp);
            await _db.SaveChangesAsync(ct);
        }
    }
}

