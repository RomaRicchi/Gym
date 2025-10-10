namespace Api.Contracts;

public record SocioListItemDto(
    uint id, string dni, string nombre, string email, string? telefono, bool? activo, DateTime? creado_en);

public record SocioCreateDto(
    string dni, string nombre, string email, string? telefono);

public record SocioUpdateDto(
    string nombre, string email, string? telefono, bool? activo);
