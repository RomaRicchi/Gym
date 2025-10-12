using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IEstadoOrdenPagoRepository
    {
        Task<IReadOnlyList<EstadoOrdenPago>> GetAllAsync(CancellationToken ct = default);
        Task<EstadoOrdenPago?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<EstadoOrdenPago?> GetByNombreAsync(string nombre, CancellationToken ct = default);
        Task<EstadoOrdenPago> AddAsync(EstadoOrdenPago entity, CancellationToken ct = default);
        Task UpdateAsync(EstadoOrdenPago entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
