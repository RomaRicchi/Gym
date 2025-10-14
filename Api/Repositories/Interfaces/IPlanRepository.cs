using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IPlanRepository
    {
        /// <summary>
        /// Obtiene una lista paginada de planes con filtros opcionales.
        /// </summary>
        Task<(IReadOnlyList<Plan> items, int total)> GetPagedAsync(
            int page,
            int pageSize,
            string? q = null,
            int[]? dias = null,
            bool? activo = null,
            CancellationToken ct = default);

        /// <summary>
        /// Obtiene un plan por su ID.
        /// </summary>
        Task<Plan?> GetAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Verifica si existe un plan con el mismo nombre (para evitar duplicados).
        /// </summary>
        Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null, CancellationToken ct = default);

        /// <summary>
        /// Agrega un nuevo plan.
        /// </summary>
        Task<Plan> AddAsync(Plan entity, CancellationToken ct = default);

        /// <summary>
        /// Actualiza un plan existente.
        /// </summary>
        Task<bool> UpdateAsync(Plan entity, CancellationToken ct = default);

        /// <summary>
        /// Marca un plan como inactivo (baja lógica).
        /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
