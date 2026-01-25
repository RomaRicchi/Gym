using Gym.Domain.Entities;

namespace Gym.Domain.Interfaces;

public interface IPlanRepository : IRepository<Plan>
{
    Task<IReadOnlyList<Plan>> GetActivosAsync(CancellationToken ct = default);
}
