using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using API.Models;
using API.Context;
using API.Utilidades;

// VARIABLES GLOBALES ============================================================================================================
#region  Variables globales para poder setear

double capacidadTanque = -1;

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

app.MapGet("/name", () =>
{
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
        valDefault.capacidadTanque = capacidadTanque;
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
app.MapGet("/verEstado", async ([FromServices] DataContext dbContext) =>
{
    return Results.Ok(dbContext.DatosAG);
});

#endregion Finaliza los endpoints de datos de tiempo real.


//==================ACTUALIZACION DE DATOS - AJUSTES GENERALES ======================================================================

#region Endpoint para modificar los ajustes como el tiempo de riego, actualizando el estado se debe poder encender la bomba o apagarla.

app.MapPut("/actualizarAjustes", async ([FromServices] DataContext dbContext, [FromBody] recolectorData recolector) =>
{
    try
    {

        //validando que los parametros numericos no sean valores negativos
        if (true)
        {
            if (recolector.valorHumedadExterna < 0)
            {
                return Results.BadRequest("El valor de la humedad externa no puede ser menor a cero");
            }
            if (recolector.valorHumedadInterna < 0)
            {
                return Results.BadRequest("El valor de la humedad interna no puede ser menor a cero");
            }

            if (recolector.valorTemperaturaExterna < 0)
            {
                return Results.BadRequest("El valor de la temperatura externa no puede ser menor a cero");
            }
            if (recolector.valorTemperaturaInterna < 0)
            {
                return Results.BadRequest("El valor de la temperatura interna no puede ser menor a cero");
            }

            if (recolector.porcentajeAguaDisponible < 0)
            {
                return Results.BadRequest("El valor del porcentaje de agua disponible no puede ser menor a cero");
            }

            if (recolector.capacidadTanque < 0)
            {
                return Results.BadRequest("El valor de la capacidad del tanque no puede ser menor a cero");
            }

            if (recolector.tiempoRiego < 0)
            {
                return Results.BadRequest("El valor del tiempo de riego no puede ser menor a cero");
            }


        }


        //Ahora buscar la data para editarla
        var parametros = dbContext.DatosAG.SingleOrDefault();

        parametros.valorHumedadExterna = recolector.valorHumedadExterna;
        parametros.valorHumedadInterna = recolector.valorHumedadInterna;

        parametros.valorTemperaturaExterna = recolector.valorTemperaturaExterna;
        parametros.valorTemperaturaInterna = recolector.valorTemperaturaInterna;

        parametros.porcentajeAguaDisponible = recolector.porcentajeAguaDisponible;

        parametros.estadoRiego = recolector.estadoRiego;

        parametros.capacidadTanque = recolector.capacidadTanque;

        parametros.tiempoRiego = recolector.tiempoRiego;

        await dbContext.SaveChangesAsync();

        return Results.Ok("Se actualizaron los ajustes generales de la aplicacion.");

    }
    catch (Exception a)
    {
        return Results.BadRequest("Algo salio mal intente nuevamente.." + a.Message);
    }
});

#endregion Terminan los endpoints para modificar los ajustes generales


//===================AGREGACION DE UN REGISTRO=======================================================================================

#region Endpoints para agregar un registro a la BD. Recordar que estos registros se hacen cada que se detecta un cambio en el estado de cualquier variable.

//AGRAGACION registro individual
app.MapPost("/agregarRegistro", async ([FromServices] DataContext dbContext, [FromBody] recolectorData recolector) =>
{
    try
    {


        //VALIDAR que los valores numericos no sean negativos
        if (true)
        {

            if (recolector.valorHumedadInterna < 0)
            {
                return Results.BadRequest("El valor de la humedad interna no puede ser menor a cero");
            }
            if (recolector.valorHumedadExterna < 0)
            {
                return Results.BadRequest("El valor de la humedad externa no puede ser menor a cero");
            }

            if (recolector.valorTemperaturaInterna < 0)
            {
                return Results.BadRequest("El valor de la temperatura interna no puede ser menor a cero");
            }
            if (recolector.valorTemperaturaExterna < 0)
            {
                return Results.BadRequest("El valor de la temperatura externa no puede ser menor a cero");
            }

            if (recolector.porcentajeAguaDisponible < 0)
            {
                return Results.BadRequest("El valor del porcentaje de agua disponible no puede ser menor a cero");
            }

            if (recolector.capacidadTanque < 0)
            {
                return Results.BadRequest("El valor de la capacidad de tanque no puede ser menor a cero");
            }

            if (recolector.tiempoRiego < 5)
            {
                return Results.BadRequest("El valor del tiempo de riego no puede ser menor a cinco");
            }
        }

        //CREACION de instancia de un registro.
        DataRegistro nuevoRegistro = new DataRegistro();

        //ASIGNACION de datos evaluados en el punto de arriba.
        if (true)
        {
            nuevoRegistro.valorHumedadInterna = recolector.valorHumedadInterna;
            nuevoRegistro.valorHumedadExterna = recolector.valorHumedadExterna;

            nuevoRegistro.valorTemperaturaInterna = recolector.valorTemperaturaInterna;
            nuevoRegistro.valorTemperaturaExterna = recolector.valorHumedadExterna;

            nuevoRegistro.capacidadTanque = recolector.capacidadTanque;

            nuevoRegistro.porcentajeAguaDisponible = recolector.porcentajeAguaDisponible;
            nuevoRegistro.tiempoRiego = recolector.tiempoRiego;
        }

        //ASIGNAR VALORES DE FECHA
        if (true)
        {

            if (recolector.fecha == "" || recolector.fecha == null)
            {

                //se crea una fecha tomando de referencia el dia de hoy.
                DateTime fecha = DateTime.Now;

                //y se agregan los valores especificos al registro
                nuevoRegistro.fechaComparadora = fecha;
                nuevoRegistro.fechaConsola = fecha.ToShortDateString().ToString();
                nuevoRegistro.anio = fecha.Year;
                nuevoRegistro.mes = fecha.Month;
                nuevoRegistro.dia = fecha.Day;
                nuevoRegistro.horaCompleta = fecha.TimeOfDay;
                nuevoRegistro.hora = nuevoRegistro.horaCompleta.Hours;
                nuevoRegistro.minuto = nuevoRegistro.horaCompleta.Minutes;

            }
            else
            {
                //se le asigna la fecha que se le ha enviado.
                conversor utilidad = new conversor();

                DateTime fecha = utilidad.stringToDateTime(recolector.fecha);
                //y se agregan los valores especificos al registro
                nuevoRegistro.fechaComparadora = fecha;
                nuevoRegistro.fechaConsola = fecha.ToShortDateString().ToString();
                nuevoRegistro.anio = fecha.Year;
                nuevoRegistro.mes = fecha.Month;
                nuevoRegistro.dia = fecha.Day;
                nuevoRegistro.horaCompleta = fecha.TimeOfDay;
                nuevoRegistro.hora = nuevoRegistro.horaCompleta.Hours;
                nuevoRegistro.minuto = nuevoRegistro.horaCompleta.Minutes;

            }


        }

        //ASIGNAR ESTADO DE RIEGO
        nuevoRegistro.estadoRiego = recolector.estadoRiego;

        //ahora a registrarlo en la BD.
        await dbContext.AddAsync(nuevoRegistro);
        await dbContext.SaveChangesAsync();


        return Results.Ok("Se realizo un nuevo registro.");

    }
    catch (Exception a)
    {
        return Results.BadRequest("\n\nAlgo salio mal al intentar agregar el registro intente nuevamente.." + a.Message);
    }
});

#endregion Fin endpoint para agregacion de registro.

//===================VISUALIZAR todos los registros =================================================================================

#region Endpoint para ver todos los datos de la tabla de registros.

// Endpoint que retorna todos los registros
app.MapGet("/verRegistros", async ([FromServices] DataContext dbContext) =>
{
    return Results.Ok(dbContext.Datos);
});


#endregion Termina el endpoint para visualizar registros.

//==================DATOS DE GRAFICAS EN TIEMPO REAL=================================================================================

app.Run();
