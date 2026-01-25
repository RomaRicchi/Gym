namespace Api.Contracts
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioDto Usuario { get; set; } = new();
    }

    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public string? Rol { get; set; }
        public string? Avatar { get; set; }
    }
}
