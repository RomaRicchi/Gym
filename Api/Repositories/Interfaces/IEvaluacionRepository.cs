using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IEvaluacionRepository
    {
        Task<IEnumerable<Evaluacion>> GetAllAsync(CancellationToken ct = default);
        Task<Evaluacion?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Evaluacion> AddAsync(Evaluacion model, CancellationToken ct = default);
        Task<Evaluacion?> UpdateAsync(int id, Evaluacion model, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
