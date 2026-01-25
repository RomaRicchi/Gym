namespace Gym.Application.DTOs.Personal;

public class PersonalDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Especialidad { get; set; }
    public bool Estado { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PersonalCreateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Especialidad { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;
    public string? Email { get; set; }
    public int? RolId { get; set; }
}

public class PersonalUpdateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Especialidad { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; }
    public string? Email { get; set; }
    public int? RolId { get; set; }
}
