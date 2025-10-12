using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IPersonalRepository
    {
        Task<IReadOnlyList<Personal>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Personal>> GetActivosAsync(CancellationToken ct = default);
        Task<Personal?> GetByIdAsync(uint id, CancellationToken ct = default);
        Task<Personal> AddAsync(Personal entity, CancellationToken ct = default);
        Task UpdateAsync(Personal entity, CancellationToken ct = default);
        Task DeleteAsync(uint id, CancellationToken ct = default);
    }
}

