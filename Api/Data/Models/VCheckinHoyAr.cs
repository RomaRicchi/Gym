using System;

namespace Api.Data.Models;

public partial class VCheckinHoyAr
{
    public uint Id { get; set; }

    public uint SocioId { get; set; }

    public uint? TurnoPlantillaId { get; set; }

    public DateTime? FechaAr { get; set; }

    public string Origen { get; set; } = null!;
}
