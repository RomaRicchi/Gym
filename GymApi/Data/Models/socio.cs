using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class socio
{
    public uint id { get; set; }

    public string dni { get; set; } = null!;

    public string nombre { get; set; } = null!;

    public string email { get; set; } = null!;

    public string? telefono { get; set; }

    public bool? activo { get; set; }

    public DateTime creado_en { get; set; }

    public virtual ICollection<checkin> checkin { get; set; } = new List<checkin>();

    public virtual ICollection<orden_pago> orden_pago { get; set; } = new List<orden_pago>();

    public virtual ICollection<rutina_asignada> rutina_asignada { get; set; } = new List<rutina_asignada>();

    public virtual ICollection<suscripcion> suscripcion { get; set; } = new List<suscripcion>();
}
