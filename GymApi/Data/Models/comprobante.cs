using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;

public partial class comprobante
{
    public uint id { get; set; }

    public uint orden_id { get; set; }

    public string file_url { get; set; } = null!;

    public string? mime_type { get; set; }

    public DateTime subido_en { get; set; }

    public virtual orden_pago orden { get; set; } = null!;
}
