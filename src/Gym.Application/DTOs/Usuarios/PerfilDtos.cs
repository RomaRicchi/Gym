namespace Gym.Application.DTOs.Usuarios;

public class PerfilDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public string? RolNombre { get; set; }
    public string? Avatar { get; set; }

    // Datos de Personal (si aplica)
    public string? Nombre { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Especialidad { get; set; }

    // Datos de Socio (si aplica)
    public string? Dni { get; set; }
    public DateTime? FechaNacimiento { get; set; }
}

public class PerfilUpdateRequest
{
    public string? Nombre { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Especialidad { get; set; }
    public bool Estado { get; set; } = true;
    public string? Alias { get; set; }
    public string? Email { get; set; }
}

public class PerfilSocioUpdateRequest
{
    public string? Alias { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Nombre { get; set; }
    public string? Dni { get; set; }
    public DateTime? FechaNacimiento { get; set; }
}
