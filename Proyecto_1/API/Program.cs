using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using API.Context;

using API.Models;
using API.Utilidades;


//variable con la direccion del pueto que se va a utilizar.
String localHost = "http://localhost:5090";
String nombrePuerto = "COM5";


var builder = WebApplication.CreateBuilder(args);

//inyeccion de dependencia de sql
builder.Services.AddSqlServer<DataContext>(builder.Configuration.GetConnectionString("llave"));

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

var app = builder.Build();

app.UseCors( "AllowAll" );

//--------------------------------------CREACION DE BD-----------------------------------------------

//endpoint que vamos a utilizar para crear la instancia de la BD, basta con hacerlo una vez en el equipo que utilizaremos de manera local.
app.MapGet("/crearBD", async ([FromServices] DataContext dbContext) =>
{
    dbContext.Database.EnsureCreated();

    //crear la instancia del valor por defecto
    var recolectado = dbContext.Parametros.Count();

    if (recolectado == 0)
    {
        //si no existe entonces se crea uno por defecto
        ParamApp valDefault = new ParamApp();
        valDefault.id = Guid.NewGuid();
        valDefault.userName = "Batman";
        valDefault.valPomodoro = 25;
        valDefault.valDescanso = 5;
        valDefault.valDescansoLargo = 15;
        valDefault.codGrupo = Guid.NewGuid().ToString();

        await dbContext.AddAsync(valDefault);
        await dbContext.SaveChangesAsync();
    }

    return Results.Ok("Se creo la BD en tu sistema.");

});

//para eliminar la BD
app.MapGet("/eliminarBD", async ([FromServices] DataContext dbContext) =>
{
    dbContext.Database.EnsureDeleted();

    return Results.Ok("Se elimino la BD en tu sistema.");

});

//--------------------------------------CONSUMO DE DATOS SIMPLES--------------------------------------------

//ENDPOINT -> manda todos los datos. 
app.MapGet("/datos", async ([FromServices] DataContext dbContext) =>
{
    return Results.Ok(dbContext.Datos.OrderBy(e => e.fecha));
});

//ENDPINT -> manda los datos de la configuracion.
app.MapGet("/datosUser", async ([FromServices] DataContext dbContext) =>
{

    var recolectado = dbContext.Parametros.Count();

    if (recolectado == 0)
    {
        //si no existe entonces se crea uno por defecto
        ParamApp valDefault = new ParamApp();
        valDefault.id = Guid.NewGuid();
        valDefault.userName = "patito";
        valDefault.valPomodoro = 25;
        valDefault.valDescanso = 5;
        valDefault.valDescansoLargo = 15;

        await dbContext.AddAsync(valDefault);
        await dbContext.SaveChangesAsync();
        Results.Ok("Se inserto un nuevo dato");
    }

    return Results.Ok(dbContext.Parametros);

});

//ENDPOINT -> para obtener los usuarios
app.MapGet("/obtenerUsuarios", async ([FromServices] DataContext dbContext) =>
{

    parametrosFiltro respuesta = new parametrosFiltro();
    respuesta.usuarios = dbContext.Datos.Select(e => e.userName).Distinct();

    return Results.Ok(respuesta);
});

//-------------------------------------ACTUALIZACION DE DATOS---------------------------------------

//para actualizar los parametros de la aplicacion.
app.MapPut("/actualizarParametrosApp", async ([FromServices] DataContext dbContext, [FromBody] Recolector recolector) =>
{
    try
    {

        //validando que no venga vacio
        if (recolector.nameUser == string.Empty)
        {
            recolector.nameUser = "UserDafult";
        }

        //validando que el valor del nuevo pomodoro sea mayor a cero
        if (recolector.nuevoValPomodoro < 1 || recolector.nuevoValPomodoro > 45)
        {

            return Results.BadRequest("El valor del pomodoro no debe ser menor a 1");
        }

        //validando que el valor del nuevo descanso sea mayor a cero
        if (recolector.nuevoValDescanso < 1)
        {
            return Results.BadRequest("El valor del descanso no debe ser menor a 1");
        }

        var parametros = dbContext.Parametros.SingleOrDefault();

        parametros.userName = recolector.nameUser;
        parametros.valPomodoro = recolector.nuevoValPomodoro;
        parametros.valDescanso = recolector.nuevoValDescanso;
        parametros.valDescansoLargo = recolector.nuevoValDescansoLargo;

        await dbContext.SaveChangesAsync();

        return Results.Ok("Se actualizaron los parametros de la aplicacion.");

    }
    catch (Exception a)
    {
        return Results.BadRequest("Algo salio mal intente nuevamente.." + a.Message);
    }
});

//------------------------------------AGREGAR REGISTROS---------------------------------------------

