using System;

namespace Api.Data.Models
{
    /// <summary>
    /// Vista que muestra la cantidad de cupos reservados por turno (v_cupo_reservado)
    /// </summary>
    public class VCupoReservado
    {
        public int turno_id { get; set; }
        public int reservados { get; set; }
        public DateTime fecha { get; set; }
    }
}
