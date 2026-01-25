using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IGrupoMuscularRepository
    {
        Task<IEnumerable<GrupoMuscular>> GetAllAsync(CancellationToken ct = default);
        Task<GrupoMuscular?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(GrupoMuscular entity, CancellationToken ct = default);
        Task<GrupoMuscular?> UpdateAsync(int id, GrupoMuscular updatedEntity, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
