using System;
using System.Collections.Generic;

namespace Api.Data.Models
{
    public partial class Checkin
    {
        public int Id { get; set; }

        public int SocioId { get; set; }

        public int? TurnoPlantillaId { get; set; }

        public DateTime FechaHora { get; set; }

        public string Origen { get; set; } = null!;

        public virtual Socio Socio { get; set; } = null!;

        public virtual TurnoPlantilla? TurnoPlantilla { get; set; }
    }
}