//para agregar un dato -> esto vera como esta la data en cada momento
app.MapPost("/agregarRegistro", async ([FromServices] DataContext dbContext, [FromBody] Data registro) =>
{
    try
    {
        //Validacion de numero de pomodoro
        if ((registro.numeroPomodoro > 4 || 1 > registro.numeroPomodoro) && (!registro.descansoNormal || !registro.descansoLargo) && registro.numeroPomodoro != -1)
        {
            Console.WriteLine($"El valor leido es -> {registro.numeroPomodoro}");

            return Results.BadRequest("Error, el valor no esta dentro del intervalo de pomodoro... [1 -4] ");
        }

        //Validacion de numero de descanso
        if ((registro.numeroDescanso > 4 || 1 > registro.numeroDescanso) && (registro.descansoNormal || registro.descansoLargo) && registro.numeroDescanso != -1)
        {
            Console.WriteLine($"El valor leido es -> {registro.numeroDescanso}");
            return Results.BadRequest("Error, el valor no esta dentro del intervalo de descanso... [1 -4] ");
        }

        //viendo si nos mandan un parametro de fecha o no.
        if (registro.fecha == "" || registro.fecha == null)
        {
            //de aca sacaremos los datos
            DateTime fecha = DateTime.Now;

            registro.fecha = fecha.ToString();
            registro.fecha_corta = fecha.ToShortDateString().ToString();
            registro.dia = fecha.Day.ToString();
            registro.mes = fecha.Month.ToString();
            registro.anio = fecha.Year.ToString();
            registro.hora = fecha.Hour;
            registro.minuto = fecha.Minute;
            registro.segundo = fecha.Second;
            registro.fecha_comparadora = fecha;

        }

        else
        {
            //Si no esta vacio entonces debe cumplir con el formato definido
            try
            {

                //Pasar la fecha a los diferentes tipos que necesitamos.
                DateTime fechaConvertida = DateTime.ParseExact(registro.fecha, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                //asignacion de valores
                registro.fecha = fechaConvertida.ToString();
                registro.fecha_corta = fechaConvertida.ToShortDateString().ToString();
                registro.dia = fechaConvertida.Day.ToString();
                registro.mes = fechaConvertida.Month.ToString();
                registro.anio = fechaConvertida.Year.ToString();
                registro.hora = fechaConvertida.Hour;
                registro.minuto = fechaConvertida.Minute;
                registro.segundo = fechaConvertida.Second;
                registro.fecha_comparadora = fechaConvertida;

            }
            catch (Exception a)
            {

                return Results.BadRequest("Error, la fecha enviada no cumple con el formato ->'dd/MM/yyyy HH:mm:ss'\n" + a.Message);

            }
        }

        if (registro.userName == "")
        {
            var paramData = dbContext.Parametros.SingleOrDefault(); //datos de la configuracion
            if (paramData == null)
            {
                registro.userName = "SinRegistro";
            }
            else
            {
                registro.userName = paramData.userName;
            }
        }

        //se crea el identificador de este registro.
        registro.id = Guid.NewGuid();

        //aca les damos un numero de grupo.
        if (true)
        {
            //para mantenerlos por grupo haremos que tome el valor del codigo actal y que tambien genere uno nuevo.
            var datosParametros = dbContext.Parametros.SingleOrDefault();

            if (registro.numeroPomodoro == 1 && registro.inicio)
            {
                datosParametros.codGrupo = Guid.NewGuid().ToString();
            }
            registro.codGrupo = datosParametros.codGrupo;

            registro.ts = datosParametros.valPomodoro;
            registro.ds = datosParametros.valDescanso;
        }

        //ahora a registrarlo en la BD.
        await dbContext.AddAsync(registro);
        await dbContext.SaveChangesAsync();

        return Results.Ok("Se realizo un nuevo registro.");

    }
    catch (Exception a)
    {
        return Results.BadRequest("Algo salio mal intente nuevamente.." + a.Message);
    }
});

//--------------------------------CONSUMO DE DATOS FILTRADOS-----------------------------------------
//ENDPOINT que trae las fechas que posee un usuario
app.MapPost("/obtenerFechasUsuario", async ([FromServices] DataContext dbContext, [FromBody] Recolector recolector) =>
{
    IEnumerable<string> fechas = dbContext.Datos.Where(e => e.userName == recolector.nameUser).Select(e => e.fecha_corta).Distinct();

    parametrosFiltro respuesta = new parametrosFiltro();
    respuesta.fechas = fechas;

    return Results.Ok(respuesta);
});

app.MapPost("/obtenerGrupos", async ([FromServices] DataContext dbContext, [FromBody] Recolector recolector) =>
{
    conversor utilidad = new conversor();

    //tomar todos los registros que coincidan con el userName
    IList<Recolector> list = dbContext.Datos.Where(e => e.userName == recolector.nameUser
                                                            && e.fecha_corta == recolector.fecha1
                                                            && e.numeroPomodoro == 1 && e.inicio)
                                                    .Select(e => new Recolector()
                                                    {
                                                        codigoGrupo = e.codGrupo,
                                                        fecha1 = e.fecha
                                                    }).Distinct().ToList();
    foreach (var item in list)
    {
        item.stringToDateTime();
    }


    list = list.OrderBy(e => e.fechaComparadora).ToList();

    IList<string> listadoTmp = new List<string>();
    foreach (var item in list)
    {
        listadoTmp.Add(item.codigoGrupo);
    }

    IEnumerable<string> listado = listadoTmp;

    parametrosFiltro respuesta = new parametrosFiltro();
    respuesta.grupos = listado;

    return Results.Ok(respuesta);

});

//necesito el nameUser, y una fecha -> retorna todos los datos para la grafica uno.
app.MapPost("/grafica1", async ([FromServices] DataContext dbContext, [FromBody] Recolector recolector) =>
{

    conversor util = new conversor();
    DateTime limiteInferior = util.stringToDateTimeConHorioCero(recolector.fecha1);

    //para esta grafica mandaremos todo de manera seccionada es decir por codigo de grupo y filtrado por fecha y usuario

    //tomar todos los registros que coincidan con el userName
    IEnumerable<IGrouping<string, Data>> listado = dbContext.Datos.Where(e => e.userName == recolector.nameUser &&
                                                    e.fecha_comparadora.Date >= limiteInferior && limiteInferior >= e.fecha_comparadora.Date )
                                                    .OrderBy(e => e.fecha).GroupBy(e => e.codGrupo);

    IList<datosG1> datosGrafica = new List<datosG1>();

    conversor utilidad = new conversor();


    foreach (var grupo in listado)
    {
        datosG1 temporalGrafica = new datosG1();

        temporalGrafica.usuario = recolector.nameUser;

        Console.WriteLine($"\n\nSe encontro un grupo....-------------->>>>\n ");

        IEnumerable<Data> dataGroup = grupo.OrderBy(e => e.fecha);

        //agrupar los grupos por numeros de modoros.

        IEnumerable<IGrouping<int, Data>> pomodoros = dataGroup.Where(e => e.numeroPomodoro != -1 && e.numeroPomodoro != 0)
                                                    .OrderBy(e => e.numeroPomodoro).GroupBy(e => e.numeroPomodoro);

        // return Results.Ok(pomodoros);

        IEnumerable<IGrouping<int, Data>> descansos = dataGroup.Where(e => e.numeroDescanso != -1 && e.numeroDescanso != 0)
                                                    .GroupBy(e => e.numeroDescanso);

        //para agregar pomodoros
        foreach (var g_pom in pomodoros)
        {
            IEnumerable<Data> g_pomOrdenaro = g_pom.OrderBy(e => e.fecha);

            int numPomActual = 0;
            double tiempoAcumulado = 0;
            string inicio = "";
            string ref_tiempo = "";
            string final = "";
            bool agregar = true;

            foreach (var pom in g_pomOrdenaro)
            {

                if (pom.inicio)
                {
                    if (pom.numeroPomodoro == 1 && pom.inicio)
                    {
                        temporalGrafica.fecha_comparadora = utilidad.stringToDateTime(pom.fecha);
                        temporalGrafica.fecha = pom.fecha;
                        temporalGrafica.fecha_corta = pom.fecha_corta;
                        temporalGrafica.codigoGrupo = pom.codGrupo;
                        temporalGrafica.dia = pom.dia;
                        temporalGrafica.mes = pom.mes;
                        temporalGrafica.tiempoStandar = pom.ts;
                        temporalGrafica.descansoStandar = pom.ds;
                    }

                    tiempoAcumulado = 0;
                    numPomActual = pom.numeroPomodoro;
                    inicio = pom.fecha;
                    ref_tiempo = inicio;
                    agregar = true;
                }
                else if (pom.numeroPomodoro == numPomActual)
                {

                    //significa que puede ser un corte o el final del pomodoro
                    if (pom.fin)
                    {
                        final = pom.fecha;

                        //hacer la suma de tiempo trabajado
                        DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                        DateTime fechaActual = utilidad.stringToDateTime(pom.fecha);

                        TimeSpan duracion = fechaActual - fechaArriba;
                        double minutosTranscurridos = duracion.TotalMinutes;

                        if (agregar)
                        {
                            tiempoAcumulado += minutosTranscurridos;
                        }

                        //ahora a registrar esto.
                        subDatosGrafica1 nuevo = new subDatosGrafica1();

                        nuevo.inicio = inicio;
                        nuevo.fin = final;
                        nuevo.tiempo = tiempoAcumulado;
                        nuevo.penalizacion = pom.ts - tiempoAcumulado;

                        if (pom.numeroPomodoro == 1)
                        {
                            temporalGrafica.P1 = nuevo;
                        }
                        else if (pom.numeroPomodoro == 2)
                        {
                            temporalGrafica.P2 = nuevo;
                        }
                        else if (pom.numeroPomodoro == 3)
                        {
                            temporalGrafica.P3 = nuevo;
                        }
                        else if (pom.numeroPomodoro == 4)
                        {
                            temporalGrafica.P4 = nuevo;
                        }
                    }

                    else
                    {
                        //significa que existe un corte donde existe una penalizacion por no trabajar seguido
                        if (agregar)
                        {
                            //como sufrio un corte de flujo de trabajo entonces debe negar la agregacion.
                            agregar = false;

                            //hacemos una suma de lo que lleva por ahora el usuario
                            DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                            DateTime fechaActual = utilidad.stringToDateTime(pom.fecha);

                            TimeSpan duracion = fechaActual - fechaArriba;
                            double minutosTranscurridos = duracion.TotalMinutes;

                            tiempoAcumulado += minutosTranscurridos;

                        }
                        else
                        {
                            //significa que el pana regreso por ende vamos a hacerle la pala :v
                            agregar = true;
                            ref_tiempo = pom.fecha;
                        }
                    }
                }
            }

        }

        //para agregar descansos
        foreach (var g_desc in descansos)
        {
            g_desc.OrderBy(e => e.fecha);

            int numDescActual = 0;
            double tiempoAcumulado = 0;
            string inicio = "";
            string ref_tiempo = "";
            string final = "";
            bool agregar = true;

            foreach (var desc in g_desc)
            {
                if (desc.inicio)
                {
                    tiempoAcumulado = 0;
                    numDescActual = desc.numeroDescanso;
                    inicio = desc.fecha;
                    ref_tiempo = inicio;
                    agregar = true;
                }
                else if (desc.numeroDescanso == numDescActual)
                {

                    //significa que puede ser un corte o el final del pomodoro
                    if (desc.fin)
                    {
                        final = desc.fecha;

                        //hacer la suma de tiempo trabajado
                        DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                        DateTime fechaActual = utilidad.stringToDateTime(desc.fecha);

                        TimeSpan duracion = fechaActual - fechaArriba;
                        double minutosTranscurridos = duracion.TotalMinutes;

                        if (agregar)
                        {
                            tiempoAcumulado += minutosTranscurridos;
                        }

                        //ahora a registrar esto.
                        subDatosGrafica1 nuevo = new subDatosGrafica1();

                        nuevo.inicio = inicio;
                        nuevo.fin = final;
                        nuevo.tiempo = tiempoAcumulado;
                        nuevo.penalizacion = desc.ds - tiempoAcumulado;

                        if (desc.numeroDescanso == 1)
                        {
                            temporalGrafica.D1 = nuevo;
                        }
                        else if (desc.numeroDescanso == 2)
                        {
                            temporalGrafica.D2 = nuevo;
                        }
                        else if (desc.numeroDescanso == 3)
                        {
                            temporalGrafica.D3 = nuevo;
                        }
                        else if (desc.numeroDescanso == 4)
                        {
                            temporalGrafica.D4 = nuevo;
                        }
                    }

                    else
                    {
                        //significa que existe un corte donde existe una penalizacion por no trabajar seguido
                        if (agregar)
                        {
                            //como sufrio un corte de flujo de trabajo entonces debe negar la agregacion.
                            agregar = false;

                            //hacemos una suma de lo que lleva por ahora el usuario
                            //hacer la suma de tiempo trabajado
                            DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                            DateTime fechaActual = utilidad.stringToDateTime(desc.fecha);

                            TimeSpan duracion = fechaActual - fechaArriba;
                            double minutosTranscurridos = duracion.TotalMinutes;

                            tiempoAcumulado += minutosTranscurridos;

                        }
                        else
                        {
                            //significa que el pana regreso por ende vamos a hacerle la pala :v
                            agregar = true;
                            ref_tiempo = desc.fecha;
                        }
                    }
                }


            }

        }


        datosGrafica.Add(temporalGrafica);
    }

    //ordenar por fechas
    IEnumerable<datosG1> respuesta = datosGrafica.OrderBy(e => e.fecha_comparadora);



    return Results.Ok(respuesta);

});

//necesito el nameUser, y una fecha1  ( limiteInferior de filtrado).
//y una fecha2 ( limiteSuperior de filtrado ).
app.MapPost("/grafica2", async ([FromServices] DataContext dbContext, [FromBody] Recolector recolector) =>
{
    conversor util = new conversor();
    DateTime limiteInferior = util.stringToDateTimeConHorioCero(recolector.fecha1);
    DateTime limiteSuperior = util.stringToDateTimeConHorioCero(recolector.fecha2);
    //primero ver que si se cumpla
    //para esta grafica mandaremos todo de manera seccionada es decir por codigo de grupo y filtrado por fecha y usuario

    //tomar todos los registros que coincidan con el userName
    IEnumerable<IGrouping<string, Data>> listado = dbContext.Datos.Where(e => e.userName == recolector.nameUser &&
                                                    e.fecha_comparadora.Date >= limiteInferior && limiteSuperior >= e.fecha_comparadora.Date )
                                                    .OrderBy(e => e.fecha).GroupBy(e => e.codGrupo);

    IList<datosG1> datosGrafica = new List<datosG1>();

    conversor utilidad = new conversor();


    foreach (var grupo in listado)
    {
        datosG1 temporalGrafica = new datosG1();

        temporalGrafica.usuario = recolector.nameUser;

        Console.WriteLine($"\n\nSe encontro un grupo....-------------->>>>\n ");

        IEnumerable<Data> dataGroup = grupo.OrderBy(e => e.fecha);

        //agrupar los grupos por numeros de modoros.

        IEnumerable<IGrouping<int, Data>> pomodoros = dataGroup.Where(e => e.numeroPomodoro != -1 && e.numeroPomodoro != 0)
                                                    .OrderBy(e => e.numeroPomodoro).GroupBy(e => e.numeroPomodoro);

        // return Results.Ok(pomodoros);

        IEnumerable<IGrouping<int, Data>> descansos = dataGroup.Where(e => e.numeroDescanso != -1 && e.numeroDescanso != 0)
                                                    .GroupBy(e => e.numeroDescanso);

        //para agregar pomodoros
        foreach (var g_pom in pomodoros)
        {
            IEnumerable<Data> g_pomOrdenaro = g_pom.OrderBy(e => e.fecha);

            int numPomActual = 0;
            double tiempoAcumulado = 0;
            string inicio = "";
            string ref_tiempo = "";
            string final = "";
            bool agregar = true;

            foreach (var pom in g_pomOrdenaro)
            {

                if (pom.inicio)
                {
                    if (pom.numeroPomodoro == 1 && pom.inicio)
                    {
                        temporalGrafica.fecha_comparadora = utilidad.stringToDateTime(pom.fecha);
                        temporalGrafica.fecha = pom.fecha;
                        temporalGrafica.fecha_corta = pom.fecha_corta;
                        temporalGrafica.codigoGrupo = pom.codGrupo;
                        temporalGrafica.dia = pom.dia;
                        temporalGrafica.mes = pom.mes;
                        temporalGrafica.tiempoStandar = pom.ts;
                        temporalGrafica.descansoStandar = pom.ds;
                    }

                    tiempoAcumulado = 0;
                    numPomActual = pom.numeroPomodoro;
                    inicio = pom.fecha;
                    ref_tiempo = inicio;
                    agregar = true;
                }
                else if (pom.numeroPomodoro == numPomActual)
                {

                    //significa que puede ser un corte o el final del pomodoro
                    if (pom.fin)
                    {
                        final = pom.fecha;

                        //hacer la suma de tiempo trabajado
                        DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                        DateTime fechaActual = utilidad.stringToDateTime(pom.fecha);

                        TimeSpan duracion = fechaActual - fechaArriba;
                        double minutosTranscurridos = duracion.TotalMinutes;

                        if (agregar)
                        {
                            tiempoAcumulado += minutosTranscurridos;
                        }

                        //ahora a registrar esto.
                        subDatosGrafica1 nuevo = new subDatosGrafica1();

                        nuevo.inicio = inicio;
                        nuevo.fin = final;
                        nuevo.tiempo = tiempoAcumulado;
                        nuevo.penalizacion = pom.ts - tiempoAcumulado;

                        if (pom.numeroPomodoro == 1)
                        {
                            temporalGrafica.P1 = nuevo;
                        }
                        else if (pom.numeroPomodoro == 2)
                        {
                            temporalGrafica.P2 = nuevo;
                        }
                        else if (pom.numeroPomodoro == 3)
                        {
                            temporalGrafica.P3 = nuevo;
                        }
                        else if (pom.numeroPomodoro == 4)
                        {
                            temporalGrafica.P4 = nuevo;
                        }
                    }

                    else
                    {
                        //significa que existe un corte donde existe una penalizacion por no trabajar seguido
                        if (agregar)
                        {
                            //como sufrio un corte de flujo de trabajo entonces debe negar la agregacion.
                            agregar = false;

                            //hacemos una suma de lo que lleva por ahora el usuario
                            DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                            DateTime fechaActual = utilidad.stringToDateTime(pom.fecha);

                            TimeSpan duracion = fechaActual - fechaArriba;
                            double minutosTranscurridos = duracion.TotalMinutes;

                            tiempoAcumulado += minutosTranscurridos;

                        }
                        else
                        {
                            //significa que el pana regreso por ende vamos a hacerle la pala :v
                            agregar = true;
                            ref_tiempo = pom.fecha;
                        }
                    }
                }
            }

        }

        //para agregar descansos
        foreach (var g_desc in descansos)
        {
            g_desc.OrderBy(e => e.fecha);

            int numDescActual = 0;
            double tiempoAcumulado = 0;
            string inicio = "";
            string ref_tiempo = "";
            string final = "";
            bool agregar = true;

            foreach (var desc in g_desc)
            {
                if (desc.inicio)
                {
                    tiempoAcumulado = 0;
                    numDescActual = desc.numeroDescanso;
                    inicio = desc.fecha;
                    ref_tiempo = inicio;
                    agregar = true;
                }
                else if (desc.numeroDescanso == numDescActual)
                {

                    //significa que puede ser un corte o el final del pomodoro
                    if (desc.fin)
                    {
                        final = desc.fecha;

                        //hacer la suma de tiempo trabajado
                        DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                        DateTime fechaActual = utilidad.stringToDateTime(desc.fecha);

                        TimeSpan duracion = fechaActual - fechaArriba;
                        double minutosTranscurridos = duracion.TotalMinutes;

                        if (agregar)
                        {
                            tiempoAcumulado += minutosTranscurridos;
                        }

                        //ahora a registrar esto.
                        subDatosGrafica1 nuevo = new subDatosGrafica1();

                        nuevo.inicio = inicio;
                        nuevo.fin = final;
                        nuevo.tiempo = tiempoAcumulado;
                        nuevo.penalizacion = desc.ds - tiempoAcumulado;

                        if (desc.numeroDescanso == 1)
                        {
                            temporalGrafica.D1 = nuevo;
                        }
                        else if (desc.numeroDescanso == 2)
                        {
                            temporalGrafica.D2 = nuevo;
                        }
                        else if (desc.numeroDescanso == 3)
                        {
                            temporalGrafica.D3 = nuevo;
                        }
                        else if (desc.numeroDescanso == 4)
                        {
                            temporalGrafica.D4 = nuevo;
                        }
                    }

                    else
                    {
                        //significa que existe un corte donde existe una penalizacion por no trabajar seguido
                        if (agregar)
                        {
                            //como sufrio un corte de flujo de trabajo entonces debe negar la agregacion.
                            agregar = false;

                            //hacemos una suma de lo que lleva por ahora el usuario
                            //hacer la suma de tiempo trabajado
                            DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                            DateTime fechaActual = utilidad.stringToDateTime(desc.fecha);

                            TimeSpan duracion = fechaActual - fechaArriba;
                            double minutosTranscurridos = duracion.TotalMinutes;

                            tiempoAcumulado += minutosTranscurridos;

                        }
                        else
                        {
                            //significa que el pana regreso por ende vamos a hacerle la pala :v
                            agregar = true;
                            ref_tiempo = desc.fecha;
                        }
                    }
                }


            }

        }


        datosGrafica.Add(temporalGrafica);
    }

    //ordenar por fechas
    IEnumerable<datosG1> respuesta = datosGrafica.OrderBy(e => e.fecha_comparadora);



    return Results.Ok(respuesta);

});

app.MapPost("/grafica_4_5_6", async ([FromServices] DataContext dbContext, [FromBody] Recolector recolector) =>
{
    conversor util = new conversor();
    DateTime limiteInferior = util.stringToDateTimeConHorioCero(recolector.fecha1);
    DateTime limiteSuperior = util.stringToDateTimeConHorioCero(recolector.fecha2);

    //primero ver que si se cumpla
    //para esta grafica mandaremos todo de manera seccionada es decir por codigo de grupo y filtrado por fecha y usuario

    //tomar todos los registros que coincidan con el userName
    IEnumerable<IGrouping<string, Data>> listado = dbContext.Datos.Where(e => e.userName == recolector.nameUser &&
                                                    e.fecha_comparadora.Date >= limiteInferior &&  e.fecha_comparadora.Date <= limiteSuperior )
                                                    .OrderBy(e => e.fecha).GroupBy(e => e.codGrupo);

    IList<datosG1> datosGrafica = new List<datosG1>();

    conversor utilidad = new conversor();


    foreach (var grupo in listado)
    {
        datosG1 temporalGrafica = new datosG1();

        temporalGrafica.usuario = recolector.nameUser;

        Console.WriteLine($"\n\nSe encontro un grupo....-------------->>>>\n ");

        IEnumerable<Data> dataGroup = grupo.OrderBy(e => e.fecha);

        //agrupar los grupos por numeros de modoros.

        IEnumerable<IGrouping<int, Data>> pomodoros = dataGroup.Where(e => e.numeroPomodoro != -1 && e.numeroPomodoro != 0)
                                                    .OrderBy(e => e.numeroPomodoro).GroupBy(e => e.numeroPomodoro);

        // return Results.Ok(pomodoros);

        IEnumerable<IGrouping<int, Data>> descansos = dataGroup.Where(e => e.numeroDescanso != -1 && e.numeroDescanso != 0)
                                                    .GroupBy(e => e.numeroDescanso);

        //para agregar pomodoros
        foreach (var g_pom in pomodoros)
        {
            IEnumerable<Data> g_pomOrdenaro = g_pom.OrderBy(e => e.fecha);

            int numPomActual = 0;
            double tiempoAcumulado = 0;
            string inicio = "";
            string ref_tiempo = "";
            string final = "";
            bool agregar = true;

            IList<penalizacion> listaPenalizaciones = new List<penalizacion>();

            foreach (var pom in g_pomOrdenaro)
            {

                if (pom.inicio)
                {
                    if (pom.numeroPomodoro == 1 && pom.inicio)
                    {

                        temporalGrafica.fecha_comparadora = utilidad.stringToDateTime(pom.fecha);
                        temporalGrafica.fecha = pom.fecha;
                        temporalGrafica.fecha_corta = pom.fecha_corta;
                        temporalGrafica.codigoGrupo = pom.codGrupo;
                        temporalGrafica.dia = pom.dia;
                        temporalGrafica.mes = pom.mes;
                        temporalGrafica.tiempoStandar = pom.ts;
                        temporalGrafica.descansoStandar = pom.ds;

                    }

                    tiempoAcumulado = 0;
                    numPomActual = pom.numeroPomodoro;
                    inicio = pom.fecha;
                    ref_tiempo = inicio;
                    agregar = true;


                }
                else if (pom.numeroPomodoro == numPomActual)
                {

                    //significa que puede ser un corte o el final del pomodoro
                    if (pom.fin)
                    {
                        final = pom.fecha;

                        //hacer la suma de tiempo trabajado
                        DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                        DateTime fechaActual = utilidad.stringToDateTime(pom.fecha);

                        TimeSpan duracion = fechaActual - fechaArriba;
                        double minutosTranscurridos = duracion.TotalMinutes;

                        if (agregar)
                        {
                            tiempoAcumulado += minutosTranscurridos;
                        }

                        //ahora a registrar esto.
                        subDatosGrafica1 nuevo = new subDatosGrafica1();

                        nuevo.inicio = inicio;
                        nuevo.fin = final;
                        nuevo.tiempo = tiempoAcumulado;
                        nuevo.penalizacion = pom.ts - tiempoAcumulado;
                        nuevo.listaPensalizaciones = listaPenalizaciones;
                        listaPenalizaciones = new List<penalizacion>();

                        if (pom.numeroPomodoro == 1)
                        {
                            temporalGrafica.P1 = nuevo;
                        }
                        else if (pom.numeroPomodoro == 2)
                        {
                            temporalGrafica.P2 = nuevo;
                        }
                        else if (pom.numeroPomodoro == 3)
                        {
                            temporalGrafica.P3 = nuevo;
                        }
                        else if (pom.numeroPomodoro == 4)
                        {
                            temporalGrafica.P4 = nuevo;
                        }
                    }

                    else
                    {
                        //significa que existe un corte donde existe una penalizacion por no trabajar seguido
                        if (agregar)
                        {
                            //como sufrio un corte de flujo de trabajo entonces debe negar la agregacion.
                            agregar = false;

                            //hacemos una suma de lo que lleva por ahora el usuario
                            DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                            DateTime fechaActual = utilidad.stringToDateTime(pom.fecha);

                            TimeSpan duracion = fechaActual - fechaArriba;
                            double minutosTranscurridos = duracion.TotalMinutes;

                            tiempoAcumulado += minutosTranscurridos;

                            penalizacion nuevaPenalizacionInicio = new penalizacion();
                            nuevaPenalizacionInicio.inicio = true;
                            nuevaPenalizacionInicio.fin = false;
                            nuevaPenalizacionInicio.fecha = fechaActual;

                            listaPenalizaciones.Add(nuevaPenalizacionInicio);

                        }
                        else
                        {
                            //significa que el pana regreso por ende vamos a hacerle la pala :v
                            agregar = true;
                            ref_tiempo = pom.fecha;

                            DateTime fechaActual = utilidad.stringToDateTime(pom.fecha);

                            penalizacion nuevaPenalizacionFin = new penalizacion();
                            nuevaPenalizacionFin.inicio = false;
                            nuevaPenalizacionFin.fin = true;
                            nuevaPenalizacionFin.fecha = fechaActual;

                            listaPenalizaciones.Add(nuevaPenalizacionFin);
                        }
                    }
                }
            }

        }

        //para agregar descansos
        foreach (var g_desc in descansos)
        {
            g_desc.OrderBy(e => e.fecha);

            int numDescActual = 0;
            double tiempoAcumulado = 0;
            string inicio = "";
            string ref_tiempo = "";
            string final = "";
            bool agregar = true;

            IList<penalizacion> listaPenalizaciones = new List<penalizacion>();

            foreach (var desc in g_desc)
            {
                if (desc.inicio)
                {
                    tiempoAcumulado = 0;
                    numDescActual = desc.numeroDescanso;
                    inicio = desc.fecha;
                    ref_tiempo = inicio;
                    agregar = true;
                }
                else if (desc.numeroDescanso == numDescActual)
                {

                    //significa que puede ser un corte o el final del pomodoro
                    if (desc.fin)
                    {
                        final = desc.fecha;

                        //hacer la suma de tiempo trabajado
                        DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                        DateTime fechaActual = utilidad.stringToDateTime(desc.fecha);

                        TimeSpan duracion = fechaActual - fechaArriba;
                        double minutosTranscurridos = duracion.TotalMinutes;

                        if (agregar)
                        {
                            tiempoAcumulado += minutosTranscurridos;
                        }

                        //ahora a registrar esto.
                        subDatosGrafica1 nuevo = new subDatosGrafica1();

                        nuevo.inicio = inicio;
                        nuevo.fin = final;
                        nuevo.tiempo = tiempoAcumulado;
                        nuevo.penalizacion = desc.ds - tiempoAcumulado;
                        nuevo.listaPensalizaciones = listaPenalizaciones;
                        listaPenalizaciones = new List<penalizacion>();

                        if (desc.numeroDescanso == 1)
                        {
                            temporalGrafica.D1 = nuevo;
                        }
                        else if (desc.numeroDescanso == 2)
                        {
                            temporalGrafica.D2 = nuevo;
                        }
                        else if (desc.numeroDescanso == 3)
                        {
                            temporalGrafica.D3 = nuevo;
                        }
                        else if (desc.numeroDescanso == 4)
                        {
                            temporalGrafica.D4 = nuevo;
                        }
                    }

                    else
                    {
                        //significa que existe un corte donde existe una penalizacion por no trabajar seguido
                        if (agregar)
                        {
                            //como sufrio un corte de flujo de trabajo entonces debe negar la agregacion.
                            agregar = false;

                            //hacemos una suma de lo que lleva por ahora el usuario
                            //hacer la suma de tiempo trabajado
                            DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                            DateTime fechaActual = utilidad.stringToDateTime(desc.fecha);

                            TimeSpan duracion = fechaActual - fechaArriba;
                            double minutosTranscurridos = duracion.TotalMinutes;

                            tiempoAcumulado += minutosTranscurridos;
                            penalizacion nuevaPenalizacionInicio = new penalizacion();
                            nuevaPenalizacionInicio.inicio = true;
                            nuevaPenalizacionInicio.fin = false;
                            nuevaPenalizacionInicio.fecha = fechaActual;

                            listaPenalizaciones.Add(nuevaPenalizacionInicio);

                        }
                        else
                        {
                            //significa que el pana regreso por ende vamos a hacerle la pala :v
                            agregar = true;
                            ref_tiempo = desc.fecha;

                            DateTime fechaActual = utilidad.stringToDateTime(desc.fecha);

                            penalizacion nuevaPenalizacionFin = new penalizacion();
                            nuevaPenalizacionFin.inicio = false;
                            nuevaPenalizacionFin.fin = true;
                            nuevaPenalizacionFin.fecha = fechaActual;

                            listaPenalizaciones.Add(nuevaPenalizacionFin);

                        }
                    }
                }


            }

        }


        datosGrafica.Add(temporalGrafica);
    }

    //ordenar por fechas
    IEnumerable<datosG1> respuesta = datosGrafica.OrderBy(e => e.fecha_comparadora);

    //agrupando datos por fecha corta.
    IEnumerable<IGrouping<string, datosG1>> listado2 = respuesta.GroupBy(e => e.fecha_corta);

    //DECLARACION DEL OBJETO RESPUESTA
    datosGFJson respuestaFinal = new datosGFJson();
    respuestaFinal.data = new List<datosGF>();

    // return Results.Ok( listado2 ); //COMENTAR ------------------------->

    foreach (var items in listado2)
    {

        datosGF temporal = new datosGF();
        temporal.descansos = new List<elementoPrimario>();
        temporal.trabajo = new List<elementoPrimario>();


        Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        bool mostrar = true;



        foreach (var elemento in items)
        {

            elementoPrimario primario = new elementoPrimario();
            IList<elementoInterno> dataP1 = new List<elementoInterno>();
            primario.dataPrimaria = dataP1;

            elementoPrimario primarioDescanso = new elementoPrimario();
            IList<elementoInterno> dataPD = new List<elementoInterno>();
            primarioDescanso.dataPrimaria = dataPD;

            if (mostrar)
            {
                temporal.fecha = elemento.fecha;
                temporal.fecha_corta = elemento.fecha_corta;
                temporal.fecha_comparadora = elemento.fecha_comparadora;
                Console.WriteLine($"La fecha de este agrupamiento es --> {elemento.fecha_corta}");
                mostrar = false;
            }

            //cada uno de estos elementos es donde debo recolectar la data.
            if (elemento.P1 != null)
            {
                //agregar la data de este elemento.
                //necesito crear un elemento interno.
                elementoInterno interno = new elementoInterno();
                interno.nameElemento = "Pomodoro 1";
                interno.arreglo = new List<claveValor>();

                DateTime tiempoReferencia = utilidad.stringToDateTime(elemento.P1.inicio);

                bool primero = true;

                bool sentado = true;

                if (elemento.P1.listaPensalizaciones != null)
                {
                    //ver si tenemos elementos.
                    if (elemento.P1.listaPensalizaciones.Count() > 0)
                    {

                        int contador = 0;
                        penalizacion[] arreglo = elemento.P1.listaPensalizaciones.ToArray();



                        while (contador < arreglo.Length)
                        {

                            //si es la interrupcion inicial.
                            if (arreglo[contador].inicio)
                            {
                                //calcular los minutos y agregar los elementos.\

                                //tomar el valor de la penalizacion inicio.
                                DateTime fechaTemporal = arreglo[contador].fecha;

                                while (tiempoReferencia < fechaTemporal)
                                {
                                    claveValor nuevaClaveValor = new claveValor();
                                    nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                    nuevaClaveValor.valor = 0;

                                    interno.arreglo.Add(nuevaClaveValor);

                                    tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                }

                                tiempoReferencia = fechaTemporal;
                                //aumentar el contador
                                contador++;
                                sentado = false;
                            }
                            else
                            {

                                //si es la interrupcion inicial.
                                if (arreglo[contador].fin)
                                {
                                    //calcular los minutos y agregar los elementos.\

                                    //tomar el valor de la penalizacion inicio.
                                    DateTime fechaTemporal = arreglo[contador].fecha;

                                    while (tiempoReferencia <= fechaTemporal)
                                    {
                                        claveValor nuevaClaveValor = new claveValor();
                                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                        nuevaClaveValor.valor = 1;

                                        interno.arreglo.Add(nuevaClaveValor);

                                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                    }

                                    tiempoReferencia = fechaTemporal;
                                    //aumentar el contador
                                    contador++;
                                    sentado = true;
                                }
                            }

                        }
                    }

                }

                if (sentado)
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.P1.fin);

                    while (tiempoReferencia < fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 0;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }

                }
                else
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.P1.fin);

                    while (tiempoReferencia <= fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 1;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }
                }

                dataP1.Add(interno);

                //creacion de elemento interno.
            }

            //cada uno de estos elementos es donde debo recolectar la data.
            if (elemento.P2 != null)
            {
                //agregar la data de este elemento.
                //necesito crear un elemento interno.
                elementoInterno interno = new elementoInterno();
                interno.nameElemento = "Pomodoro 2";
                interno.arreglo = new List<claveValor>();

                DateTime tiempoReferencia = utilidad.stringToDateTime(elemento.P2.inicio);

                bool primero = true;

                bool sentado = true;

                if (elemento.P2.listaPensalizaciones != null)
                {
                    //ver si tenemos elementos.
                    if (elemento.P2.listaPensalizaciones.Count() > 0)
                    {

                        int contador = 0;
                        penalizacion[] arreglo = elemento.P2.listaPensalizaciones.ToArray();



                        while (contador < arreglo.Length)
                        {

                            //si es la interrupcion inicial.
                            if (arreglo[contador].inicio)
                            {
                                //calcular los minutos y agregar los elementos.\

                                //tomar el valor de la penalizacion inicio.
                                DateTime fechaTemporal = arreglo[contador].fecha;

                                while (tiempoReferencia < fechaTemporal)
                                {
                                    claveValor nuevaClaveValor = new claveValor();
                                    nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                    nuevaClaveValor.valor = 0;

                                    interno.arreglo.Add(nuevaClaveValor);

                                    tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                }

                                tiempoReferencia = fechaTemporal;
                                //aumentar el contador
                                contador++;
                                sentado = false;
                            }
                            else
                            {

                                //si es la interrupcion inicial.
                                if (arreglo[contador].fin)
                                {
                                    //calcular los minutos y agregar los elementos.\

                                    //tomar el valor de la penalizacion inicio.
                                    DateTime fechaTemporal = arreglo[contador].fecha;

                                    while (tiempoReferencia <= fechaTemporal)
                                    {
                                        claveValor nuevaClaveValor = new claveValor();
                                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                        nuevaClaveValor.valor = 1;

                                        interno.arreglo.Add(nuevaClaveValor);

                                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                    }

                                    tiempoReferencia = fechaTemporal;
                                    //aumentar el contador
                                    contador++;
                                    sentado = true;
                                }
                            }

                        }
                    }

                }

                if (sentado)
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.P2.fin);

                    while (tiempoReferencia < fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 0;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }

                }
                else
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.P2.fin);

                    while (tiempoReferencia <= fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 1;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }
                }

                dataP1.Add(interno);

                //creacion de elemento interno.
            }

            if (elemento.P3 != null)
            {
                //agregar la data de este elemento.
                //necesito crear un elemento interno.
                elementoInterno interno = new elementoInterno();
                interno.nameElemento = "Pomodoro 3";
                interno.arreglo = new List<claveValor>();

                DateTime tiempoReferencia = utilidad.stringToDateTime(elemento.P3.inicio);

                bool primero = true;

                bool sentado = true;

                if (elemento.P3.listaPensalizaciones != null)
                {
                    //ver si tenemos elementos.
                    if (elemento.P3.listaPensalizaciones.Count() > 0)
                    {

                        int contador = 0;
                        penalizacion[] arreglo = elemento.P3.listaPensalizaciones.ToArray();



                        while (contador < arreglo.Length)
                        {

                            //si es la interrupcion inicial.
                            if (arreglo[contador].inicio)
                            {
                                //calcular los minutos y agregar los elementos.\

                                //tomar el valor de la penalizacion inicio.
                                DateTime fechaTemporal = arreglo[contador].fecha;

                                while (tiempoReferencia < fechaTemporal)
                                {
                                    claveValor nuevaClaveValor = new claveValor();
                                    nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                    nuevaClaveValor.valor = 0;

                                    interno.arreglo.Add(nuevaClaveValor);

                                    tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                }

                                tiempoReferencia = fechaTemporal;
                                //aumentar el contador
                                contador++;
                                sentado = false;
                            }
                            else
                            {

                                //si es la interrupcion inicial.
                                if (arreglo[contador].fin)
                                {
                                    //calcular los minutos y agregar los elementos.\

                                    //tomar el valor de la penalizacion inicio.
                                    DateTime fechaTemporal = arreglo[contador].fecha;

                                    while (tiempoReferencia <= fechaTemporal)
                                    {
                                        claveValor nuevaClaveValor = new claveValor();
                                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                        nuevaClaveValor.valor = 1;

                                        interno.arreglo.Add(nuevaClaveValor);

                                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                    }

                                    tiempoReferencia = fechaTemporal;
                                    //aumentar el contador
                                    contador++;
                                    sentado = true;
                                }
                            }

                        }
                    }

                }

                if (sentado)
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.P3.fin);

                    while (tiempoReferencia < fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 0;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }

                }
                else
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.P3.fin);

                    while (tiempoReferencia <= fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 1;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }
                }

                dataP1.Add(interno);

                //creacion de elemento interno.
            }

            if (elemento.P4 != null)
            {
                //agregar la data de este elemento.
                //necesito crear un elemento interno.
                elementoInterno interno = new elementoInterno();
                interno.nameElemento = "Pomodoro 4";
                interno.arreglo = new List<claveValor>();

                DateTime tiempoReferencia = utilidad.stringToDateTime(elemento.P4.inicio);

                bool primero = true;

                bool sentado = true;

                if (elemento.P4.listaPensalizaciones != null)
                {
                    //ver si tenemos elementos.
                    if (elemento.P4.listaPensalizaciones.Count() > 0)
                    {

                        int contador = 0;
                        penalizacion[] arreglo = elemento.P4.listaPensalizaciones.ToArray();



                        while (contador < arreglo.Length)
                        {

                            //si es la interrupcion inicial.
                            if (arreglo[contador].inicio)
                            {
                                //calcular los minutos y agregar los elementos.\

                                //tomar el valor de la penalizacion inicio.
                                DateTime fechaTemporal = arreglo[contador].fecha;

                                while (tiempoReferencia < fechaTemporal)
                                {
                                    claveValor nuevaClaveValor = new claveValor();
                                    nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                    nuevaClaveValor.valor = 0;

                                    interno.arreglo.Add(nuevaClaveValor);

                                    tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                }

                                tiempoReferencia = fechaTemporal;
                                //aumentar el contador
                                contador++;
                                sentado = false;
                            }
                            else
                            {

                                //si es la interrupcion inicial.
                                if (arreglo[contador].fin)
                                {
                                    //calcular los minutos y agregar los elementos.\

                                    //tomar el valor de la penalizacion inicio.
                                    DateTime fechaTemporal = arreglo[contador].fecha;

                                    while (tiempoReferencia <= fechaTemporal)
                                    {
                                        claveValor nuevaClaveValor = new claveValor();
                                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                        nuevaClaveValor.valor = 1;

                                        interno.arreglo.Add(nuevaClaveValor);

                                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                    }

                                    tiempoReferencia = fechaTemporal;
                                    //aumentar el contador
                                    contador++;
                                    sentado = true;
                                }
                            }

                        }
                    }

                }

                if (sentado)
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.P4.fin);

                    while (tiempoReferencia < fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 0;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }

                }
                else
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.P4.fin);

                    while (tiempoReferencia <= fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 1;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }
                }

                dataP1.Add(interno);

                //creacion de elemento interno.
            }

            // -----------------------------DESCANSOS----------------------------------
            //cada uno de estos elementos es donde debo recolectar la data.
            if (elemento.D1 != null)
            {
                //agregar la data de este elemento.
                //necesito crear un elemento interno.
                elementoInterno interno = new elementoInterno();
                interno.nameElemento = "Descanso 1";
                interno.arreglo = new List<claveValor>();

                DateTime tiempoReferencia = utilidad.stringToDateTime(elemento.D1.inicio);

                bool primero = true;

                bool sentado = false;

                if (elemento.D1.listaPensalizaciones != null)
                {
                    //ver si tenemos elementos.
                    if (elemento.D1.listaPensalizaciones.Count() > 0)
                    {

                        int contador = 0;
                        penalizacion[] arreglo = elemento.D1.listaPensalizaciones.ToArray();



                        while (contador < arreglo.Length)
                        {

                            //si es la interrupcion inicial.
                            if (arreglo[contador].inicio)
                            {
                                //calcular los minutos y agregar los elementos.\

                                //tomar el valor de la penalizacion inicio.
                                DateTime fechaTemporal = arreglo[contador].fecha;

                                while (tiempoReferencia < fechaTemporal)
                                {
                                    claveValor nuevaClaveValor = new claveValor();
                                    nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                    nuevaClaveValor.valor = 1;

                                    interno.arreglo.Add(nuevaClaveValor);

                                    tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                }

                                tiempoReferencia = fechaTemporal;
                                //aumentar el contador
                                contador++;
                                sentado = true;
                            }
                            else
                            {

                                //si es la interrupcion inicial.
                                if (arreglo[contador].fin)
                                {
                                    //calcular los minutos y agregar los elementos.\

                                    //tomar el valor de la penalizacion inicio.
                                    DateTime fechaTemporal = arreglo[contador].fecha;

                                    while (tiempoReferencia <= fechaTemporal)
                                    {
                                        claveValor nuevaClaveValor = new claveValor();
                                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                        nuevaClaveValor.valor = 0;

                                        interno.arreglo.Add(nuevaClaveValor);

                                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                    }

                                    tiempoReferencia = fechaTemporal;
                                    //aumentar el contador
                                    contador++;
                                    sentado = false;
                                }
                            }

                        }
                    }

                }

                if (sentado)
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.D1.fin);

                    while (tiempoReferencia < fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 0;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }

                }
                else
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.D1.fin);

                    while (tiempoReferencia <= fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 1;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }
                }

                dataPD.Add(interno);

                //creacion de elemento interno.
            }

            if (elemento.D2 != null)
            {
                //agregar la data de este elemento.
                //necesito crear un elemento interno.
                elementoInterno interno = new elementoInterno();
                interno.nameElemento = "Descanso 2";
                interno.arreglo = new List<claveValor>();

                DateTime tiempoReferencia = utilidad.stringToDateTime(elemento.D2.inicio);

                bool primero = true;

                bool sentado = false;

                if (elemento.D2.listaPensalizaciones != null)
                {
                    //ver si tenemos elementos.
                    if (elemento.D2.listaPensalizaciones.Count() > 0)
                    {

                        int contador = 0;
                        penalizacion[] arreglo = elemento.D2.listaPensalizaciones.ToArray();



                        while (contador < arreglo.Length)
                        {

                            //si es la interrupcion inicial.
                            if (arreglo[contador].inicio)
                            {
                                //calcular los minutos y agregar los elementos.\

                                //tomar el valor de la penalizacion inicio.
                                DateTime fechaTemporal = arreglo[contador].fecha;

                                while (tiempoReferencia < fechaTemporal)
                                {
                                    claveValor nuevaClaveValor = new claveValor();
                                    nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                    nuevaClaveValor.valor = 1;

                                    interno.arreglo.Add(nuevaClaveValor);

                                    tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                }

                                tiempoReferencia = fechaTemporal;
                                //aumentar el contador
                                contador++;
                                sentado = true;
                            }
                            else
                            {

                                //si es la interrupcion inicial.
                                if (arreglo[contador].fin)
                                {
                                    //calcular los minutos y agregar los elementos.\

                                    //tomar el valor de la penalizacion inicio.
                                    DateTime fechaTemporal = arreglo[contador].fecha;

                                    while (tiempoReferencia <= fechaTemporal)
                                    {
                                        claveValor nuevaClaveValor = new claveValor();
                                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                        nuevaClaveValor.valor = 0;

                                        interno.arreglo.Add(nuevaClaveValor);

                                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                    }

                                    tiempoReferencia = fechaTemporal;
                                    //aumentar el contador
                                    contador++;
                                    sentado = false;
                                }
                            }

                        }
                    }

                }

                if (sentado)
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.D2.fin);

                    while (tiempoReferencia < fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 0;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }

                }
                else
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.D2.fin);

                    while (tiempoReferencia <= fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 1;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }
                }

                dataPD.Add(interno);

                //creacion de elemento interno.
            }

            if (elemento.D3 != null)
            {
                //agregar la data de este elemento.
                //necesito crear un elemento interno.
                elementoInterno interno = new elementoInterno();
                interno.nameElemento = "Descanso 3";
                interno.arreglo = new List<claveValor>();

                DateTime tiempoReferencia = utilidad.stringToDateTime(elemento.D3.inicio);

                bool primero = true;

                bool sentado = false;

                if (elemento.D3.listaPensalizaciones != null)
                {
                    //ver si tenemos elementos.
                    if (elemento.D3.listaPensalizaciones.Count() > 0)
                    {

                        int contador = 0;
                        penalizacion[] arreglo = elemento.D3.listaPensalizaciones.ToArray();



                        while (contador < arreglo.Length)
                        {

                            //si es la interrupcion inicial.
                            if (arreglo[contador].inicio)
                            {
                                //calcular los minutos y agregar los elementos.\

                                //tomar el valor de la penalizacion inicio.
                                DateTime fechaTemporal = arreglo[contador].fecha;

                                while (tiempoReferencia < fechaTemporal)
                                {
                                    claveValor nuevaClaveValor = new claveValor();
                                    nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                    nuevaClaveValor.valor = 1;

                                    interno.arreglo.Add(nuevaClaveValor);

                                    tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                }

                                tiempoReferencia = fechaTemporal;
                                //aumentar el contador
                                contador++;
                                sentado = true;
                            }
                            else
                            {

                                //si es la interrupcion inicial.
                                if (arreglo[contador].fin)
                                {
                                    //calcular los minutos y agregar los elementos.\

                                    //tomar el valor de la penalizacion inicio.
                                    DateTime fechaTemporal = arreglo[contador].fecha;

                                    while (tiempoReferencia <= fechaTemporal)
                                    {
                                        claveValor nuevaClaveValor = new claveValor();
                                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                        nuevaClaveValor.valor = 0;

                                        interno.arreglo.Add(nuevaClaveValor);

                                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                    }

                                    tiempoReferencia = fechaTemporal;
                                    //aumentar el contador
                                    contador++;
                                    sentado = false;
                                }
                            }

                        }
                    }

                }

                if (sentado)
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.D3.fin);

                    while (tiempoReferencia < fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 0;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }

                }
                else
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.D3.fin);

                    while (tiempoReferencia <= fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 1;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }
                }

                dataPD.Add(interno);

                //creacion de elemento interno.
            }

            if (elemento.D4 != null)
            {
                //agregar la data de este elemento.
                //necesito crear un elemento interno.
                elementoInterno interno = new elementoInterno();
                interno.nameElemento = "Descanso 4";
                interno.arreglo = new List<claveValor>();

                DateTime tiempoReferencia = utilidad.stringToDateTime(elemento.D4.inicio);

                bool primero = true;

                bool sentado = false;

                if (elemento.D4.listaPensalizaciones != null)
                {
                    //ver si tenemos elementos.
                    if (elemento.D4.listaPensalizaciones.Count() > 0)
                    {

                        int contador = 0;
                        penalizacion[] arreglo = elemento.D4.listaPensalizaciones.ToArray();



                        while (contador < arreglo.Length)
                        {

                            //si es la interrupcion inicial.
                            if (arreglo[contador].inicio)
                            {
                                //calcular los minutos y agregar los elementos.\

                                //tomar el valor de la penalizacion inicio.
                                DateTime fechaTemporal = arreglo[contador].fecha;

                                while (tiempoReferencia < fechaTemporal)
                                {
                                    claveValor nuevaClaveValor = new claveValor();
                                    nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                    nuevaClaveValor.valor = 1;

                                    interno.arreglo.Add(nuevaClaveValor);

                                    tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                }

                                tiempoReferencia = fechaTemporal;
                                //aumentar el contador
                                contador++;
                                sentado = true;
                            }
                            else
                            {

                                //si es la interrupcion inicial.
                                if (arreglo[contador].fin)
                                {
                                    //calcular los minutos y agregar los elementos.\

                                    //tomar el valor de la penalizacion inicio.
                                    DateTime fechaTemporal = arreglo[contador].fecha;

                                    while (tiempoReferencia <= fechaTemporal)
                                    {
                                        claveValor nuevaClaveValor = new claveValor();
                                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                                        nuevaClaveValor.valor = 0;

                                        interno.arreglo.Add(nuevaClaveValor);

                                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                                    }

                                    tiempoReferencia = fechaTemporal;
                                    //aumentar el contador
                                    contador++;
                                    sentado = false;
                                }
                            }

                        }
                    }

                }

                if (sentado)
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.D4.fin);

                    while (tiempoReferencia < fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 0;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }

                }
                else
                {
                    DateTime fechaFinal = utilidad.stringToDateTime(elemento.D4.fin);

                    while (tiempoReferencia <= fechaFinal)
                    {
                        claveValor nuevaClaveValor = new claveValor();
                        nuevaClaveValor.clave = tiempoReferencia.ToString("HH:mm");
                        nuevaClaveValor.valor = 1;

                        interno.arreglo.Add(nuevaClaveValor);

                        tiempoReferencia = tiempoReferencia.AddMinutes(1);
                    }
                }

                dataPD.Add(interno);

                //creacion de elemento interno.
            }

            //Agragar el primario al temporal trabajo
            temporal.trabajo.Add(primario);
            temporal.descansos.Add(primarioDescanso);

            //agregar estos datos.


        }

        respuestaFinal.data.Add(temporal);

    }


    // return Results.Ok( respuestaFinal );
    return Results.Ok(respuestaFinal);
});

