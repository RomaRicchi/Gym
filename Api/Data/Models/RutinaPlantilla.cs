using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Models
{
    public class RutinaPlantilla
    {
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(120)]
        public string? Objetivo { get; set; }

        //  Relación con la tabla intermedia
        public virtual ICollection<RutinaPlantillaEjercicio> RutinaPlantillaEjercicios { get; set; } 
            = new List<RutinaPlantillaEjercicio>();
    }
}
