using Gym.Domain.Entities;
using Gym.Domain.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositories;

public class PlanRepository : RepositoryBase<Plan>, IPlanRepository
{
    public PlanRepository(GymDbContext db) : base(db)
    {
    }

    public async Task<IReadOnlyList<Plan>> GetActivosAsync(CancellationToken ct = default)
    {
        return await _dbSet
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .ToListAsync(ct);
    }
}
