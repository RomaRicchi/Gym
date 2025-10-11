using System;

namespace Api.Data.Models;

public partial class VOrdenesAr
{
    public uint Id { get; set; }

    public uint SocioId { get; set; }

    public uint PlanId { get; set; }

    public decimal Monto { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? VenceEnAr { get; set; }

    public DateTime CreadoEn { get; set; }
}
