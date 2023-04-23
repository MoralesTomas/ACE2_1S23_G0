using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using API.Models;
using API.Context;

#region  Variables globales para poder setear

int capacidadTanque = -1;

#endregion Termina seteo de variables globales


// Creando un constructor (builder) para una aplicación web. Se utilizará para agregar configuraciones y servicios a la aplicación
var builder = WebApplication.CreateBuilder(args);


#region Inyeccion de dependencias.
//inyeccion de dependencia de sql
builder.Services.AddSqlServer<DataContext>(builder.Configuration.GetConnectionString("llave"));

//Para que el json sea legible
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

#endregion termina inyeccion de dependencias


var app = builder.Build();

app.UseCors("AllowAll");

//=====================SALIDAS TEST==================================================================================================

#region Salidas de test, sin importancia...

app.MapGet("/name", () => {
    return Results.Ok("Todo bien :D");
});



#endregion Termina salidas de test...


//==================BASE DE DATOS / Creacion y Eliminacion ==========================================================================


#region Enpoints que interactuan en creacion y eliminacion de la BD

// Endpoint que vamos a utilizar para crear la instancia de la BD, basta con hacerlo una vez en el equipo que utilizaremos de manera local.
app.MapGet("/crearBD", async ([FromServices] DataContext dbContext) =>
{
    dbContext.Database.EnsureCreated();

    //Validar que existan datos en la tabla de ajustes generales.
    var recolectado = dbContext.DatosAG.Count();

    if (recolectado == 0)
    {
        //si no existe entonces se crea una instancia de data de ajustes generales por defecto
        DataAG valDefault = new DataAG();
        valDefault.Id = Guid.NewGuid();
        
        //Valores negativos al crear primera instancia
        valDefault.valorHumedadInterna = -1;   
        valDefault.valorHumedadExterna = -1;

        valDefault.valorTemperaturaInterna = -1;
        valDefault.valorTemperaturaExterna = -1;
        
        valDefault.porcentajeAguaDisponible = -1;
        valDefault.estadoRiego = false;
        valDefault.capacidadTanque = -1;
        valDefault.tiempoRiego = 0;

        //Agregar la data y guardarla
        await dbContext.AddAsync(valDefault);
        await dbContext.SaveChangesAsync();
    }

    return Results.Ok("Se creo la BD en tu sistema.");

});

// Endpoint para eliminar la BD del sistema.
app.MapGet("/eliminarBD", async ([FromServices] DataContext dbContext) =>
{
    dbContext.Database.EnsureDeleted();

    return Results.Ok("Se elimino la BD en tu sistema.");

});

#endregion Termina la definicion de endpoints que interactuan con la creacion y la eliminacion de la BD


//==================DATOS TIEMPO REAL================================================================================================

#region  Endpoints que nos sirven para poder obtener los datos en tiempo real.

// Endpoint que retorna los datos de la configuracion del sistema
app.MapGet("/verConfig", async ([FromServices] DataContext dbContext) =>
{
    return Results.Ok(dbContext.DatosAG);
});

#endregion Finaliza los endpoints de datos de tiempo real.


app.Run();
