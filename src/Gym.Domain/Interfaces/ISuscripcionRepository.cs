using Gym.Domain.Entities;

namespace Gym.Domain.Interfaces;

public interface ISuscripcionRepository : IRepository<Suscripcion>
{
    Task<(IReadOnlyList<Suscripcion> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        int? socioId = null,
        bool? activo = null,
        CancellationToken ct = default);

    Task<Suscripcion?> GetActivaBySocioAsync(int socioId, CancellationToken ct = default);
}
