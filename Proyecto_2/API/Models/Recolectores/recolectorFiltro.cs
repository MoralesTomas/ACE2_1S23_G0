namespace API.Models
{
    public class recolectorFiltro
    {
        public string fecha { get; set; }
        public string fecha1 { get; set; }
        public string fecha2 { get; set; }
        public DateTime fechaComparadora1 { get; set; }
        public DateTime fechaComparadora2 { get; set; }
        public int hora_1 { get; set; }   
        public int minuto_1 { get; set; }   
        public int segundo_1 { get; set; }   
        public int hora_2 { get; set; }   
        public int minuto_2 { get; set; }   
        public int segundo_2 { get; set; } 
        public int dia { get; set; }
        public int mes { get; set; }
        public int anio { get; set; }
    }
}