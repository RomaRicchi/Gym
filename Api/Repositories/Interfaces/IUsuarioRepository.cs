using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IReadOnlyList<object>> GetAllAsync(CancellationToken ct = default);
        Task<object?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Usuario> CreateAsync(Usuario u, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, Usuario updated, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
