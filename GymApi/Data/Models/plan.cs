using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;

public partial class plan
{
    public uint id { get; set; }

    public string nombre { get; set; } = null!;

    public int dias_por_semana { get; set; }

    public decimal precio { get; set; }

    public bool? activo { get; set; }

    public virtual ICollection<orden_pago> orden_pago { get; set; } = new List<orden_pago>();

    public virtual ICollection<rutina_plantilla> rutina_plantilla { get; set; } = new List<rutina_plantilla>();

    public virtual ICollection<suscripcion> suscripcion { get; set; } = new List<suscripcion>();
}
