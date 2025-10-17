namespace Api.Contracts
{
    public record OrdenPagoCreateDto(
        int SocioId,
        int PlanId,
        int EstadoId,
        decimal Monto,
        DateTime? VenceEn = null,
        string? Notas = null
    );
}
