using Gym.Domain.Entities;

namespace Gym.Domain.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<Usuario?> GetWithDetailsAsync(int id, CancellationToken ct = default);
}
