using Api.Data.Models;

namespace Api.Repositories.Interfaces;

public interface IComprobanteRepository
{
    Task<Comprobante?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Comprobante>> GetByOrdenAsync(int ordenId, CancellationToken ct = default);
    Task<Comprobante> AddAsync(Comprobante entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
