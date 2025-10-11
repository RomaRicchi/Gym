using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class SuscripcionTurno
{
    public uint Id { get; set; }

    public uint SuscripcionId { get; set; }

    public uint TurnoPlantillaId { get; set; }

    public virtual Suscripcion Suscripcion { get; set; } = null!;

    public virtual TurnoPlantilla TurnoPlantilla { get; set; } = null!;
}
