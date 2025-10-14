using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class SuscripcionTurno
{
    public int Id { get; set; }

    public int SuscripcionId { get; set; }

    public int TurnoPlantillaId { get; set; }

    public virtual Suscripcion Suscripcion { get; set; } = null!;

    public virtual TurnoPlantilla TurnoPlantilla { get; set; } = null!;
}
