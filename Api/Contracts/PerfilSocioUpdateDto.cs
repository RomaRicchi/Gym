using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Contracts
{
    public class PerfilSocioUpdateDto
    {
        public string? Alias { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Nombre { get; set; }
    }
}

