using Api.Data.Models;

namespace Api.Repositories.Interfaces;

public interface IComprobanteRepository
{
    Task<comprobante?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<comprobante>> GetByOrdenAsync(int ordenId, CancellationToken ct = default);
    Task<comprobante> AddAsync(comprobante entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

