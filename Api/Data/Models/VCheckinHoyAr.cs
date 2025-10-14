using System;

namespace Api.Data.Models;

public partial class VCheckinHoyAr
{
    public int Id { get; set; }

    public int SocioId { get; set; }

    public int? TurnoPlantillaId { get; set; }

    public DateTime? FechaAr { get; set; }

    public string Origen { get; set; } = null!;
}