//ENDPOINT tiempo real --->
app.MapPost("/tiempoReal", async ([FromServices] DataContext dbContext, [FromBody] Recolector recolector) =>
{
    //para esta grafica mandaremos todo de manera seccionada es decir por codigo de grupo y filtrado por fecha y usuario

    DateTime finalDateTime = DateTime.Now;
    string fecha_comparadora = finalDateTime.ToShortDateString();

    Console.WriteLine($"La fecha comparadora es -> {fecha_comparadora}");


    //tomar todos los registros que coincidan con el userName
    IEnumerable<IGrouping<string, Data>> listado = dbContext.Datos.Where(e => e.userName == recolector.nameUser && e.fecha_corta == fecha_comparadora)
                                                    .OrderByDescending(e => e.fecha).GroupBy(e => e.codGrupo);
    Console.WriteLine($"Tamanio del listado -> {listado.Count()}");


    IList<datosG1> datosGrafica = new List<datosG1>();

    conversor utilidad = new conversor();

    //aunque hacemos un foreach lo retornaremos en el primer ciclo.
    foreach (var grupo in listado)
    {
        datosG1 temporalGrafica = new datosG1();

        temporalGrafica.usuario = recolector.nameUser;

        Console.WriteLine($"\n\nSe encontro un grupo....-------------->>>>\n ");

        IEnumerable<Data> dataGroup = grupo.OrderBy(e => e.fecha);

        //agrupar los grupos por numeros de modoros.

        IEnumerable<IGrouping<int, Data>> pomodoros = dataGroup.Where(e => e.numeroPomodoro != -1 && e.numeroPomodoro != 0)
                                                    .OrderBy(e => e.numeroPomodoro).GroupBy(e => e.numeroPomodoro);

        // return Results.Ok(pomodoros);

        IEnumerable<IGrouping<int, Data>> descansos = dataGroup.Where(e => e.numeroDescanso != -1 && e.numeroDescanso != 0)
                                                    .GroupBy(e => e.numeroDescanso);

        bool agregado = false;

        //para agregar pomodoros
        foreach (var g_pom in pomodoros)
        {
            IEnumerable<Data> g_pomOrdenaro = g_pom.OrderBy(e => e.fecha);

            int numPomActual = 0;
            double tiempoAcumulado = 0;
            string inicio = "";
            string ref_tiempo = "";
            string final = "";
            bool agregar = true;
            
            Data pomodoroPivote = new Data();

            foreach (var pom in g_pomOrdenaro)
            {

                if (pom.inicio)
                {
                    if (pom.numeroPomodoro == 1 && pom.inicio)
                    {
                        temporalGrafica.fecha_comparadora = utilidad.stringToDateTime(pom.fecha);
                        temporalGrafica.fecha = pom.fecha;
                        temporalGrafica.fecha_corta = pom.fecha_corta;
                        temporalGrafica.codigoGrupo = pom.codGrupo;
                        temporalGrafica.dia = pom.dia;
                        temporalGrafica.mes = pom.mes;
                        temporalGrafica.tiempoStandar = pom.ts;
                        temporalGrafica.descansoStandar = pom.ds;
                    }

                    tiempoAcumulado = 0;
                    numPomActual = pom.numeroPomodoro;
                    inicio = pom.fecha;
                    ref_tiempo = inicio;
                    agregar = true;
                    agregado = false;
                    pomodoroPivote = pom;
                }
                else if (pom.numeroPomodoro == numPomActual)
                {

                    //significa que puede ser un corte o el final del pomodoro
                    if (pom.fin)
                    {
                        final = pom.fecha;

                        //hacer la suma de tiempo trabajado
                        DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                        DateTime fechaActual = utilidad.stringToDateTime(pom.fecha);

                        TimeSpan duracion = fechaActual - fechaArriba;
                        double minutosTranscurridos = duracion.TotalMinutes;

                        if (agregar)
                        {
                            tiempoAcumulado += minutosTranscurridos;
                        }

                        //ahora a registrar esto.
                        subDatosGrafica1 nuevo = new subDatosGrafica1();

                        nuevo.inicio = inicio;
                        nuevo.fin = final;
                        nuevo.tiempo = tiempoAcumulado;
                        nuevo.penalizacion = pom.ts - tiempoAcumulado;

                        if (pom.numeroPomodoro == 1)
                        {
                            temporalGrafica.P1 = nuevo;
                        }
                        else if (pom.numeroPomodoro == 2)
                        {
                            temporalGrafica.P2 = nuevo;
                        }
                        else if (pom.numeroPomodoro == 3)
                        {
                            temporalGrafica.P3 = nuevo;
                        }
                        else if (pom.numeroPomodoro == 4)
                        {
                            temporalGrafica.P4 = nuevo;
                        }

                        agregado = true;    //si ya fue agregado entonces no necesito hacer el calculo con este dato.
                    }

                    else
                    {
                        //significa que existe un corte donde existe una penalizacion por no trabajar seguido
                        if (agregar)
                        {
                            //como sufrio un corte de flujo de trabajo entonces debe negar la agregacion.
                            agregar = false;

                            //hacemos una suma de lo que lleva por ahora el usuario
                            DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                            DateTime fechaActual = utilidad.stringToDateTime(pom.fecha);

                            TimeSpan duracion = fechaActual - fechaArriba;
                            double minutosTranscurridos = duracion.TotalMinutes;

                            tiempoAcumulado += minutosTranscurridos;

                        }
                        else
                        {
                            //significa que el pana regreso por ende vamos a hacerle la pala :v
                            agregar = true;
                            ref_tiempo = pom.fecha;
                        }
                    }
                }
            }

            //si aca aun no ha sido agregado entonces tengo que ver que onda xd.
            if ( !agregado )
            {
                //aca es donde tengo que ver si esta ante una penalizacion y por ello aun no ha sido agregada la data.

                //primer si es que no esta penalizado

                //entonces le agrego al tiempo de trabajo desde la referencia de tiempo hasta el punto actual
                DateTime fechaPivote = DateTime.Now;


                //DEBUG--------------------
                //hacer la suma de tiempo trabajado
                DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                DateTime fechaActual = DateTime.Now;

                TimeSpan duracion = fechaActual - fechaArriba;
                double minutosTranscurridos = duracion.TotalMinutes;
                if (agregar)
                {
                    tiempoAcumulado += minutosTranscurridos;
                }

                //ahora a registrar esto.
                subDatosGrafica1 nuevo = new subDatosGrafica1();

                nuevo.inicio = inicio;
                nuevo.fin = final;
                nuevo.tiempo = tiempoAcumulado;
                nuevo.penalizacion = pomodoroPivote.ts - tiempoAcumulado;

                if (pomodoroPivote.numeroPomodoro == 1)
                {
                    temporalGrafica.P1 = nuevo;
                }
                else if (pomodoroPivote.numeroPomodoro == 2)
                {
                    temporalGrafica.P2 = nuevo;
                }
                else if (pomodoroPivote.numeroPomodoro == 3)
                {
                    temporalGrafica.P3 = nuevo;
                }
                else if (pomodoroPivote.numeroPomodoro == 4)
                {
                    temporalGrafica.P4 = nuevo;
                }

                agregado = true;

            }

        }

        //para agregar descansos
        foreach (var g_desc in descansos)
        {
            IEnumerable<Data> g_DescOrdenaro = g_desc.OrderBy(e => e.fecha);

            int numDescActual = 0;
            double tiempoAcumulado = 0;
            string inicio = "";
            string ref_tiempo = "";
            string final = "";
            bool agregar = true;

            Data descansoPivote = new Data();

            foreach (var desc in g_DescOrdenaro)
            {
                if (desc.inicio)
                {
                    tiempoAcumulado = 0;
                    numDescActual = desc.numeroDescanso;
                    inicio = desc.fecha;
                    ref_tiempo = inicio;
                    agregar = true;
                    agregado = false;
                    descansoPivote = desc;
                }
                else if (desc.numeroDescanso == numDescActual)
                {

                    //significa que puede ser un corte o el final del pomodoro
                    if (desc.fin)
                    {
                        final = desc.fecha;

                        //hacer la suma de tiempo trabajado
                        DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                        DateTime fechaActual = utilidad.stringToDateTime(desc.fecha);

                        TimeSpan duracion = fechaActual - fechaArriba;
                        double minutosTranscurridos = duracion.TotalMinutes;

                        if (agregar)
                        {
                            tiempoAcumulado += minutosTranscurridos;
                        }

                        //ahora a registrar esto.
                        subDatosGrafica1 nuevo = new subDatosGrafica1();

                        nuevo.inicio = inicio;
                        nuevo.fin = final;
                        nuevo.tiempo = tiempoAcumulado;
                        nuevo.penalizacion = desc.ds - tiempoAcumulado;

                        if (desc.numeroDescanso == 1)
                        {
                            temporalGrafica.D1 = nuevo;
                        }
                        else if (desc.numeroDescanso == 2)
                        {
                            temporalGrafica.D2 = nuevo;
                        }
                        else if (desc.numeroDescanso == 3)
                        {
                            temporalGrafica.D3 = nuevo;
                        }
                        else if (desc.numeroDescanso == 4)
                        {
                            temporalGrafica.D4 = nuevo;
                        }
                        agregado = true;    //si ya fue agregado entonces no necesito hacer el calculo con este dato.

                    }

                    else
                    {
                        //significa que existe un corte donde existe una penalizacion por no trabajar seguido
                        if (agregar)
                        {
                            //como sufrio un corte de flujo de trabajo entonces debe negar la agregacion.
                            agregar = false;

                            //hacemos una suma de lo que lleva por ahora el usuario
                            //hacer la suma de tiempo trabajado
                            DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                            DateTime fechaActual = utilidad.stringToDateTime(desc.fecha);

                            TimeSpan duracion = fechaActual - fechaArriba;
                            double minutosTranscurridos = duracion.TotalMinutes;

                            tiempoAcumulado += minutosTranscurridos;

                        }
                        else
                        {
                            //significa que el pana regreso por ende vamos a hacerle la pala :v
                            agregar = true;
                            ref_tiempo = desc.fecha;
                        }
                        agregado = true;
                    }
                }


            }

            //si aca aun no ha sido agregado entonces tengo que ver que onda xd.
            if ( !agregado )
            {
                //aca es donde tengo que ver si esta ante una penalizacion y por ello aun no ha sido agregada la data.

                //primer si es que no esta penalizado

                //entonces le agrego al tiempo de trabajo desde la referencia de tiempo hasta el punto actual
                DateTime fechaPivote = DateTime.Now;


                //DEBUG--------------------
                //hacer la suma de tiempo trabajado
                DateTime fechaArriba = utilidad.stringToDateTime(ref_tiempo);
                DateTime fechaActual = DateTime.Now;

                TimeSpan duracion = fechaActual - fechaArriba;
                double minutosTranscurridos = duracion.TotalMinutes;
                if (agregar)
                {
                    tiempoAcumulado += minutosTranscurridos;
                }

                //ahora a registrar esto.
                subDatosGrafica1 nuevo = new subDatosGrafica1();

                nuevo.inicio = inicio;
                nuevo.fin = final;
                nuevo.tiempo = tiempoAcumulado;
                nuevo.penalizacion = descansoPivote.ts - tiempoAcumulado;

                if (descansoPivote.numeroDescanso == 1)
                {
                    temporalGrafica.D1 = nuevo;
                }
                else if (descansoPivote.numeroDescanso == 2)
                {
                    temporalGrafica.D2 = nuevo;
                }
                else if (descansoPivote.numeroDescanso == 3)
                {
                    temporalGrafica.D3 = nuevo;
                }
                else if (descansoPivote.numeroDescanso == 4)
                {
                    temporalGrafica.D4 = nuevo;
                }

                agregado = true;

            }

        }


        datosGrafica.Add(temporalGrafica);
    }

    //ordenar por fechas
    IEnumerable<datosG1> respuesta = datosGrafica.OrderBy(e => e.fecha_comparadora);



    return Results.Ok(respuesta);

});

//-----------------------DEBUG------------------------
app.MapGet("/datosUserDEBUG", async ([FromServices] DataContext dbContext) =>
{

    var recolectado = dbContext.Parametros.Count();

    if (recolectado == 0)
    {
        //si no existe entonces se crea uno por defecto
        ParamApp valDefault = new ParamApp();
        valDefault.id = Guid.NewGuid();
        valDefault.userName = "patito";
        valDefault.valPomodoro = 25;
        valDefault.valDescanso = 5;
        valDefault.valDescansoLargo = 15;

        await dbContext.AddAsync(valDefault);
        await dbContext.SaveChangesAsync();
        Results.Ok("Se inserto un nuevo dato");
    }

    IEnumerable<ParamApp> list = dbContext.Parametros;


    return Results.Ok( list.ToArray()[0] );

});

app.MapPost("/agregarRegistroDEBUG", async ([FromServices] DataContext dbContext, [FromBody] Data registro) =>
{
    Console.WriteLine($"NameUser -> { registro.userName }");
    Console.WriteLine($"NumeroPomodoro -> { registro.numeroPomodoro }");
    
    

    return Results.Ok( "Se recibieron los datos" );
});


app.Run();
