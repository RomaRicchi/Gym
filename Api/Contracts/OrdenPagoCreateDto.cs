namespace Api.Contracts
{
    public class OrdenPagoCreateDto
    {
        public int SocioId { get; set; }
        public int PlanId { get; set; }
        public int EstadoId { get; set; }
        public DateTime VenceEn { get; set; }
        public string? Notas { get; set; }
        public int? ComprobanteId { get; set; }
    }
}

