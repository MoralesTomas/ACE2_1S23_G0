namespace API.Models
{
    public class DataAG //Data de ajustes generales
    {
        public Guid Id { get; set; }
        public int valorHumedad { get; set; }  //Ultimo valor de humedad leido -> tiempo real
        public int valorTemperatura { get; set; }   //Ultimo valor de temperatura leido -> tiempo real
        public int porcentajeAguaDisponible { get; set; } //Ultimo valor de agua disponible leido -> tiempo real
        public bool estadoRiego { get; set; }   //Ultimo valor del estado del sistema leido -> tiempo real.
        public int capacidadTanque { get; set; }    //Ultimo valor de la capacidad de nuestro tanque de agua leido -> tiempo real.
        
    }
}