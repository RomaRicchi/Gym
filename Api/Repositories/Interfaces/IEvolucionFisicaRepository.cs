using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IEvolucionFisicaRepository
    {
        Task<EvolucionFisica> CrearAsync(EvolucionFisica entidad, CancellationToken ct);
        Task<List<EvolucionFisica>> GetBySocioAsync(int socioId, CancellationToken ct);
        Task<EvolucionFisica?> GetByIdAsync(int id, CancellationToken ct);
        Task<bool> ActualizarAsync(EvolucionFisica dto, CancellationToken ct);
        Task<bool> EliminarAsync(int id, CancellationToken ct);
    }
}
