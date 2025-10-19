namespace Api.Data.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expira { get; set; }

        public Usuario Usuario { get; set; } = null!;
    }
}
