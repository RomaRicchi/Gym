using System.Collections.Generic;

namespace Api.Data.Models
{
    public class GrupoMuscular
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? ImagenUrl { get; set; }

        // Relaciones
        public virtual ICollection<Ejercicio> Ejercicios { get; set; } = new List<Ejercicio>();
        public virtual ICollection<RutinaPlantilla> RutinasPlantilla { get; set; } = new List<RutinaPlantilla>();
    }
}
