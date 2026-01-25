using System;
using System.Collections.Generic;

namespace Api.Data.Models
{
    public partial class OrdenPago
    {
        public int Id { get; set; }

        public int SocioId { get; set; }

        public int PlanId { get; set; }

        public int EstadoId { get; set; }

        public decimal Monto { get; set; }

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        public DateTime? VenceEn { get; set; }

        public string? Notas { get; set; }

        public int? ComprobanteId { get; set; }   // puede ser null si fue pago en efectivo
        public virtual Comprobante? Comprobante { get; set; }


        // 🔹 Relaciones
        public virtual Socio Socio { get; set; } = null!;

        public virtual Plan Plan { get; set; } = null!;

        public virtual EstadoOrdenPago Estado { get; set; } = null!;

       
    }
}
