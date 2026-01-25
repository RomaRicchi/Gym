using Gym.Domain.Entities;

namespace Gym.Domain.Interfaces;

public interface ISocioRepository : IRepository<Socio>
{
    Task<(IReadOnlyList<Socio> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null,
        bool? activo = null,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(string dni, string email, CancellationToken ct = default);
    Task<Socio?> GetWithSuscripcionesAsync(int id, CancellationToken ct = default);
}
