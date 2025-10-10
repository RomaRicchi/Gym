using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class Plan
{
    public uint Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int Dias_por_semana { get; set; }

    public decimal Precio { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<orden_pago> Orden_pago { get; set; } = new List<orden_pago>();

    public virtual ICollection<rutina_plantilla> Rutina_plantilla { get; set; } = new List<rutina_plantilla>();

    public virtual ICollection<suscripcion> Suscripcion { get; set; } = new List<suscripcion>();
}
