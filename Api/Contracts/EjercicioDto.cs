namespace Api.Contracts
{
    public class EjercicioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Tips { get; set; }
        public string? MediaUrl { get; set; }
        public int GrupoMuscularId { get; set; }
        public string? GrupoMuscularNombre { get; set; }
    }
}
