namespace API.Models
{
    public class subDatosGrafica1
    {
        public double tiempo { get; set; }
        public string inicio { get; set; }
        public string fin { get; set; }
        public double penalizacion { get; set; }
        public IList<penalizacion> listaPensalizaciones { get; set; }
    }

    public class penalizacion{
        public bool inicio { get; set; }
        public bool fin { get; set; }
        public DateTime fecha { get; set; }
    }

}