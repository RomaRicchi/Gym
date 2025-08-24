using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class rutina_plantilla
{
    public uint id { get; set; }

    public string nombre { get; set; } = null!;

    public string? objetivo { get; set; }

    public uint? plan_id { get; set; }

    public virtual plan? plan { get; set; }

    public virtual ICollection<rutina_asignada> rutina_asignada { get; set; } = new List<rutina_asignada>();

    public virtual ICollection<rutina_plantilla_ejercicio> rutina_plantilla_ejercicio { get; set; } = new List<rutina_plantilla_ejercicio>();
}
