namespace Gym.Application.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UsuarioInfoDto Usuario { get; set; } = new();
}

public class UsuarioInfoDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public string? Rol { get; set; }
    public string? Avatar { get; set; }
    public int TenantId { get; set; }
}
