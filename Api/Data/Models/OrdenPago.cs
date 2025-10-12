using System;
using System.Collections.Generic;

namespace Api.Data.Models
{
    public partial class OrdenPago
    {
        public uint Id { get; set; }

        public uint SocioId { get; set; }

        public uint PlanId { get; set; }

        public uint? SuscripcionId { get; set; }

        public decimal Monto { get; set; }

        public DateTime VenceEn { get; set; }

        public uint EstadoId { get; set; }   // FK a estado_orden_pago

        public DateTime CreadoEn { get; set; }

        public string? Notas { get; set; }

        // 🔗 Relaciones
        public virtual EstadoOrdenPago? Estado { get; set; } = null!;
        public virtual Plan? Plan { get; set; } = null!;
        public virtual Socio? Socio { get; set; } = null!;
        public virtual Suscripcion? Suscripcion { get; set; }
        public virtual ICollection<Comprobante> Comprobantes { get; set; } = new List<Comprobante>();
        public virtual ICollection<OrdenTurno> OrdenesTurno { get; set; } = new List<OrdenTurno>();
    }
}
