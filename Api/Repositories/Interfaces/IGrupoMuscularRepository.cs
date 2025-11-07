using Api.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    public interface IGrupoMuscularRepository
    {
        Task<(IEnumerable<GrupoMuscular> items, int totalItems)> GetPagedAsync(
            int page, int pageSize, string? q, CancellationToken ct);

        Task<GrupoMuscular?> GetByIdAsync(int id, CancellationToken ct);

        Task<GrupoMuscular> AddAsync(GrupoMuscular model, CancellationToken ct);

        Task<GrupoMuscular?> UpdateAsync(int id, GrupoMuscular model, CancellationToken ct);

        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
