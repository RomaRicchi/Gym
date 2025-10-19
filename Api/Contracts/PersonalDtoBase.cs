namespace Api.Contracts
{
    public class PersonalDtoBase
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Especialidad { get; set; }
        public string? Direccion { get; set; }
        public bool Activo { get; set; }
        public string? Email { get; set; }
        public int? RolId { get; set; }
    }

    public class PersonalCreateDto : PersonalDtoBase
    {
    }
        
     public class PersonalUpdateDto : PersonalDtoBase
    {
    }

}
