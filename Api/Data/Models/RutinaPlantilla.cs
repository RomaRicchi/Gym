using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Api.Data.Models
{
    public class RutinaPlantilla
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(120)]
        public string? Objetivo { get; set; }

        // 🔗 Relación con grupo muscular
        public int GrupoMuscularId { get; set; }
        public virtual GrupoMuscular GrupoMuscular { get; set; } = null!;

        public string? ImagenUrl { get; set; }

        public virtual ICollection<RutinaPlantillaEjercicio> RutinaPlantillaEjercicios { get; set; }
            = new List<RutinaPlantillaEjercicio>();
    }
}
