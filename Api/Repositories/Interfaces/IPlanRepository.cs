using Api.Data.Models;

namespace Api.Repositories.Interfaces;

public interface IPlanRepository
{
    Task<(IReadOnlyList<Plan> items, int total)> GetPagedAsync(
        int page, int pageSize, string? q = null, int[]? dias = null, bool? activo = null,
        CancellationToken ct = default);

    Task<Plan?> GetAsync(uint id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string nombre, uint? excludeId = null, CancellationToken ct = default);
    Task<Plan> AddAsync(Plan entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(Plan entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(uint id, CancellationToken ct = default);
}
