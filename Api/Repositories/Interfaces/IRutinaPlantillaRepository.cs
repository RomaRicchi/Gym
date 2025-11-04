using Api.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    public interface IRutinaPlantillaRepository
    {
        Task<IEnumerable<RutinaPlantilla>> GetAllAsync(CancellationToken ct = default);
        Task<RutinaPlantilla?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<RutinaPlantilla> AddAsync(RutinaPlantilla entity, CancellationToken ct = default);
        Task<RutinaPlantilla?> UpdateAsync(int id, RutinaPlantilla entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
