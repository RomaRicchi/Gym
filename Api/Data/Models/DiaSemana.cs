using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Models
{
    [Table("dia_semana")]
    public class DiaSemana
    {
        [Key]
        [Column("id")]
        public byte Id { get; set; }   // byte equivale a TINYINT UNSIGNED

        [Required]
        [Column("nombre")]
        [StringLength(20)]
        public string Nombre { get; set; } = string.Empty;

        // Relación inversa opcional
        public ICollection<TurnoPlantilla>? TurnosPlantilla { get; set; }
    }
}
