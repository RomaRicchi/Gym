using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Models
{
    [Table("suscripcion_turno")]
    public class SuscripcionTurno
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("suscripcion_id")]
        public int SuscripcionId { get; set; }

        [ForeignKey(nameof(SuscripcionId))]
        public Suscripcion Suscripcion { get; set; } = null!;

        [Required]
        [Column("turno_plantilla_id")]
        public int TurnoPlantillaId { get; set; }

        [ForeignKey(nameof(TurnoPlantillaId))]
        public TurnoPlantilla TurnoPlantilla { get; set; } = null!;
    }
}
