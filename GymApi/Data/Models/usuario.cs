using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class usuario
{
    public uint id { get; set; }

    public string email { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    public string rol { get; set; } = null!;

    public string estado { get; set; } = null!;

    public DateTime creado_en { get; set; }
}
