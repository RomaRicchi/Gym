namespace Api.Data.Models
{
    public class VSuscripcionesAr
    {
        public int id { get; set; }
        public int socio_id { get; set; }
        public int plan_id { get; set; }
        public DateTime inicio_ar { get; set; }
        public DateTime? fin_ar { get; set; }
        public int estado { get; set; }

       
    }
}
