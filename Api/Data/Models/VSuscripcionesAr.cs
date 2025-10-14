using System;

namespace Api.Data.Models;

public partial class VSuscripcionesAr
{
    public int Id { get; set; }

    public int SocioId { get; set; }

    public int PlanId { get; set; }

    public DateTime? InicioAr { get; set; }

    public DateTime? FinAr { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime CreadoEn { get; set; }
}
