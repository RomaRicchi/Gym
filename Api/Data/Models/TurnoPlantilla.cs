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
        public int Id { get; set; }

        [Column("activo")]
        public bool Activo { get; set; }

        [Column("duracion_min")]
        public int DuracionMin { get; set; }

        [Column("hora_inicio")]
        public TimeSpan HoraInicio { get; set; }

        // 🔹 Relación con DiaSemana
        [Column("dia_semana_id")]
        public byte DiaSemanaId { get; set; }

        [ForeignKey(nameof(DiaSemanaId))]
        public DiaSemana DiaSemana { get; set; }

        // 🔹 Relación con Personal
        [Column("personal_id")]
        public int PersonalId { get; set; }

        [ForeignKey(nameof(PersonalId))]
        public Personal Personal { get; set; }

        // 🔹 Relación con Sala
        [Column("sala_id")]
        public int SalaId { get; set; }

        [ForeignKey(nameof(SalaId))]
        public Sala Sala { get; set; }
    }
}
