namespace API.Models
{
    public class datosFiltrado
    {
        public string fechaCorta { get; set; }  //Fecha que se pueda visualizar de manera normal
        public DateTime fecha { get; set; } //Fecha para realizar las comparaciones como tal.
        public List<TimeSpan> horario { get; set; }
    }

}