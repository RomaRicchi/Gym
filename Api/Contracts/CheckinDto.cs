namespace Api.Contracts
{
    public class CheckinDto
    {
        public int SocioId { get; set; }
        public int TurnoPlantillaId { get; set; }
        public int? ProfesorId { get; set; }         
        public string? Observaciones { get; set; }    
    }
}
