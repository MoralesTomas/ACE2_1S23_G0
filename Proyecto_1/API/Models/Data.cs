namespace API.Models
{
    public class Data
    {
        public Guid id { get; set; }
        public int ts { get; set; }     //referencia al tiempo estandar que dura cada pomodoro.
        public int ds { get; set; }      //referencia al tiempo estandar que dura cada descanso.
        public string fecha { get; set; }   //esta contendra fecha horas minutos y segundos
        public string fecha_corta { get; set; } //esta solo contendra la fecha sin valores de tiempo horario.
        public bool descansoLargo { get; set; }
        public bool descansoNormal { get; set; }
        public bool inicio { get; set; }
        public bool fin { get; set; }
        public int numeroPomodoro { get; set; }
        public int numeroDescanso { get; set; }
        public string dia { get; set; }
        public string mes { get; set; }
        public string anio { get; set; }
        public int hora { get; set; }
        public int minuto { get; set; }
        public int segundo { get; set; }
        public string userName { get; set; }
        public bool sentado { get; set; }
        public string codGrupo { get; set; }
    }
}