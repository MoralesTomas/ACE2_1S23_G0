namespace API.Models
{
    public class HorarioRiego
    {
        public IList<Horario> Lunes { get; set; }
        public IList<Horario> Martes { get; set;}
        public IList<Horario> Miercoles { get; set;}
        public IList<Horario> Jueves { get; set;}
        public IList<Horario> Viernes { get; set;}
        public IList<Horario> Sabado { get; set;}
        public IList<Horario> Domingo { get; set;}
    }
}