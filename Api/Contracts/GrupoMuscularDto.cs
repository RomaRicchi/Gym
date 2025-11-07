namespace Api.Contracts
{
    public class GrupoMuscularDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? ImagenUrl { get; set; }
    }
}
