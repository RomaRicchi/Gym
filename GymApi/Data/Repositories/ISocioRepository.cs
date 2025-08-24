using GymApi.Data.Models;

namespace GymApi.Repositories;

public interface ISocioRepository
{
    Task<(IReadOnlyList<socio> items, int total)> GetPagedAsync(
        int page, int pageSize, string? q = null, bool? activo = null, CancellationToken ct = default);

    Task<bool> ExistsAsync(string dni, string email, CancellationToken ct = default);
    Task<socio> AddAsync(socio entity, CancellationToken ct = default);
}
