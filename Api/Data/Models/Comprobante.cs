using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class Comprobante
{
    public uint Id { get; set; }

    public uint Orden_id { get; set; }

    public string File_url { get; set; } = null!;

    public string? Mime_type { get; set; }

    public DateTime Subido_en { get; set; }

    public virtual orden_pago Orden_pago { get; set; } = null!;
}
