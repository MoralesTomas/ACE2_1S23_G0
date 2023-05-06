namespace API.Models
{
    public class datosG1
    {
        public string fecha { get; set; }
        public DateTime fecha_comparadora { get; set; }
        public string fecha_corta { get; set; }
        public string codigoGrupo { get; set; }
        public string dia { get; set; }
        public string mes { get; set; }
        public int tiempoStandar { get; set; }
        public int descansoStandar { get; set; }
        public string usuario { get; set; }
        public subDatosGrafica1 P1 { get; set; }
        public subDatosGrafica1 D1 { get; set; }
        public subDatosGrafica1 P2 { get; set; }
        public subDatosGrafica1 D2 { get; set; }
        public subDatosGrafica1 P3 { get; set; }
        public subDatosGrafica1 D3 { get; set; }
        public subDatosGrafica1 P4 { get; set; }
        public subDatosGrafica1 D4 { get; set; }
    }
}