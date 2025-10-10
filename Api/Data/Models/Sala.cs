using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class Sala
{
    public uint Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int Capacidad { get; set; }

    public bool? Activa { get; set; }

    public virtual ICollection<turno_plantilla> Turno_plantilla { get; set; } = new List<turno_plantilla>();
}
