using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IOrdenTurnoRepository
    {
        Task<IReadOnlyList<OrdenTurno>> GetAllAsync(CancellationToken ct = default);
        Task<OrdenTurno?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<OrdenTurno>> GetByOrdenPagoAsync(int ordenPagoId, CancellationToken ct = default);
        Task<OrdenTurno> AddAsync(OrdenTurno entity, CancellationToken ct = default);
        Task UpdateAsync(OrdenTurno entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
