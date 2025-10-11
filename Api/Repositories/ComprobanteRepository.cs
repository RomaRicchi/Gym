using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class ComprobanteRepository : IComprobanteRepository
{
    private readonly GymDbContext _db;

    public ComprobanteRepository(GymDbContext db)
    {
        _db = db;
    }

    public async Task<Comprobante?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Comprobantes.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<IReadOnlyList<Comprobante>> GetByOrdenAsync(int ordenId, CancellationToken ct = default)
    {
        return await _db.Comprobantes
            .Where(c => c.OrdenId == ordenId)
            .OrderByDescending(c => c.SubidoEn)
            .ToListAsync(ct);
    }

    public async Task<Comprobante> AddAsync(Comprobante entity, CancellationToken ct = default)
    {
        _db.Comprobantes.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var comp = await _db.Comprobantes.FindAsync(new object?[] { id }, ct);
        if (comp != null)
        {
            _db.Comprobantes.Remove(comp);
            await _db.SaveChangesAsync(ct);
        }
    }
}
