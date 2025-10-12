using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface ITurnoPlantillaRepository
    {
        Task<IReadOnlyList<TurnoPlantilla>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<TurnoPlantilla>> GetActivosAsync(CancellationToken ct = default);
        Task<IReadOnlyList<TurnoPlantilla>> GetByDiaSemanaAsync(sbyte diaSemana, CancellationToken ct = default);
        Task<IReadOnlyList<TurnoPlantilla>> GetByPersonalAsync(uint personalId, CancellationToken ct = default);
        Task<TurnoPlantilla?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<TurnoPlantilla> AddAsync(TurnoPlantilla turno, CancellationToken ct = default);
        Task<bool> UpdateAsync(TurnoPlantilla turno, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExisteSolapamientoAsync(uint salaId, byte diaSemana, TimeSpan horaInicio, int duracionMin, CancellationToken ct = default);
    }
}
