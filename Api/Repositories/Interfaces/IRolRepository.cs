using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IRolRepository
    {
        Task<IReadOnlyList<Rol>> GetAllAsync(CancellationToken ct = default);
        Task<Rol?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Rol> AddAsync(Rol rol, CancellationToken ct = default);
        Task<Rol?> UpdateAsync(int id, Rol rol, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
