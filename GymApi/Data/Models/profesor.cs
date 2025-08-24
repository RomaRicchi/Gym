using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class profesor
{
    public uint id { get; set; }

    public string nombre { get; set; } = null!;

    public string? email { get; set; }

    public string estado { get; set; } = null!;

    public virtual ICollection<turno_plantilla> turno_plantilla { get; set; } = new List<turno_plantilla>();
}
