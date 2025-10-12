using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Models
{
    [Table("turno_plantilla")]
    public class TurnoPlantilla
    {
        [Key]
        [Column("id")]
        public uint Id { get; set; }

        [Column("sala_id")]
        public uint SalaId { get; set; }

        [ForeignKey("SalaId")]
        public Sala? Sala { get; set; }

        [Column("personal_id")]
        public uint PersonalId { get; set; }

        [ForeignKey("PersonalId")]
        public Personal? Personal { get; set; }

        // ✅ FK a dia_semana
        [Column("dia_semana")]
        [ForeignKey("DiaSemana")]
        public byte DiaSemanaId { get; set; }

        public DiaSemana? DiaSemana { get; set; }

        [Column("hora_inicio")]
        public TimeSpan HoraInicio { get; set; }

        [Column("duracion_min")]
        public int DuracionMin { get; set; }

        [Column("cupo")]
        public int Cupo { get; set; }

        [Column("activo")]
        public bool Activo { get; set; }
    }
}
