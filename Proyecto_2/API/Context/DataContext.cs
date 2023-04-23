using Microsoft.EntityFrameworkCore;
using System.Text.Json;
//jalando los datos de los modelos
using API.Models;

namespace API.Context
{
    public class DataContext : DbContext
    {
    public DbSet<DataRegistro> Datos { get; set; } 
    public DbSet<DataConfigSis> DatosConfig { get; set; } 
    public DbSet<DataAG> DatosAG { get; set; } 

    public DataContext(DbContextOptions<DataContext> options) :base(options) { }

    protected override void  OnModelCreating( ModelBuilder modelBuilder ){

        modelBuilder.Entity<DataRegistro>( Data =>{
            
            Data.ToTable( "DataRegistros" ); //Asignando nombre a la tabla

            Data.HasKey( dato => dato.Id );
            Data.Property( p => p.valorHumedad ).IsRequired();
            Data.Property( p => p.valorTemperatura ).IsRequired();
            Data.Property( p => p.porcentajeAguaDisponible ).IsRequired();
            Data.Property( p => p.capacidadTanque ).IsRequired();
            Data.Property( p => p.estadoRiego ).IsRequired();
            Data.Property( p => p.fechaComparadora ).IsRequired();
            Data.Property( p => p.fechaConsola ).IsRequired();
            Data.Property( p => p.anio ).IsRequired();
            Data.Property( p => p.mes ).IsRequired();
            Data.Property( p => p.dia ).IsRequired();
            Data.Property( p => p.horaCompleta ).IsRequired();
            Data.Property( p => p.hora ).IsRequired();
            Data.Property( p => p.minuto ).IsRequired();
        }
        );

        //Definiendo la entidad de DataConfigSis
        modelBuilder.Entity<DataConfigSis>( data =>{
            
            data.ToTable("DataConfigSis"); 

            data.HasKey( dato => dato.Id );
            data.Property( p => p.Lunes ).IsRequired();
            // data.Property( p => p.Martes ).IsRequired();
            // data.Property( p => p.Miercoles ).IsRequired();
            // data.Property( p => p.Jueves ).IsRequired();
            // data.Property( p => p.Viernes ).IsRequired();
            // data.Property( p => p.Sabado ).IsRequired();
            // data.Property( p => p.Domingo ).IsRequired();

        });

        //Definiendo la entidad de ConfigAG -> ajustes generales
        modelBuilder.Entity<DataAG>(data =>{

            data.ToTable("DataConfigSis"); 

            data.HasKey( dato => dato.Id );
            data.Property( p => p.valorHumedad ).IsRequired();
            data.Property( p => p.valorTemperatura ).IsRequired();
            data.Property( p => p.porcentajeAguaDisponible ).IsRequired();
            data.Property( p => p.estadoRiego ).IsRequired();
            data.Property( p => p.capacidadTanque ).IsRequired();
        }
        );
    }
    }
}


 /*
     DbSet es una clase que actúa como una puerta de acceso a una tabla específica en la BD.  
     DbSet está asociado con una tabla específica en la base de datos y permite acceder a los datos de esa tabla
*/