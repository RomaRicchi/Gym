using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Models
{
    [Table("plan")]
    public class Plan
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("dias_por_semana")]
        public int DiasPorSemana { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("activo")]
        public bool Activo { get; set; }

        // Relaciones
        public virtual ICollection<RutinaAsignada>? RutinasAsignadas { get; set; }
        public virtual ICollection<Suscripcion>? Suscripciones { get; set; }
    }
}
