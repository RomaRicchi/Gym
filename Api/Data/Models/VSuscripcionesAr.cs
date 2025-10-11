using System;

namespace Api.Data.Models;

public partial class VSuscripcionesAr
{
    public uint Id { get; set; }

    public uint SocioId { get; set; }

    public uint PlanId { get; set; }

    public DateTime? InicioAr { get; set; }

    public DateTime? FinAr { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime CreadoEn { get; set; }
}
