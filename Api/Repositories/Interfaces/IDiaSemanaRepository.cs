using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IDiaSemanaRepository
    {
        Task<IEnumerable<DiaSemana>> GetAllAsync(CancellationToken ct = default);
        Task<DiaSemana?> GetByIdAsync(byte id, CancellationToken ct = default);
    }
}
