using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class Checkin
{
    public uint Id { get; set; }

    public uint SocioId { get; set; }

    public uint? TurnoPlantillaId { get; set; }

    public DateTime FechaHora { get; set; }

    public string Origen { get; set; } = null!;

    public virtual Socio Socio { get; set; } = null!;

    public virtual TurnoPlantilla? TurnoPlantilla { get; set; }
}
