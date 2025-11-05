using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IEvaluacionRepository
    {
        Task<IReadOnlyList<Evaluacion>> GetAllAsync(CancellationToken ct = default);
        Task<Evaluacion?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<Evaluacion>> GetByProfesorIdAsync(int profesorId, CancellationToken ct = default);
        Task<IReadOnlyList<Evaluacion>> GetByRutinaAsignadaIdAsync(int rutinaAsignadaId, CancellationToken ct = default);
        Task<Evaluacion> AddAsync(Evaluacion model, CancellationToken ct = default);
        Task<Evaluacion?> UpdateAsync(int id, Evaluacion model, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
