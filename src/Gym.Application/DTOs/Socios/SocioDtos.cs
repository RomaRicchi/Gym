namespace Gym.Application.DTOs.Socios;

public record SocioDto(
    int Id,
    string Dni,
    string Nombre,
    string Email,
    string? Telefono,
    bool Activo,
    DateTime CreatedAt,
    DateTime? FechaNacimiento,
    string? PlanActual
);

public record SocioCreateRequest(
    string Dni,
    string Nombre,
    string Email,
    string? Telefono,
    DateTime? FechaNacimiento
);

public record SocioUpdateRequest(
    string? Nombre,
    string? Email,
    string? Telefono,
    DateTime? FechaNacimiento,
    bool Activo
);

public class SocioDetalleDto
{
    public int Id { get; set; }
    public string Dni { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? PlanActual { get; set; }
    public int? SuscripcionesActivas { get; set; }
}
