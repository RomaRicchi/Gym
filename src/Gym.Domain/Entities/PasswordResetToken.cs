namespace Gym.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    public int UsuarioId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expira { get; set; }

    // Navegación
    public virtual Usuario Usuario { get; set; } = null!;
}
