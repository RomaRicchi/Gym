namespace Api.Data.Models
{
    public class SuscripcionCreateDto
    {
        public int SocioId { get; set; }
        public int PlanId { get; set; }
        public int? OrdenPagoId { get; set; } 
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
    }
}
