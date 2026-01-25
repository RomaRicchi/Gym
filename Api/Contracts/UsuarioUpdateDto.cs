namespace Api.Contracts
{
    public class UsuarioUpdateDto
    {
        public string Email { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public int RolId { get; set; }
        public bool Estado { get; set; }
    }
}
