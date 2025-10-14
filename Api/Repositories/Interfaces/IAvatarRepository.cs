using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IAvatarRepository
    {
        Task<IReadOnlyList<Avatar>> GetAllAsync(CancellationToken ct = default);
        Task<Avatar?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Avatar> AddAsync(Avatar avatar, CancellationToken ct = default);
        Task<bool> UpdateAsync(Avatar avatar, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default); // baja lógica o física
    }
}
