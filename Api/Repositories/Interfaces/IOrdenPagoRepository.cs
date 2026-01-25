using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IOrdenPagoRepository
    {
        Task<OrdenPago?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<OrdenPago>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<OrdenPago>> GetByEstadoAsync(string estadoNombre, CancellationToken ct = default);
        Task<OrdenPago> AddAsync(OrdenPago entity, CancellationToken ct = default);
        Task UpdateAsync(OrdenPago entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<OrdenPago>> GetPendientesDeSuscripcionAsync(CancellationToken ct = default);
    }
}
