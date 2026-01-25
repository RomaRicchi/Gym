using Api.Data.Models;
using Api.Contracts.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories.Interfaces
{
    public interface IRutinaPlantillaEjercicioRepository
    {
        // 🔹 Obtener todos los registros (usando DTO)
        Task<IEnumerable<RutinaPlantillaEjercicioDto>> GetAllAsync(CancellationToken ct = default);

        // 🔹 Obtener un registro por ID
        Task<RutinaPlantillaEjercicio?> GetByIdAsync(int id, CancellationToken ct = default);

        // 🔹 Obtener todos los ejercicios de una rutina específica
        Task<IReadOnlyList<RutinaPlantillaEjercicio>> GetByRutinaIdAsync(int rutinaId, CancellationToken ct = default);

        // 🔹 Agregar un nuevo registro
        Task<RutinaPlantillaEjercicio> AddAsync(RutinaPlantillaEjercicio entity, CancellationToken ct = default);

        // 🔹 Actualizar un registro existente
        Task<RutinaPlantillaEjercicio?> UpdateAsync(int id, RutinaPlantillaEjercicio entity, CancellationToken ct = default);

        // 🔹 Eliminar un registro
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
