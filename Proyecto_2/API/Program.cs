using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using API.Models;
using API.Context;
using API.Utilidades;
using Swashbuckle.AspNetCore.Swagger;

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

//SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion termina inyeccion de dependencias


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

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


// Endpoint que retorna los datos de la configuracion del sistema
app.MapGet("/verEstadoArduino", async ([FromServices] DataContext dbContext) =>
{
    DataAG respuesta = dbContext.DatosAG.FirstOrDefault();

    respArduino result = new respArduino();
    result.estadoRiego = respuesta.estadoRiego;
    result.tiempoRiego = respuesta.tiempoRiego;

    return Results.Ok(result);
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


        if( true ){
             //por si se bugea y es mayor al 100 %
            if (recolector.porcentajeAguaDisponible > 100){
                recolector.porcentajeAguaDisponible = 100;
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
                nuevoRegistro.segundo = nuevoRegistro.horaCompleta.Seconds;

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
                nuevoRegistro.segundo = nuevoRegistro.horaCompleta.Seconds;

            }


        }

        //ASIGNAR ESTADO DE RIEGO
        nuevoRegistro.estadoRiego = recolector.estadoRiego;

        //ahora a registrarlo en la BD.
        await dbContext.AddAsync(nuevoRegistro);
        await dbContext.SaveChangesAsync();

        //VALUAR SI EXISTEN CAMBIOS PARA APLICARLOS A LOS AJUSTES GENERALES
        if (true)
        {
            try
            {
                //Ahora buscar la data para validar posibles cambios.
                var parametros = dbContext.DatosAG.SingleOrDefault();   // Esto no puede ser null
                bool aplicarCambios = false;

                if (parametros.valorHumedadExterna != nuevoRegistro.valorHumedadExterna)
                {
                    parametros.valorHumedadExterna = nuevoRegistro.valorHumedadExterna;
                    aplicarCambios = true;
                }
                if (parametros.valorHumedadInterna != nuevoRegistro.valorHumedadInterna)
                {
                    parametros.valorHumedadInterna = nuevoRegistro.valorHumedadInterna;
                    aplicarCambios = true;
                }

                if (parametros.valorTemperaturaExterna != nuevoRegistro.valorTemperaturaExterna)
                {
                    parametros.valorTemperaturaExterna = nuevoRegistro.valorTemperaturaExterna;
                    aplicarCambios = true;
                }
                if (parametros.valorTemperaturaInterna != nuevoRegistro.valorTemperaturaInterna)
                {
                    parametros.valorTemperaturaInterna = nuevoRegistro.valorTemperaturaInterna;
                    aplicarCambios = true;
                }

                if (parametros.porcentajeAguaDisponible != nuevoRegistro.porcentajeAguaDisponible)
                {
                    parametros.porcentajeAguaDisponible = nuevoRegistro.porcentajeAguaDisponible;
                    aplicarCambios = true;
                }

                if (parametros.estadoRiego != nuevoRegistro.estadoRiego)
                {
                    parametros.estadoRiego = nuevoRegistro.estadoRiego;
                    aplicarCambios = true;
                }

                if (parametros.capacidadTanque != nuevoRegistro.capacidadTanque)
                {
                    parametros.capacidadTanque = nuevoRegistro.capacidadTanque;
                    aplicarCambios = true;
                }

                if (parametros.tiempoRiego != nuevoRegistro.tiempoRiego)
                {
                    parametros.tiempoRiego = nuevoRegistro.tiempoRiego;
                    aplicarCambios = true;
                }

                if (aplicarCambios)
                {
                    await dbContext.SaveChangesAsync();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Ocurrio un error al realizar validaciones en el endpoint '/agregarRegistro' ");

            }
        }

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

// Endpoint que retorna todos los registros de manera descendente, teniendo como primer dato el ultimo registro
app.MapGet("/verRegistros", async ([FromServices] DataContext dbContext) =>
{

    IEnumerable<DataRegistro> result = dbContext.Datos.OrderByDescending(e => e.fechaComparadora);

    return Results.Ok(result);
});


#endregion Termina el endpoint para visualizar registros.

//==================DATOS DE GRAFICAS A LO LARGO DEL TIEMPO=================================================================================

#region Enpoint para poder visualizar los datos a lo largo del tiempo

// PARA TEMPERATURA ---> debe tener horario.
app.MapPost("/datosHistoricos", async ([FromServices] DataContext dbContext, [FromBody] recolectorFiltro recolector) =>
{

    conversor util = new conversor();
    DateTime limiteInferior = recolector.fechaComparadora1;
    DateTime limiteSuperior = recolector.fechaComparadora2;

    //obtenemos las fechas que esten dentro del rango
    IEnumerable<DataRegistro> dataFiltrada = dbContext.Datos.Where(e => e.fechaComparadora >= limiteInferior && e.fechaComparadora <= limiteSuperior);

    IEnumerable<IGrouping<string, DataRegistro>> dataAgrupada = dataFiltrada.GroupBy(e => e.fechaConsola);

    List<datosHistorico> respuesta = new List<datosHistorico>();

    foreach (var grupoDia in dataAgrupada)
    {
        DataRegistro inferior = null;
        DataRegistro superior = null;

        foreach (var data in grupoDia)
        {

            if (inferior == null)
            {
                inferior = data;
                continue;
            }

            if (superior == null)
            {
                superior = data;

                //calculando los tiempos entre inferior y superior
                TimeSpan intervaloInterno = superior.fechaComparadora - inferior.fechaComparadora;

                datosHistorico temporalInterno = new datosHistorico();

                //Asignacion de los estados del registro
                if (true)
                {
                    temporalInterno.fecha = data.fechaComparadora;
                    temporalInterno.fechaCorta = data.fechaConsola;

                    temporalInterno.valorHumedadExterna = data.valorHumedadExterna;
                    temporalInterno.valorHumedadInterna = data.valorHumedadInterna;

                    temporalInterno.valorTemperaturaExterna = data.valorTemperaturaExterna;
                    temporalInterno.valorTemperaturaInterna = data.valorTemperaturaInterna;

                    temporalInterno.porcentajeAguaDisponible = data.porcentajeAguaDisponible;

                    temporalInterno.capacidadTanque = data.capacidadTanque;
                    temporalInterno.estadoRiego = data.estadoRiego;
                    temporalInterno.tiempoRiego = data.tiempoRiego;
                }


                temporalInterno.horario = new List<TimeSpan>();

                for (int i = 0; i < intervaloInterno.TotalSeconds; i++)
                {
                    inferior.fechaComparadora = inferior.fechaComparadora.AddSeconds(1);
                    var nuevoValor = inferior.fechaComparadora.TimeOfDay;
                    temporalInterno.horario.Add(nuevoValor);
                }
                //AGREGACION
                respuesta.Add(temporalInterno);

                continue;
            }

            //si llega aca entonces hay que cambiar el valor del inferior y el nuevo valor leido es el superior
            inferior = superior;
            superior = data;

            //calculando los tiempos entre inferior y superior
            TimeSpan intervalo = superior.fechaComparadora - inferior.fechaComparadora;

            datosHistorico temporal = new datosHistorico();

            //Asignacion de los estados del registro
            if (true)
            {
                temporal.fecha = data.fechaComparadora;
                temporal.fechaCorta = data.fechaConsola;

                temporal.valorHumedadExterna = data.valorHumedadExterna;
                temporal.valorHumedadInterna = data.valorHumedadInterna;

                temporal.valorTemperaturaExterna = data.valorTemperaturaExterna;
                temporal.valorTemperaturaInterna = data.valorTemperaturaInterna;

                temporal.porcentajeAguaDisponible = data.porcentajeAguaDisponible;

                temporal.capacidadTanque = data.capacidadTanque;
                temporal.estadoRiego = data.estadoRiego;
                temporal.tiempoRiego = data.tiempoRiego;
            }

            temporal.horario = new List<TimeSpan>();


            for (int i = 0; i < intervalo.TotalSeconds; i++)
            {
                inferior.fechaComparadora = inferior.fechaComparadora.AddSeconds(1);
                var nuevoValor = inferior.fechaComparadora.TimeOfDay;
                temporal.horario.Add(nuevoValor);
            }

            //AGREGACION
            respuesta.Add(temporal);

        }

    }

    return Results.Ok(respuesta);

});

#endregion Terminan los endpoint.


//===================UTILIDADES DE FILTRADO==================================================================================================

#region Endpoint para poder obtener la data para el filtrado de las graficas a lo largo del timepo.

app.MapGet("/fechasDisponibles", async ([FromServices] DataContext dbContext) =>
{

    IList<string> result = dbContext.Datos.OrderByDescending(e => e.fechaComparadora).Select(e => new string(e.fechaConsola)).ToList();

    return Results.Ok(result.Distinct());
});

#endregion


//===================OBTENER HORARIOS DE UN DIA ESPECIFICO====================================================================================

//Este endpoint retornara los horarios que existen en una fecha especifica.
app.MapPost("/horariosFecha", async ([FromServices] DataContext dbContext, [FromBody] recolectorFiltro recolector) =>
{


    //Primero buscar si existe dicha fecha.
    IList<DataRegistro> registros = dbContext.Datos.Where(e => e.dia == recolector.dia && e.mes == recolector.mes && e.anio == recolector.anio).ToList();

    //Validar si la cantidad de registros es nula retornarla de una
    if (registros.Count() == 0)
    {

        //se retornara un arreglo vacio lo cual indicara que no existe ningun registro de este dia en especifico.
        return Results.Ok(registros);

    }

    //Si existe un registro entonces aca es donde tomamos los horarios
    IList<TimeSpan> horarios = registros.Select(e => e.horaCompleta).Distinct().ToList();

    return Results.Ok(horarios);

});


//===================OBTENER DIA Y HORARIOS==================================================================================================

#region  Endpoint que retorna los dias y los horarios de cada dia.

app.MapGet("/datosSegmentados", async ([FromServices] DataContext dbContext) =>
{

    IList<DataRegistro> datosOrdenados = dbContext.Datos.OrderByDescending(e => e.fechaComparadora).ToList();

    IList<IGrouping<string, DataRegistro>> dataAgrupada = datosOrdenados.GroupBy(e => e.fechaConsola).ToList();

    IList<datosFiltrado> respuesta = new List<datosFiltrado>();

    foreach (var data in dataAgrupada)
    {
        datosFiltrado temporal = new datosFiltrado();
        temporal.horario = new List<TimeSpan>();

        bool asignar = true;

        IEnumerable<DataRegistro> horariosOrdenados = data.OrderBy(e => e.fechaComparadora);

        foreach (var registro in horariosOrdenados)
        {
            if (asignar)
            {
                temporal.fecha = registro.fechaComparadora;
                temporal.fechaCorta = registro.fechaConsola;
                asignar = false;
            }

            temporal.horario.Add(registro.horaCompleta);
        }
        respuesta.Add(temporal);
    }

    return Results.Ok(respuesta);

});

#endregion

//======================DUMMY===============================================
//Este endpoint retornara los horarios que existen en una fecha especifica.
app.MapPost("/filtradoOficial", async ([FromServices] DataContext dbContext, [FromBody] recolectorFiltro recolector) =>
{

    TimeSpan horario1 = new TimeSpan(recolector.hora_1, recolector.minuto_1, recolector.segundo_1);
    TimeSpan horario2 = new TimeSpan(recolector.hora_2, recolector.minuto_2, recolector.segundo_2);

    Console.WriteLine($"horario 1 -> {horario1}");
    Console.WriteLine($"horario 2 -> {horario2}");

    if (horario2 < horario1)
    {
        return Results.BadRequest("el horario uno no puede ser mayor al horario 2....");
    }


    DataRegistro inferior = null;
    DataRegistro superior = null;

    IEnumerable<DataRegistro> registros = dbContext.Datos.Where(e => e.fechaConsola.Equals(recolector.fecha));

    Console.WriteLine($"count de registros -> {registros.Count()}");


    IEnumerable<DataRegistro> list_inferior = registros.OrderBy(e => e.fechaComparadora).Where(e => e.horaCompleta <= horario1);
    IEnumerable<DataRegistro> list_superior = registros.OrderBy(e => e.fechaComparadora).Where(e => e.horaCompleta >= horario2);

    List<datosHistorico> respuesta = new List<datosHistorico>();    //aca iran los datos segmentos de segundo en segundo

    if (list_inferior.Count() != 0)
    {
        Console.WriteLine("encontramos un inferior");
        inferior = list_inferior.Last();
    }

    if (list_superior.Count() != 0)
    {
        Console.WriteLine("encontramos un superior");
        superior = list_superior.First();
    }

    //CASOS 

    //No hay datos en este intervalo.
    if (inferior == null && superior == null)
    {
        TimeSpan intervalo = horario2 - horario1;

        datosHistorico temp1 = new datosHistorico();

        //asignando valores zero default
        if (true)
        {
            temp1.valorHumedadExterna = 0;
            temp1.valorHumedadInterna = 0;
            temp1.valorTemperaturaExterna = 0;
            temp1.valorTemperaturaInterna = 0;
            temp1.porcentajeAguaDisponible = 0;
            temp1.capacidadTanque = 0;
            temp1.estadoRiego = false;
            temp1.tiempoRiego = 0;
        }

        temp1.horario = new List<TimeSpan>();

        temp1.horario.Add(horario1);    //agregacion del inicio del horario_1
        for (int i = 0; i < intervalo.TotalSeconds; i++)
        {
            horario1 = horario1.Add(new TimeSpan(0, 0, 1));
            temp1.horario.Add(horario1);
        }
        //AGREGACION de los datos nulos.
        respuesta.Add(temp1);
    }

    //Solo existe superior
    if (inferior == null && superior != null)
    {

        TimeSpan intervalo = horario2 - horario1;

        datosHistorico temp1 = new datosHistorico();

        //asignando valores zero default
        if (true)
        {
            temp1.valorHumedadExterna = 0;
            temp1.valorHumedadInterna = 0;
            temp1.valorTemperaturaExterna = 0;
            temp1.valorTemperaturaInterna = 0;
            temp1.porcentajeAguaDisponible = 0;
            temp1.capacidadTanque = 0;
            temp1.estadoRiego = false;
            temp1.tiempoRiego = 0;
        }

        temp1.horario = new List<TimeSpan>();

        temp1.horario.Add(horario1);    //agregacion del inicio del horario_1

        for (int i = 0; i < intervalo.TotalSeconds - 1 ; i++)
        {
            horario1 = horario1.Add(new TimeSpan(0, 0, 1));
            temp1.horario.Add(horario1);
        }
        //AGREGACION de los datos nulos.
        respuesta.Add(temp1);

        //validar que el superior no execeda lo que estamos buscando.
        if (superior.horaCompleta <= horario2)
        {

            datosHistorico temp2 = new datosHistorico();
            temp2.horario = new List<TimeSpan>();
            temp2.horario.Add(horario2);

            //agregacion de valores de sensores y mas
            if (true)
            {
                temp2.fecha = superior.fechaComparadora;
                temp2.fechaCorta = superior.fechaConsola;

                temp2.valorHumedadExterna = superior.valorHumedadExterna;
                temp2.valorHumedadInterna = superior.valorHumedadInterna;

                temp2.valorTemperaturaExterna = superior.valorTemperaturaExterna;
                temp2.valorTemperaturaInterna = superior.valorTemperaturaInterna;

                temp2.porcentajeAguaDisponible = superior.porcentajeAguaDisponible;

                temp2.capacidadTanque = superior.capacidadTanque;
                temp2.estadoRiego = superior.estadoRiego;
                temp2.tiempoRiego = superior.tiempoRiego;
            }

            //AGREGACION A LA LISTA GENERAL
            respuesta.Add(temp2);

        }


    }

    //solo existe inferior
    if (inferior != null && superior == null)
    {

        //a -todos los datos son los mismos del inicio al fin.
        datosHistorico temp = new datosHistorico();
        temp.horario = new List<TimeSpan>();

        temp.horario.Add(horario1);    //agregacion del inicio del horario_1

        TimeSpan intervalo = horario2 - horario1;

        for (int i = 0; i < intervalo.TotalSeconds; i++)
        {
            horario1 = horario1.Add(new TimeSpan(0, 0, 1));
            temp.horario.Add(horario1);
        }

        //agregacion de valores de sensores y mas
        if (true)
        {
            temp.fecha = inferior.fechaComparadora;
            temp.fechaCorta = inferior.fechaConsola;

            temp.valorHumedadExterna = inferior.valorHumedadExterna;
            temp.valorHumedadInterna = inferior.valorHumedadInterna;

            temp.valorTemperaturaExterna = inferior.valorTemperaturaExterna;
            temp.valorTemperaturaInterna = inferior.valorTemperaturaInterna;

            temp.porcentajeAguaDisponible = inferior.porcentajeAguaDisponible;

            temp.capacidadTanque = inferior.capacidadTanque;
            temp.estadoRiego = inferior.estadoRiego;
            temp.tiempoRiego = inferior.tiempoRiego;
        }

        //AGREGACION A LA LISTA GENERAL
        respuesta.Add(temp);


    }

    //Si existen ambos
    if( inferior != null && superior != null ){
        IEnumerable<DataRegistro> nuevoListado = registros.Where( e => e.horaCompleta >= inferior.horaCompleta &&  e.horaCompleta <= superior.horaCompleta );

       
        DataRegistro inferiorInterno = null;
        DataRegistro superiorInterno = null;

        foreach (var data in nuevoListado)
        {
            
            if (inferiorInterno == null)
            {
                inferiorInterno = data;

                if(  inferiorInterno.horaCompleta < horario1 ){
                    inferiorInterno.horaCompleta = horario1;
                }
                
                continue;
            }

            if (superiorInterno == null)
            {
                superiorInterno = data;

                if(  superiorInterno.horaCompleta > horario2 ){
                    
                    superiorInterno.horaCompleta = horario2;
                }


                //calculando los tiempos entre inferior y superior
                TimeSpan intervaloInterno = superiorInterno.horaCompleta - inferiorInterno.horaCompleta;

                datosHistorico temporalInterno = new datosHistorico();

                //Asignacion de los estados del registro
                if (true)
                {
                    temporalInterno.fecha = inferiorInterno.fechaComparadora;
                    temporalInterno.fechaCorta = inferiorInterno.fechaConsola;

                    temporalInterno.valorHumedadExterna = inferiorInterno.valorHumedadExterna;
                    temporalInterno.valorHumedadInterna = inferiorInterno.valorHumedadInterna;

                    temporalInterno.valorTemperaturaExterna = inferiorInterno.valorTemperaturaExterna;
                    temporalInterno.valorTemperaturaInterna = inferiorInterno.valorTemperaturaInterna;

                    temporalInterno.porcentajeAguaDisponible = inferiorInterno.porcentajeAguaDisponible;

                    temporalInterno.capacidadTanque = inferiorInterno.capacidadTanque;
                    temporalInterno.estadoRiego = inferiorInterno.estadoRiego;
                    temporalInterno.tiempoRiego = inferiorInterno.tiempoRiego;
                }


                temporalInterno.horario = new List<TimeSpan>();
                temporalInterno.horario.Add(inferiorInterno.horaCompleta);

                for (int i = 0; i < intervaloInterno.TotalSeconds - 1 ; i++)
                {
                    inferiorInterno.horaCompleta = inferiorInterno.horaCompleta.Add(new TimeSpan(0, 0, 1));
                    temporalInterno.horario.Add(inferiorInterno.horaCompleta);
                }
                //AGREGACION
                respuesta.Add(temporalInterno);

                continue;
            }

            //si llega aca entonces hay que cambiar el valor del inferior y el nuevo valor leido es el superior
            inferiorInterno = superiorInterno;
            superiorInterno = data;

            if(  superiorInterno.horaCompleta > horario2 ){
                superiorInterno.horaCompleta = horario2;
            }

            //calculando los tiempos entre inferior y superior
            TimeSpan intervalo = superiorInterno.horaCompleta - inferiorInterno.horaCompleta;

            datosHistorico temporal = new datosHistorico();

            //Asignacion de los estados del registro
            if (true)
            {
                temporal.fecha = inferiorInterno.fechaComparadora;
                temporal.fechaCorta = inferiorInterno.fechaConsola;

                temporal.valorHumedadExterna = inferiorInterno.valorHumedadExterna;
                temporal.valorHumedadInterna = inferiorInterno.valorHumedadInterna;

                temporal.valorTemperaturaExterna = inferiorInterno.valorTemperaturaExterna;
                temporal.valorTemperaturaInterna = inferiorInterno.valorTemperaturaInterna;

                temporal.porcentajeAguaDisponible = inferiorInterno.porcentajeAguaDisponible;

                temporal.capacidadTanque = inferiorInterno.capacidadTanque;
                temporal.estadoRiego = inferiorInterno.estadoRiego;
                temporal.tiempoRiego = inferiorInterno.tiempoRiego;
            }

            temporal.horario = new List<TimeSpan>();

            temporal.horario.Add(inferiorInterno.horaCompleta);

            for (int i = 0; i < intervalo.TotalSeconds - 1 ; i++)
            {
                inferiorInterno.horaCompleta = inferiorInterno.horaCompleta.Add(new TimeSpan(0, 0, 1));
                temporal.horario.Add(inferiorInterno.horaCompleta);
            }

            //AGREGACION
            respuesta.Add(temporal);
        }

        if (superiorInterno != null)
        {
            //agregacion del ultimo es decir su limite.
            datosHistorico temporalEx = new datosHistorico();

            //Asignacion de los estados del registro
            if (true)
            {
                temporalEx.fecha = superiorInterno.fechaComparadora;
                temporalEx.fechaCorta = superiorInterno.fechaConsola;

                temporalEx.valorHumedadExterna = superiorInterno.valorHumedadExterna;
                temporalEx.valorHumedadInterna = superiorInterno.valorHumedadInterna;

                temporalEx.valorTemperaturaExterna = superiorInterno.valorTemperaturaExterna;
                temporalEx.valorTemperaturaInterna = superiorInterno.valorTemperaturaInterna;

                temporalEx.porcentajeAguaDisponible = superiorInterno.porcentajeAguaDisponible;

                temporalEx.capacidadTanque = superiorInterno.capacidadTanque;
                temporalEx.estadoRiego = superiorInterno.estadoRiego;
                temporalEx.tiempoRiego = superiorInterno.tiempoRiego;
            }

            temporalEx.horario = new List<TimeSpan>();

            temporalEx.horario.Add(superiorInterno.horaCompleta);

            respuesta.Add(temporalEx);
        }

    }


    return Results.Ok(respuesta);

});



//=====================ENCENDIDO DE BOMBA====================================

#region Endpoint para encender la bomba

// app.MapGet("/encenderBomba", async ([FromServices] DataContext dbContext) =>
// {
//     return Results.Ok(dbContext.DatosAG);
// });


#endregion


app.Run();
