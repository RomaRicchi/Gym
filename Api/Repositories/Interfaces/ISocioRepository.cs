using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface ISocioRepository
    {
        // 🔹 Devuelve socios paginados con total de registros
        Task<(IReadOnlyList<Socio> items, int total)> GetPagedAsync(
            int page,
            int pageSize,
            string? q = null,
            bool? activo = null,
            CancellationToken ct = default);

        // 🔹 Agregar un socio nuevo
        Task<Socio> AddAsync(Socio socio, CancellationToken ct = default);

        // 🔹 Obtener un socio por ID
        Task<Socio?> GetByIdAsync(int id, CancellationToken ct = default);

        // 🔹 Eliminar (devuelve true si se eliminó)
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        // 🔹 Actualizar parcialmente un socio existente (devuelve true si se encontró)
        Task<bool> UpdateAsync(int id, Action<Socio> updateAction, CancellationToken ct = default);

        // 🔹 Verifica si ya existe un socio por DNI o Email
        Task<bool> ExistsAsync(string dni, string email, CancellationToken ct = default);

        // 🔹 Cambiar estado activo/inactivo (devuelve true si se encontró)
        Task<bool> SetActivoAsync(int id, bool value, CancellationToken ct = default);

        // 🔹 Permite consultas personalizadas (sin ejecutar)
        IQueryable<Socio> Query();
    }
}
