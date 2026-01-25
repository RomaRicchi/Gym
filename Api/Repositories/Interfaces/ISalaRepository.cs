using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface ISalaRepository
    {
        Task<IReadOnlyList<Sala>> GetAllAsync(CancellationToken ct = default);
        Task<Sala?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Sala> AddAsync(Sala entity, CancellationToken ct = default);
        Task UpdateAsync(Sala entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
