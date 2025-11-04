using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Models
{
    [Table("evaluacion")]
    public class Evaluacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("rutina_asignada_id")]
        public int RutinaAsignadaId { get; set; }

        [Column("profesor_id")]
        public int? ProfesorId { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        // Relaciones
        [ForeignKey(nameof(RutinaAsignadaId))]
        public virtual RutinaAsignada RutinasAsignadas { get; set; } = null!;

        [ForeignKey(nameof(ProfesorId))]
        public virtual Personal? Profesor { get; set; } = null!;
    }
}
