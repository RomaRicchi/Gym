using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface ISuscripcionRepository
    {
        Task<IReadOnlyList<Suscripcion>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Suscripcion>> GetActivasAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Suscripcion>> GetBySocioAsync(int socioId, CancellationToken ct = default);
        Task<Suscripcion?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Suscripcion> AddAsync(Suscripcion entity, CancellationToken ct = default);
        Task UpdateAsync(Suscripcion entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<Suscripcion?> GetActivaByPlanAsync(int socioId, int planId, CancellationToken ct = default);

        IQueryable<Suscripcion> Query();
    }
}
