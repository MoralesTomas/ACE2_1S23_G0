namespace API.Models
{
    public class DataConfigSis
    {
        public Guid Id { get; set; }
        public IEnumerable<Horario>? Lunes { get; set; }   //Horario de riego del dia LUNES
        // public Horario[] Martes { get; set;}   //Horario de riego del dia MARTES
        // public Horario[] Miercoles { get; set;}    //Horario de riego del dia MIERCOLES
        // public Horario[] Jueves { get; set;}   //Horario de riego del dia JUEVES
        // public Horario[] Viernes { get; set;}  //Horario de riego del dia VIERNES
        // public Horario[] Sabado { get; set;}   //Horario de riego del dia SABADO
        // public Horario[] Domingo { get; set;}  //Horario de riego del dia DOMINGO
    }
}