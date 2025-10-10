using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class Socio
{
    public uint Id { get; set; }

    public string Dni { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public bool? Activo { get; set; }

    public DateTime Creado_en { get; set; }

    public virtual ICollection<checkin> Checkin { get; set; } = new List<checkin>();

    public virtual ICollection<orden_pago> Orden_pago { get; set; } = new List<orden_pago>();

    public virtual ICollection<rutina_asignada> Rutina_asignada { get; set; } = new List<rutina_asignada>();

    public virtual ICollection<suscripcion> Suscripcion { get; set; } = new List<suscripcion>();
}
