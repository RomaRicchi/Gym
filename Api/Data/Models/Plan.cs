using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class Plan
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int DiasPorSemana { get; set; }

    public decimal Precio { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<OrdenPago> OrdenesPago { get; set; } = new List<OrdenPago>();

    public virtual ICollection<RutinaPlantilla> RutinasPlantilla { get; set; } = new List<RutinaPlantilla>();

    public virtual ICollection<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
}
