using System.Collections.Generic;

namespace Api.Data.Models
{
    public partial class EstadoOrdenPago
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }


        public virtual ICollection<OrdenPago> OrdenesPago { get; set; } = new List<OrdenPago>();
    }
}
