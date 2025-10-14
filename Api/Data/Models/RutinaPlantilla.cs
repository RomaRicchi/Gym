using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class RutinaPlantilla
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Objetivo { get; set; }

    public int? PlanId { get; set; }

    public virtual Plan? Plan { get; set; }

    public virtual ICollection<RutinaAsignada> RutinasAsignadas { get; set; } = new List<RutinaAsignada>();

    public virtual ICollection<RutinaPlantillaEjercicio> RutinaPlantillaEjercicios { get; set; } = new List<RutinaPlantillaEjercicio>();
}
