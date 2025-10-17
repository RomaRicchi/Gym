using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Data.Models
{
    public partial class Comprobante
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        [StringLength(100)]
        public string? MimeType { get; set; }

        public DateTime SubidoEn { get; set; } = DateTime.UtcNow;

        // 🔹 Relación inversa 1:1 opcional con OrdenPago
        public virtual OrdenPago? OrdenPago { get; set; }
    }
}
