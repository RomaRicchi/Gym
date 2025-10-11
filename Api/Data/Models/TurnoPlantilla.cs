using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class TurnoPlantilla
{
    public uint Id { get; set; }

    public uint SalaId { get; set; }

    public uint? ProfesorId { get; set; }

    public sbyte DiaSemana { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public int DuracionMin { get; set; }

    public int Cupo { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Checkin> Checkins { get; set; } = new List<Checkin>();

    public virtual ICollection<OrdenTurno> OrdenesTurno { get; set; } = new List<OrdenTurno>();

    public virtual Profesor? Profesor { get; set; }

    public virtual Sala Sala { get; set; } = null!;

    public virtual ICollection<SuscripcionTurno> SuscripcionesTurno { get; set; } = new List<SuscripcionTurno>();
}
