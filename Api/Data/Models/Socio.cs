using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Data.Models;

public partial class Socio
{
    public int Id { get; set; }

    public string Dni { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public bool? Activo { get; set; }

    public DateTime CreadoEn { get; set; }

    public virtual ICollection<Checkin> Checkins { get; set; } = new List<Checkin>();

    public virtual ICollection<OrdenPago> OrdenesPago { get; set; } = new List<OrdenPago>();

    public virtual ICollection<RutinaAsignada> RutinasAsignadas { get; set; } = new List<RutinaAsignada>();

    public virtual ICollection<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
}
