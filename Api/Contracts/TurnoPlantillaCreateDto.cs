namespace Api.Contracts
{
    public class TurnoPlantillaCreateDto
    {
        public int SalaId { get; set; }
        public int PersonalId { get; set; }
        public byte DiaSemanaId { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public int DuracionMin { get; set; }
        public bool Activo { get; set; }
    }
}
