namespace Api.Contracts.Dtos
{
    public class RutinaPlantillaEjercicioDto
    {
        public int Id { get; set; }
        public int RutinaId { get; set; }
        public string RutinaNombre { get; set; } = string.Empty;
        public int EjercicioId { get; set; }
        public string EjercicioNombre { get; set; } = string.Empty;
        public int Orden { get; set; }
        public int Series { get; set; }
        public int Repeticiones { get; set; }
        public int DescansoSeg { get; set; }
    }
}
