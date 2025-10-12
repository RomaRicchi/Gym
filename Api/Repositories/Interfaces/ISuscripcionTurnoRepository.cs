using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface ISuscripcionTurnoRepository
    {
        Task<IReadOnlyList<SuscripcionTurno>> GetAllAsync(CancellationToken ct = default);
        Task<SuscripcionTurno?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<SuscripcionTurno>> GetBySuscripcionAsync(uint suscripcionId, CancellationToken ct = default);
        Task<IReadOnlyList<SuscripcionTurno>> GetBySocioAsync(uint socioId, CancellationToken ct = default);
        Task<SuscripcionTurno> AddAsync(SuscripcionTurno entity, CancellationToken ct = default);
        Task UpdateAsync(SuscripcionTurno entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
