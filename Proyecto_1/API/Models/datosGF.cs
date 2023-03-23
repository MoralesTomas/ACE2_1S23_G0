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
    }

    public class subDatosGF{
        public Dictionary<string, subsubDatoGF> data { get; set; }
    }

    public class subsubDatoGF{
        public Dictionary<string, int> diccionario { get; set; }
    }
}