using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken ct = default);
        Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<Usuario> AddAsync(Usuario usuario, CancellationToken ct = default);
        Task<bool> UpdateAsync(Usuario usuario, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default); // baja lógica

        Task<bool> UpdatePerfilAsync(int id, string? nombre, string? email, string? telefono, int? idAvatar, CancellationToken ct = default);

    }
}
