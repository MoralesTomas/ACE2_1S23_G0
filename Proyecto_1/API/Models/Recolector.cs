namespace API.Models
{
    public class Recolector
    {
        public string nameUser { get; set; }    //para tomar el nombre del usuario
        public int numeroPomodoro { get; set; } //para almacenar cual pomodoro tenemos que filtrar... 1 - 4
        public int numeroDescanso { get; set; } //para almacenar cual descanso tenemos que filtrar... 1 - 4
        public string fecha1 { get; set; }  //para almacenar la fecha uno -> sirve para el filtrado o como fecha especifica.
        public string fecha2 { get; set; }  //para almacenar la fecha dos -> sirve para el filtrar por rango.
        public bool Filtrar_rango { get; set; }  //para validar si es filtrado directo o por intervalo. -> si es true filtra por rango // false filtra
        public int nuevoValPomodoro { get; set; }   //para tomar el nuevo valor del tiempo de trabajo
        public int nuevoValDescanso { get; set; }   //para tomar el nuevo valor del tiempo de descanso
        public int nuevoUserName { get; set; }      //para tomar el nuevo valor del nuevo nombre usuario.
    }
}