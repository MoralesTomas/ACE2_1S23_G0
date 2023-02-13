using Microsoft.EntityFrameworkCore;
using proyectoEF.Models;

namespace   proyectoEF.Contexto;

public class DataContext:DbContext
{

    
    public DbSet<Data> Datos { get; set; }

    public DataContext(DbContextOptions<DataContext> options) :base(options) { }

    protected override void  OnModelCreating(ModelBuilder modelBuilder){

        modelBuilder.Entity<Data>(Data =>{
            
            Data.ToTable("Data");

            Data.HasKey(dato => dato.Id);

            Data.Property(p => p.Fecha).IsRequired();

            Data.Property( p => p.Tipo).IsRequired();

            Data.Property(p => p.datos).IsRequired();

        });

    }
    
}
