using System;

namespace API.Utilidades
{
    public class conversor
    {

        public conversor()
        {

        }

        public DateTime stringToDateTime(string fecha)
        {
            DateTime respuesta = DateTime.Now;
            try
            {
                respuesta = DateTime.ParseExact(fecha, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception a)
            {
                try
                {
                    respuesta = DateTime.ParseExact(fecha, "dd/MM/yyyy H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                }
                catch (Exception b)
                {
                    try
                    {
                        respuesta = DateTime.ParseExact(fecha, "d/MM/yyyy H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                    }
                    catch (Exception c)
                    {
                        try
                        {
                            respuesta = DateTime.ParseExact(fecha, "d/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                        }
                        catch (Exception d)
                        {
                            Console.WriteLine($"Ocurrio un error en el conversor con la fecha stringToDateTime -> {fecha}");

                        }

                    }

                }
            }
            return respuesta;
        }

        public DateTime stringToDateTimeSinHorario(string fecha)
        {
            DateTime respuesta = DateTime.Now;
            try
            {
                respuesta = DateTime.ParseExact(fecha, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception a)
            {
                Console.WriteLine($"Ocurrio un error en el conversor con la fecha toDateTimeSinHorario -> {fecha}");
            }
            return respuesta;

        }

        public DateTime stringToDateTimeConHorioCero(string fecha)
        {
            fecha += " 00:00:00";

            DateTime respuesta = DateTime.Now;
            try
            {
                respuesta = DateTime.ParseExact(fecha, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception a)
            {
                Console.WriteLine($"Ocurrio un error en el conversor con la fecha stringToDateTimeConHorioCero -> {fecha}");
            }
            return respuesta;

        }
    }

}