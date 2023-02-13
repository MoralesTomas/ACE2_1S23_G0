using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectoEF.Models;


public class Data
{

    public Guid Id { get; set; }

    public String Fecha { get; set; }

    public double Calor { get; set; }

    public double HumedadRelativa { get; set; }

    public double HumedadAbsoluta { get; set; }
    
    public double Velocidad { get; set; }

    public string Direccion { get; set; }

    public string Presion { get; set; }
    
}
