namespace API.Models
{
    public class DataRegistro
    {
        public Guid Id { get; set; }    //Id que identifica a cada registro.
        public double valorHumedadExterna { get; set; }  //Ultimo valor de humedad externa leida -> tiempo real
        public double valorHumedadInterna { get; set; }  //Ultimo valor de humedad interna leida -> tiempo real
        public double valorTemperaturaExterna { get; set; }   //Ultimo valor de temperatura externa leida -> tiempo real
        public double valorTemperaturaInterna { get; set; }   //Ultimo valor de temperatura interna leida -> tiempo real
        public double porcentajeAguaDisponible { get; set; }   //Valor que indica el porcentaje de agua que tenemos.
        public double capacidadTanque { get; set; }   //Valor que indica que capacidad de agua tiene el tanque.
        public bool estadoRiego { get; set; }       //Valor que indica si el sistema esta en modo de riego o no. TRUE = se esta regando; FALSE = bomba apagada.
        public int tiempoRiego { get; set; }
        public DateTime fechaComparadora { get; set; }    //Valor de la fecha para comparar con otras fechas
        public string fechaConsola { get; set; }    //Valor de la fecha en manera de un string en formato -> yyyy/mm/dd
        public int anio { get; set; }   //Valor del anio del registro
        public int mes { get; set; }    //Valor del mes del registro
        public int dia { get; set; }    //Valor del dia del registro.
        public TimeSpan horaCompleta { get; set; }  //Valor de la hora para poder comparar.
        public int hora { get; set; }   //Valor de la hora del registro.
        public int minuto { get; set; } //Valor del minuto del registro.
        public int segundo { get; set; } //Valor del segundo del registro.

    }
}