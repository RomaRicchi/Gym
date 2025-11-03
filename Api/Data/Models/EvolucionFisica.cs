using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Models
{
    [Table("evolucion_fisica")]
    public class EvolucionFisica
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // ⚠️ Eliminamos la relación conflictiva (SocioId1)
        [Column("socio_id")]
        public int SocioId { get; set; }

        // ⚠️ Marcamos como ignorado para que EF no genere SocioId1
        [NotMapped]
        public Socio? Socio { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Column("peso")]
        public decimal Peso { get; set; }

        [Column("altura")]
        public decimal Altura { get; set; }

        // 👇 Fuerza el nombre exacto en la DB
        [Column("imc")]
        public decimal? IMC { get; set; }

        [Column("peso_ideal")]
        public decimal? PesoIdeal { get; set; }

        [Column("pecho")]
        public decimal? Pecho { get; set; }

        [Column("cintura")]
        public decimal? Cintura { get; set; }

        [Column("cadera")]
        public decimal? Cadera { get; set; }

        [Column("brazo")]
        public decimal? Brazo { get; set; }

        [Column("pierna")]
        public decimal? Pierna { get; set; }

        [Column("gemelo")]
        public decimal? Gemelo { get; set; }

        [Column("observacion")]
        public string? Observacion { get; set; }
    }
}
