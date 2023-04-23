namespace API.Models
{
    public class recolectorData
     {
        public int valorHumedadExterna { get; set; }  //Ultimo valor de humedad externa leida -> tiempo real
        public int valorHumedadInterna { get; set; }  //Ultimo valor de humedad interna leida -> tiempo real
        public int valorTemperaturaExterna { get; set; }   //Ultimo valor de temperatura externa leida -> tiempo real
        public int valorTemperaturaInterna { get; set; }   //Ultimo valor de temperatura interna leida -> tiempo real
        public int porcentajeAguaDisponible { get; set; } //Ultimo valor de agua disponible leido -> tiempo real
        public bool estadoRiego { get; set; }   //Ultimo valor del estado del sistema leido -> tiempo real.
        public int capacidadTanque { get; set; }    //Ultimo valor de la capacidad de nuestro tanque de agua leido -> tiempo real.
        public int tiempoRiego { get; set; }    //Indica el valor del tiempo que debe durar el riego. Este tiempo es en segundos
    }
}