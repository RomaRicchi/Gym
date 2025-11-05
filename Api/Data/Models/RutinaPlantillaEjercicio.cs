using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Api.Data.Models
{
    public class RutinaPlantillaEjercicio
    {
        public int Id { get; set; }

        [ForeignKey(nameof(RutinaPlantilla))]
        public int RutinaId { get; set; }

        [ForeignKey(nameof(Ejercicio))]
        public int EjercicioId { get; set; }

        public int Orden { get; set; }
        public int Series { get; set; }
        public int Repeticiones { get; set; }
        public int DescansoSeg { get; set; }

        [ValidateNever]
        [JsonIgnore]
        public virtual RutinaPlantilla RutinaPlantilla { get; set; } = null!;

        [ValidateNever]
        [JsonIgnore]
        public virtual Ejercicio Ejercicio { get; set; } = null!;
    }
}
