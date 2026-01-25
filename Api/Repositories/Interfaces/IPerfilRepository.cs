using Api.Data.Models;

namespace Api.Repositories.Interfaces
{
    public interface IPerfilRepository
    {
        Task<Usuario?> GetPerfilAsync(int id, CancellationToken ct);
        Task<Usuario?> GetPerfilDetalladoAsync(int id, CancellationToken ct);
        Task<bool> UpdatePerfilAsync(int id, string? nombre, string? email, string? telefono, int? idAvatar, CancellationToken ct);
        Task<Avatar> SubirAvatarAsync(int id, Avatar avatar, CancellationToken ct);
        Task<bool> CambiarPasswordAsync(int id, string passwordActual, string nuevaPassword, CancellationToken ct);
    }
}
