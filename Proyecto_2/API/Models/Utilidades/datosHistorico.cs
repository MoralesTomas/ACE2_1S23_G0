namespace API.Models
{
    public class datosHistorico
    {
        public string fechaCorta { get; set; }  //Fecha que se pueda visualizar de manera normal
        public DateTime fecha { get; set; } //Fecha para realizar las comparaciones como tal.
        public List<TimeSpan> horario { get; set; }
        public double valorHumedadExterna { get; set; }  //Ultimo valor de humedad externa leida -> tiempo real
        public double valorHumedadInterna { get; set; }  //Ultimo valor de humedad interna leida -> tiempo real
        public double valorTemperaturaExterna { get; set; }   //Ultimo valor de temperatura externa leida -> tiempo real
        public double valorTemperaturaInterna { get; set; }   //Ultimo valor de temperatura interna leida -> tiempo real
        public double porcentajeAguaDisponible { get; set; }   //Valor que indica el porcentaje de agua que tenemos.
        public double capacidadTanque { get; set; }   //Valor que indica que capacidad de agua tiene el tanque.
        public bool estadoRiego { get; set; }       //Valor que indica si el sistema esta en modo de riego o no. TRUE = se esta regando; FALSE = bomba apagada.
        public int tiempoRiego { get; set; }
    }
}