using Api.Data.Models;

namespace Api.Repositories.Interfaces;

public interface IPlanRepository
{
    Task<(IReadOnlyList<plan> items, int total)> GetPagedAsync(
        int page, int pageSize, string? q = null, int[]? dias = null, bool? activo = null,
        CancellationToken ct = default);

    Task<plan?> GetAsync(uint id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string nombre, uint? excludeId = null, CancellationToken ct = default);
    Task<plan> AddAsync(plan entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(plan entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(uint id, CancellationToken ct = default);
}
