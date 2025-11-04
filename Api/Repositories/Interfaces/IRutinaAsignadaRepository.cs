using Api.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    public interface IRutinaAsignadaRepository
    {
        Task<IEnumerable<RutinaAsignada>> GetAllAsync(CancellationToken ct = default);
        Task<RutinaAsignada?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<RutinaAsignada>> GetBySocioIdAsync(int socioId, CancellationToken ct = default);
        Task<RutinaAsignada> AddAsync(RutinaAsignada entity, CancellationToken ct = default);
        Task<RutinaAsignada?> UpdateAsync(int id, RutinaAsignada entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
