using Gym.Domain.Entities;

namespace Gym.Domain.Interfaces;

public interface IOrdenPagoRepository : IRepository<OrdenPago>
{
    Task<(IReadOnlyList<OrdenPago> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        int? socioId = null,
        int? estadoId = null,
        CancellationToken ct = default);

    Task<OrdenPago?> GetWithDetailsAsync(int id, CancellationToken ct = default);
}
