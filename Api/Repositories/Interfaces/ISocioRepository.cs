using Api.Data.Models;

namespace Api.Repositories.Interfaces;

public interface ISocioRepository
{
    Task<(IReadOnlyList<Socio> items, int total)> GetPagedAsync(
        int page, int pageSize, string? q = null, bool? activo = null, CancellationToken ct = default);

    Task<Socio?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<bool> ExistsAsync(string dni, string email, CancellationToken ct = default);

    Task<Socio> AddAsync(Socio entity, CancellationToken ct = default);

    /// <summary>Actualiza campos editables del socio (nombre, email, teléfono, activo).</summary>
    Task<bool> UpdateAsync(int id, Action<Socio> apply, CancellationToken ct = default);

    /// <summary>Baja lógica: cambia el flag 'activo'.</summary>
    Task<bool> SetActivoAsync(int id, bool value, CancellationToken ct = default);

    /// <summary>Baja física.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    IQueryable<Socio> Query(); 
}
