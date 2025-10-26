using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface ITurnoPlantillaRepository
    {
        Task<IReadOnlyList<TurnoPlantilla>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<TurnoPlantilla>> GetActivosAsync(CancellationToken ct = default);
        Task<IReadOnlyList<TurnoPlantilla>> GetByDiaSemanaAsync(byte diaSemana, CancellationToken ct = default);
        Task<IReadOnlyList<TurnoPlantilla>> GetByPersonalAsync(int personalId, CancellationToken ct = default);
        Task<TurnoPlantilla?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<TurnoPlantilla> AddAsync(TurnoPlantilla turno, CancellationToken ct = default);
        Task<bool> UpdateAsync(TurnoPlantilla turno, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExisteSolapamientoAsync(int salaId, byte diaSemana, TimeSpan horaInicio, int duracionMin, CancellationToken ct = default);
    }
}
