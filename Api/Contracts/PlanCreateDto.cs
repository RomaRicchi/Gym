namespace Api.Contracts
{
    public class PlanCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int DiasPorSemana { get; set; }
        public decimal Precio { get; set; }
        public bool Activo { get; set; } = true;
    }
}
