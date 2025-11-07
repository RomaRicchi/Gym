using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Api.Data.Models
{
    public partial class Ejercicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Tips { get; set; }
        public string? MediaUrl { get; set; }

        // 🔗 Relación con grupo muscular
        public int GrupoMuscularId { get; set; }
        public virtual GrupoMuscular GrupoMuscular { get; set; } = null!;

        public virtual ICollection<RutinaPlantillaEjercicio> RutinaPlantillaEjercicios { get; set; }
            = new List<RutinaPlantillaEjercicio>();
    }
}
