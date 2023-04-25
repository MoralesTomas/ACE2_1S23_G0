namespace API.Models
{
    public class recolectorFiltro
    {
        public string fecha1 { get; set; }
        public string fecha2 { get; set; }
        public DateTime fechaComparadora1 { get; set; }
        public DateTime fechaComparadora2 { get; set; }
        public int hora { get; set; }   
        public int minuto { get; set; }   
        public int segundo { get; set; }   
        public int dia { get; set; }
        public int mes { get; set; }
        public int anio { get; set; }
    }
}