using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class Comprobante
{
    public uint Id { get; set; }

    public uint OrdenId { get; set; }

    public string FileUrl { get; set; } = null!;

    public string? MimeType { get; set; }

    public DateTime SubidoEn { get; set; }

    public virtual OrdenPago OrdenPago { get; set; } = null!;
}
