using System.Collections.Generic;
namespace API.Models
{

    public class datosGFJson{
        public IList<datosGF> data { get; set; }
    }

    public class datosGF
    {
        public string fecha_corta { get; set; }
        public string fecha { get; set; }
        public DateTime fecha_comparadora { get; set; }
        public IList<elementoPrimario> trabajo { get; set; }
        public IList<elementoPrimario> descansos { get; set; }
        
    }

    public class elementoPrimario{
        public IList<elementoInterno> dataPrimaria { get; set; }
    }

    //estos elementosInternos seran los elementos internos de los dos grupos secundarios
    public class elementoInterno{
        public string nameElemento { get; set; }
        public IList<claveValor> arreglo { get; set; }

        /*
        EJEMPLO:

            "descanso 2 ": [
                            {
                                "12:00": 0
                            },
                            {
                                "12:01": 1
                            }
                        ]
        EJEMPLO 2:

        "Pomodoro 1 ": [
                        {
                            "12:00": 1
                        },
                        {
                            "12:01": 0
                        }
                    ]
        */
    }

    //esto simulara la clave y valor del tipo:
    public class claveValor{
        public string clave { get; set; }
        public int valor { get; set; }

        /*
        {
            "12:01": 0
        }
        */

    }
}