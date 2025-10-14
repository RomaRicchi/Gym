namespace Api.Contracts
{
    public record OrdenPagoCreateDto(
        int SocioId,
        int PlanId,
        decimal Monto,
        DateTime? VenceEn,
        int EstadoId,
        string? Notas
    );
}
