using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface ITurnoPlantillaRepository
    {
        Task<IReadOnlyList<TurnoPlantilla>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<TurnoPlantilla>> GetActivosAsync(CancellationToken ct = default);
        Task<IReadOnlyList<TurnoPlantilla>> GetByDiaSemanaAsync(sbyte dia, CancellationToken ct = default);
        Task<IReadOnlyList<TurnoPlantilla>> GetByPersonalAsync(uint personalId, CancellationToken ct = default);
        Task<TurnoPlantilla?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<TurnoPlantilla> AddAsync(TurnoPlantilla entity, CancellationToken ct = default);
        Task UpdateAsync(TurnoPlantilla entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
