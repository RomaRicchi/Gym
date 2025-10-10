using Api.Data.Models;

namespace Api.Repositories.Interfaces;

public interface ISocioRepository
{
    Task<(IReadOnlyList<socio> items, int total)> GetPagedAsync(
        int page, int pageSize, string? q = null, bool? activo = null, CancellationToken ct = default);

    Task<socio?> GetByIdAsync(uint id, CancellationToken ct = default);

    Task<bool> ExistsAsync(string dni, string email, CancellationToken ct = default);

    Task<socio> AddAsync(socio entity, CancellationToken ct = default);

    /// <summary>Actualiza campos editables del socio (nombre, email, teléfono, activo).</summary>
    Task<bool> UpdateAsync(uint id, Action<socio> apply, CancellationToken ct = default);

    /// <summary>Baja lógica: cambia el flag 'activo'.</summary>
    Task<bool> SetActivoAsync(uint id, bool value, CancellationToken ct = default);

    /// <summary>Baja física.</summary>
    Task<bool> DeleteAsync(uint id, CancellationToken ct = default);
}
