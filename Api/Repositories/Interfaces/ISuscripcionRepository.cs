using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface ISuscripcionRepository
    {
        Task<IReadOnlyList<Suscripcion>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Suscripcion>> GetActivasAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Suscripcion>> GetBySocioAsync(uint socioId, CancellationToken ct = default);
        Task<Suscripcion?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Suscripcion> AddAsync(Suscripcion entity, CancellationToken ct = default);
        Task UpdateAsync(Suscripcion entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<Suscripcion?> GetActivaByPlanAsync(uint socioId, uint planId, CancellationToken ct = default);
    }
}
