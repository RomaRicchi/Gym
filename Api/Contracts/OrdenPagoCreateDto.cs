using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Api.Contracts
{
    public class OrdenPagoCreateDto
    {
        [Required(ErrorMessage = "El socio es obligatorio.")]
        public int SocioId { get; set; }

        [Required(ErrorMessage = "El plan es obligatorio.")]
        public int PlanId { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public int EstadoId { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        public DateTime FechaInicio { get; set; }

        public string? Notas { get; set; }

        // Archivo opcional (PDF o imagen)
        public IFormFile? File { get; set; }
    }
}
