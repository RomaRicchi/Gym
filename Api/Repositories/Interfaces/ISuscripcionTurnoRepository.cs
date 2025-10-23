using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface ISuscripcionTurnoRepository
    {
        // 🔹 Obtener todos los registros
        Task<IReadOnlyList<SuscripcionTurno>> GetAllAsync(CancellationToken ct = default);

        // 🔹 Obtener un registro específico por ID
        Task<SuscripcionTurno?> GetByIdAsync(int id, CancellationToken ct = default);

        // 🔹 Obtener turnos por suscripción
        Task<IReadOnlyList<SuscripcionTurno>> GetBySuscripcionAsync(int suscripcionId, CancellationToken ct = default);

        // 🔹 Obtener turnos por socio (nuevo método)
        Task<IReadOnlyList<SuscripcionTurno>> GetBySocioAsync(int socioId, CancellationToken ct = default);

        // 🔹 Crear un nuevo registro
        Task<SuscripcionTurno> AddAsync(SuscripcionTurno entity, CancellationToken ct = default);

        // 🔹 Actualizar un registro existente
        Task UpdateAsync(SuscripcionTurno entity, CancellationToken ct = default);

        // 🔹 Eliminar un registro
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
