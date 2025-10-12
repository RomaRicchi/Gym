using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class OrdenTurno
{
    public uint Id { get; set; }
    public uint OrdenPagoId { get; set; }
    public uint TurnoPlantillaId { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }

    public virtual OrdenPago OrdenPago { get; set; } = null!;
    public virtual TurnoPlantilla TurnoPlantilla { get; set; } = null!;
}

