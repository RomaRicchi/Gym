namespace Api.Dtos
{
    public class RutinaPlantillaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Objetivo { get; set; }
        public int GrupoMuscularId { get; set; }
        public string? GrupoMuscularNombre { get; set; }
        public string? ImagenUrl { get; set; }
    }
}
