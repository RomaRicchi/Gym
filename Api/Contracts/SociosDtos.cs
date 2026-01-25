namespace Api.Contracts;

// DTO usado en listados o detalle
public record SocioListItemDto(
    int Id,
    string Dni,
    string Nombre,
    string Email,
    string? Telefono,
    bool? Activo,
    DateTime? CreadoEn,
    DateTime? FechaNacimiento,
    string? PlanActual 
);

// DTO para crear un socio
public record SocioCreateDto(
    string Dni,
    string Nombre,
    string Email,
    string? Telefono,
    DateTime? FechaNacimiento
);

// DTO para editar un socio
public record SocioUpdateDto(
    string? Nombre,
    string? Email,
    string? Telefono,
    DateTime? FechaNacimiento,
    bool Activo
);
