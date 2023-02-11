using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectoEF.Models;


public class Data
{

    public Guid Id { get; set; }

    public String Tipo { get; set; }

    public String Fecha { get; set; }

    public String Datos { get; set; }
    
}
