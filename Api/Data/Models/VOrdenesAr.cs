using System;

namespace Api.Data.Models;

public partial class VOrdenesAr
{
    public int Id { get; set; }

    public int SocioId { get; set; }

    public int PlanId { get; set; }

    public decimal Monto { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? VenceEnAr { get; set; }

    public DateTime CreadoEn { get; set; }
}
