using Api.Data.Models;
using Microsoft.EntityFrameworkCore;
using Api.Repositories.Interfaces;
using Api.Data;

namespace Api.Repositories;

public class ComprobanteRepository : IComprobanteRepository
{
    private readonly GymDbContext _db;

    public ComprobanteRepository(GymDbContext db)
    {
        _db = db;
    }

    public async Task<comprobante?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Comprobante.FirstOrDefaultAsync(c => c.id == id, ct);
    }

    public async Task<IReadOnlyList<comprobante>> GetByOrdenAsync(int ordenId, CancellationToken ct = default)
    {
        return await _db.Comprobante
            .Where(c => c.orden_id == ordenId)
            .OrderByDescending(c => c.subido_en)
            .ToListAsync(ct);
    }

    public async Task<comprobante> AddAsync(comprobante entity, CancellationToken ct = default)
    {
        _db.Comprobante.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var comp = await _db.Comprobante.FindAsync(new object?[] { id }, ct);
        if (comp != null)
        {
            _db.Comprobante.Remove(comp);
            await _db.SaveChangesAsync(ct);
        }
    }
}

