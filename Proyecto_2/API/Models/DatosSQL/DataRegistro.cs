namespace API.Models
{
    public class DataRegistro
    {
        public Guid Id { get; set; }    //Id que identifica a cada registro.
        public int valorHumedad { get; set; }   //Valor que indica la humedad de la tierra.
        public int valorTemperatura { get; set; }   //Valor que indica la temperatura del medio ambiente
        public int porcentajeAguaDisponible { get; set; }   //Valor que indica el porcentaje de agua que tenemos.
        public int capacidadTanque { get; set; }   //Valor que indica que capacidad de agua tiene el tanque.
        public bool estadoRiego { get; set; }       //Valor que indica si el sistema esta en modo de riego o no. TRUE = se esta regando; FALSE = bomba apagada.
        public DateTime fechaComparadora { get; set; }    //Valor de la fecha para comparar con otras fechas
        public string fechaConsola { get; set; }    //Valor de la fecha en manera de un string en formato -> yyyy/mm/dd
        public int anio { get; set; }   //Valor del anio del registro
        public int mes { get; set; }    //Valor del mes del registro
        public int dia { get; set; }    //Valor del dia del registro.
        public TimeSpan horaCompleta { get; set; }  //Valor de la hora para poder comparar.
        public int hora { get; set; }   //Valor de la hora del registro.
        public int minuto { get; set; } //Valor del minuto del registro.
        
        
    }
}