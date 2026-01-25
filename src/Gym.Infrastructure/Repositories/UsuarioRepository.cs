using Gym.Domain.Entities;
using Gym.Domain.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositories;

public class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(GymDbContext db) : base(db)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(u => u.Rol)
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _dbSet.AnyAsync(u => u.Email == email, ct);
    }

    public async Task<Usuario?> GetWithDetailsAsync(int id, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(u => u.Rol)
            .Include(u => u.Avatar)
            .Include(u => u.Personal)
            .Include(u => u.Socio)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}
