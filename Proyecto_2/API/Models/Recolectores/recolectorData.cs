namespace API.Models
{
    public class recolectorData
     {
        public double valorHumedadExterna { get; set; }  //Ultimo valor de humedad externa leida -> tiempo real
        public double valorHumedadInterna { get; set; }  //Ultimo valor de humedad interna leida -> tiempo real
        public double valorTemperaturaExterna { get; set; }   //Ultimo valor de temperatura externa leida -> tiempo real
        public double valorTemperaturaInterna { get; set; }   //Ultimo valor de temperatura interna leida -> tiempo real
        public double porcentajeAguaDisponible { get; set; } //Ultimo valor de agua disponible leido -> tiempo real
        public bool estadoRiego { get; set; }   //Ultimo valor del estado del sistema leido -> tiempo real.
        public double capacidadTanque { get; set; }    //Ultimo valor de la capacidad de nuestro tanque de agua leido -> tiempo real.
        public int tiempoRiego { get; set; }    //Indica el valor del tiempo que debe durar el riego. Este tiempo es en segundos
        public string fecha { get; set; }   //Puede o no venir dependiendo si se utiliza para test de filtrado.
    }
}