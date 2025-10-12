using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class TurnoPlantilla
{
    public uint Id { get; set; }

    public uint SalaId { get; set; }

    public uint PersonalId { get; set; }

    public sbyte DiaSemana { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public int DuracionMin { get; set; }

    public int Cupo { get; set; }

    public bool Activo { get; set; }   // 👈  <- asegurate que sea bool (no bool?)

    // 🔗 Relaciones
    public virtual Sala Sala { get; set; } = null!;

    public virtual Personal Personal { get; set; } = null!;
}