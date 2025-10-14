using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class OrdenTurno
{
    public int Id { get; set; }
    public int OrdenPagoId { get; set; }
    public int TurnoPlantillaId { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }

    public virtual OrdenPago OrdenPago { get; set; } = null!;
    public virtual TurnoPlantilla TurnoPlantilla { get; set; } = null!;
}

