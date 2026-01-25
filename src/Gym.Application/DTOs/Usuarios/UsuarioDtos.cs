namespace Gym.Application.DTOs.Usuarios;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public int RolId { get; set; }
    public string? RolNombre { get; set; }
    public bool Estado { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UsuarioCreateRequest
{
    public string Email { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public string Password { get; set; } = string.Empty;
    public int RolId { get; set; }
    public int? PersonalId { get; set; }
    public int? SocioId { get; set; }
}

public class UsuarioUpdateRequest
{
    public string Email { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public int RolId { get; set; }
    public bool Estado { get; set; }
}
