using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Models
{
    [Table("checkin")]
    public partial class Checkin
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("socio_id")]
        public int SocioId { get; set; }

        [Column("turno_plantilla_id")]
        public int TurnoPlantillaId { get; set; }

        [Column("fecha_hora")]
        public DateTime FechaHora { get; set; }

        [Column("origen")]
        public string Origen { get; set; } = "recepcion";

        [ForeignKey(nameof(SocioId))]
        public virtual Socio Socio { get; set; } = null!;

        [ForeignKey(nameof(TurnoPlantillaId))]
        public virtual TurnoPlantilla TurnoPlantilla { get; set; } = null!;
    }
}
