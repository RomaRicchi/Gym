namespace Api.Contracts;

// DTO usado en listados o detalle
public record SocioListItemDto(
    int Id,
    string Dni,
    string Nombre,
    string Email,
    string? Telefono,
    bool? Activo,
    DateTime? CreadoEn
);

// DTO para crear un socio
public record SocioCreateDto(
    string Dni,
    string Nombre,
    string Email,
    string? Telefono
);

// DTO para editar un socio
public record SocioUpdateDto(
    string? Nombre,
    string? Email,
    string? Telefono,
    bool Activo
);
