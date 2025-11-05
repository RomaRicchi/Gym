using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Models
{
    [Table("evaluacion")]
    public class Evaluacion
    {
        [Key]
        public int Id { get; set; }

        [Column("rutina_asignada_id")]
        [ForeignKey(nameof(RutinaAsignada))]
        public int RutinaAsignadaId { get; set; }

        [Column("profesor_id")]
        [ForeignKey(nameof(Profesor))]
        public int ProfesorId { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        // Relaciones opcionales
        public virtual RutinaAsignada? RutinaAsignada { get; set; }
        public virtual Personal? Profesor { get; set; }
    }
}
