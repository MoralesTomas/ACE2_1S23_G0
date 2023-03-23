using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using API.Context;

using API.Models;
using API.Utilidades;

// DateTime fecha = DateTime.Now;

// Console.WriteLine($"fecha { fecha }");
// Console.WriteLine($"Dia={ fecha.Day}");
// Console.WriteLine($"mes={ fecha.Month}");
// Console.WriteLine($"anio={ fecha.Year}");
// Console.WriteLine($"hora={ fecha.Hour}");
// Console.WriteLine($"minuto={ fecha.Minute}");
// Console.WriteLine($"segundo={ fecha.Second}");

// Console.WriteLine($"Fecha to string -> {fecha.ToString()}");
// Console.WriteLine($"Fecha corta ->{ fecha.ToShortDateString().ToString() }");


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

var app = builder.Build();

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
        if (recolector.nuevoValPomodoro < 1 || recolector.nuevoValPomodoro > 45 )
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
        if (registro.fecha == "")
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
app.MapGet("/obtenerFechasUsuario", async ([FromServices] DataContext dbContext, [FromBody] Recolector recolector) =>
{
    IEnumerable<string> fechas = dbContext.Datos.Where(e => e.userName == recolector.nameUser).Select(e => e.fecha_corta).Distinct();

    parametrosFiltro respuesta = new parametrosFiltro();
    respuesta.fechas = fechas;

    return Results.Ok( respuesta );
});

app.MapGet("/obtenerGrupos", async ([FromServices] DataContext dbContext, [FromBody] Recolector recolector) =>
{
    conversor utilidad = new conversor();

    //tomar todos los registros que coincidan con el userName
    IList<Recolector> list = dbContext.Datos.Where(e => e.userName == recolector.nameUser
                                                            && e.fecha_corta == recolector.fecha1
                                                            && e.numeroPomodoro == 1 && e.inicio)
                                                    .Select(e => new Recolector(){ codigoGrupo = e.codGrupo, 
                                                         fecha1 = e.fecha } ).Distinct().ToList();
    foreach (var item in list)
    {
        item.stringToDateTime();
    }
    

    list = list.OrderBy( e => e.fechaComparadora ).ToList();

    IList<string> listadoTmp = new List<string>();
    foreach (var item in list)
    {
        listadoTmp.Add( item.codigoGrupo );
    }

    IEnumerable<string> listado = listadoTmp;

    parametrosFiltro respuesta = new parametrosFiltro();
    respuesta.grupos = listado;

    return Results.Ok( respuesta );

});

//necesito el nameUser, y una fecha -> retorna todos los datos para la grafica uno.
app.MapGet("/grafica1", async ([FromServices] DataContext dbContext, [FromBody] Recolector recolector) =>
{
    //para esta grafica mandaremos todo de manera seccionada es decir por codigo de grupo y filtrado por fecha y usuario

    //tomar todos los registros que coincidan con el userName
    IEnumerable<IGrouping<string, Data>> listado = dbContext.Datos.Where(e => e.userName == recolector.nameUser && e.fecha_corta == recolector.fecha1)
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
app.MapGet("/grafica2", async ([FromServices] DataContext dbContext, [FromBody] Recolector recolector) =>
{
    conversor util = new conversor();
    DateTime limiteInferior = util.stringToDateTimeSinHorario( recolector.fecha1 );
    DateTime limiteSuperior = util.stringToDateTimeSinHorario( recolector.fecha2 );
    //primero ver que si se cumpla
    //para esta grafica mandaremos todo de manera seccionada es decir por codigo de grupo y filtrado por fecha y usuario

    //tomar todos los registros que coincidan con el userName
    IEnumerable<IGrouping<string, Data>> listado = dbContext.Datos.Where(e => e.userName == recolector.nameUser && 
                                                    e.fecha_comparadora >= limiteInferior && limiteSuperior >= e.fecha_comparadora )
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

app.MapGet("/grafica_4_5_6", async ([FromServices] DataContext dbContext, [FromBody] Recolector recolector) =>
{
    conversor util = new conversor();
    DateTime limiteInferior = util.stringToDateTimeSinHorario( recolector.fecha1 );
    DateTime limiteSuperior = util.stringToDateTimeSinHorario( recolector.fecha2 );
    //primero ver que si se cumpla
    //para esta grafica mandaremos todo de manera seccionada es decir por codigo de grupo y filtrado por fecha y usuario

    //tomar todos los registros que coincidan con el userName
    IEnumerable<IGrouping<string, Data>> listado = dbContext.Datos.Where(e => e.userName == recolector.nameUser && 
                                                    e.fecha_comparadora >= limiteInferior && limiteSuperior >= e.fecha_comparadora )
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


app.Run();
