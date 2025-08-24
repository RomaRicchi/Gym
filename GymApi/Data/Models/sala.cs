using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class sala
{
    public uint id { get; set; }

    public string nombre { get; set; } = null!;

    public int capacidad { get; set; }

    public bool? activa { get; set; }

    public virtual ICollection<turno_plantilla> turno_plantilla { get; set; } = new List<turno_plantilla>();
}
