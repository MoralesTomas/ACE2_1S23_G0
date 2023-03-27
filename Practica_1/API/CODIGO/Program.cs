using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectoEF.Contexto;
using proyectoEF.Models;
using EJEMPLO_API.Utilidades;
using System.Text.Json;

//variable con la direccion del pueto que se va a utilizar.
String puertoLocal = 5012;
String puertoConsumidor = 5500;


var builder = WebApplication.CreateBuilder(args);


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

//inyeccion de dependencia de sql
builder.Services.AddSqlServer<DataContext>(builder.Configuration.GetConnectionString("llave"));

var app = builder.Build();

//endpoint de cajon
app.MapGet("/", () => "Hello World!");

//endpoint que vamos a utilizar para crear la instancia de la BD, basta con hacerlo una vez en el equipo que utilizaremos de manera local.
app.MapGet("/CrearBD", async ([FromServices] DataContext dbContext) => 
{
    dbContext.Database.EnsureCreated();

    return Results.Ok("Ok");

});


//enpoint que nos retorna todos los datos que tengamos en la BD
app.MapGet("/api/datos", async ([FromServices] DataContext dbContext)=>
{
    return Results.Ok(dbContext.Datos);
});

app.MapGet("/api/datoultimo", async ([FromServices] DataContext dbContext)=>
{
    return Results.Ok(dbContext.Datos.OrderBy( e => e.Fecha).LastOrDefault());
});

//endpoint para insertar un nuevo dato (formato queda pendiente a edicion dependiendo el modelo.)
app.MapPost("/api/insertarDato", async ([FromServices] DataContext dbContext, [FromBody] Data nuevoDato)=>
{

    try{
        nuevoDato.Id = Guid.NewGuid();
        nuevoDato.Fecha = DateTime.Now.ToString();

        await dbContext.AddAsync(nuevoDato);
        
        await dbContext.SaveChangesAsync();

        return Results.Ok("Se inserto un nuevo dato");   

    }catch(Exception a){
        return Results.BadRequest("Algo salio mal intente nuevamente.."+a.Message);
    }
});

// Nuevo enpoint datos ordenados
app.MapGet("/api/datosordenados", async ([FromServices] DataContext dbContext)=>
{
    return Results.Ok(dbContext.Datos.OrderByDescending( p => p.Fecha));
});

//enpoint que nos retorna todos los datos que tengamos en la BD
app.MapGet("/api/datos2", async ([FromServices] DataContext dbContext)=>
{   
    IEnumerable<Data> listado = dbContext.Datos.OrderBy(e => e.Fecha);

    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = System.Text.Json.JsonSerializer.Serialize(listado, options);

    return Results.Ok(json);
});

//comenzando la logica para la escucha del arduino

//instancia de un objeto de la clase Serial para inicializar el uso de puertos.
// Serial puerto = new Serial();

// //asignando la ruta a consumir.
// puerto.localHost = localHost;
// puerto.puerto = nombrePuerto;
// //iniciando un nuevo hilo que haga uso del EscucharSerial de la clase Serial.
// Thread Hilo = new Thread( puerto.EscuchaSerail );

// //INICIANDO AMBAS TAREAS.
// Hilo.Start();
app.Run();

//cuando terminemos la ejecucion de la api entonces que cierre el puerto.
// puerto.isClosed = true;
// puerto.Port.Close();


/*
Formato temporal de datos recibidos de arduino:

    cadena,<entero>
    
*/
