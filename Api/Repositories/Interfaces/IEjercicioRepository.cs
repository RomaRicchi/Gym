using Api.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    public interface IEjercicioRepository
    {
        Task<IEnumerable<Ejercicio>> GetAllAsync(CancellationToken ct = default);
        Task<Ejercicio?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Ejercicio> AddAsync(Ejercicio entity, CancellationToken ct = default);
        Task<Ejercicio?> UpdateAsync(int id, Ejercicio entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
