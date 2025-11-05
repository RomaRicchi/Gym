using Api.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    public interface IEjercicioRepository
    {
        Task<(IEnumerable<Ejercicio> items, int totalItems)> GetPagedAsync(
            int page, int pageSize, string? q, CancellationToken ct);

        Task<Ejercicio?> GetByIdAsync(int id, CancellationToken ct);
        Task<Ejercicio> AddAsync(Ejercicio model, CancellationToken ct);
        Task<Ejercicio?> UpdateAsync(int id, Ejercicio model, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
