using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Data.Models
{
    public partial class Comprobante
    {
        public uint Id { get; set; }

        public uint OrdenPagoId { get; set; }

        public string FileUrl { get; set; } = string.Empty;
        
        public string MimeType { get; set; } = string.Empty;

        public DateTime SubidoEn { get; set; }

        public virtual OrdenPago OrdenPago { get; set; } = null!;
    }
}
