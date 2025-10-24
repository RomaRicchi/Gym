using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface ICheckinRepository
    {
        Task<List<Checkin>> GetAllAsync(CancellationToken ct = default);
        Task<Checkin?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<Checkin>> GetBySocioAsync(int socioId, CancellationToken ct = default);
        Task<List<Checkin>> GetByTurnoAsync(int turnoPlantillaId, CancellationToken ct = default);
        Task<Checkin> AddAsync(Checkin entity, CancellationToken ct = default);
        Task UpdateAsync(Checkin entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
