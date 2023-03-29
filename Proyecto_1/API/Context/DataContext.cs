using Microsoft.EntityFrameworkCore;

//jalando los datos de los modelos
using API.Models;

namespace API.Context
{
    public class DataContext : DbContext
    {
    public DbSet<Data> Datos { get; set; }

    public DbSet<ParamApp> Parametros { get; set; }
    public DbSet<ParamAppPorGrupo> ParametrosActuales { get; set; }

    public DataContext(DbContextOptions<DataContext> options) :base(options) { }

    protected override void  OnModelCreating(ModelBuilder modelBuilder){

        modelBuilder.Entity<Data>(Data =>{
            
            Data.ToTable("Data");

            Data.HasKey(dato => dato.id);
            Data.Property(p => p.fecha).IsRequired();
            Data.Property( p => p.descansoLargo).IsRequired();
            Data.Property(p => p.descansoNormal).IsRequired();
            Data.Property(p => p.inicio).IsRequired();
            Data.Property(p => p.fin).IsRequired();
            Data.Property(p => p.numeroPomodoro).IsRequired();
            Data.Property(p => p.dia).IsRequired();
            Data.Property(p => p.mes).IsRequired();
            Data.Property(p => p.hora).IsRequired();
            Data.Property(p => p.minuto).IsRequired();
            Data.Property(p => p.segundo).IsRequired();
            Data.Property(p => p.userName).IsRequired();
        }
        );

        modelBuilder.Entity<ParamApp>( p =>{
            
            p.ToTable("Parametros");
            p.Property(p => p.userName).IsRequired();
            p.Property( p => p.valPomodoro).IsRequired();
            p.Property(p => p.valDescanso).IsRequired();

        });

        modelBuilder.Entity<ParamAppPorGrupo>( p =>{
            
            p.ToTable("ParametrosActuales");
            p.Property(p => p.userName).IsRequired();
            p.Property( p => p.valPomodoro).IsRequired();
            p.Property(p => p.valDescanso).IsRequired();

        });

    }
    }
}