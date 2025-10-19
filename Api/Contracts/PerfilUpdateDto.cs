using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Contracts
{
    public class PerfilUpdateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Especialidad { get; set; }
        public bool Estado { get; set; } = true;
    }
}
