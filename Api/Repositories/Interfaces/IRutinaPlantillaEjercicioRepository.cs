using Api.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    public interface IRutinaPlantillaEjercicioRepository
    {
        Task<IEnumerable<RutinaPlantillaEjercicio>> GetAllAsync(CancellationToken ct = default);
        Task<RutinaPlantillaEjercicio?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<RutinaPlantillaEjercicio>> GetByRutinaIdAsync(int rutinaId, CancellationToken ct = default);
        Task<RutinaPlantillaEjercicio> AddAsync(RutinaPlantillaEjercicio entity, CancellationToken ct = default);
        Task<RutinaPlantillaEjercicio?> UpdateAsync(int id, RutinaPlantillaEjercicio entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
