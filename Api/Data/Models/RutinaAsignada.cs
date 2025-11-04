using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Models
{
    [Table("rutina_asignada")]
    public class RutinaAsignada
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("socio_id")]
        public int SocioId { get; set; }

        [Column("rutina_id")]
        public int RutinaId { get; set; }

        [Column("plan_id")]
        public int? PlanId { get; set; }

        [Column("inicio")]
        public DateTime Inicio { get; set; }

        [Column("fin")]
        public DateTime Fin { get; set; }

        [Column("notas")]
        public string? Notas { get; set; }

        // Relaciones
        [ForeignKey(nameof(SocioId))]
        public virtual Socio Socio { get; set; } = null!;

        [ForeignKey(nameof(RutinaId))]
        public virtual RutinaPlantilla RutinasPlantilla { get; set; } = null!;

        [ForeignKey(nameof(PlanId))]
        public virtual Plan? Plan { get; set; }

        public virtual ICollection<Evaluacion>? Evaluaciones { get; set; }
    }
}
