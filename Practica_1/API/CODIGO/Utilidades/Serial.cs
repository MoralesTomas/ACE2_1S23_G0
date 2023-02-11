using System.IO.Ports;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using proyectoEF.Models;

namespace EJEMPLO_API.Utilidades
{
    public class Serial
    {
        public SerialPort Port;
        public bool isClosed = false;

        public string localHost { get; set; }

        public Serial()
        {
            Port = new SerialPort();

            //mandando el puerto.

            Port.PortName = "COM1";
            Port.BaudRate = 9600;
            Port.ReadTimeout = 1500;
            Port.Open();
        }

        public void EscuchaSerail()
        {
            while (!isClosed)
            {
                try
                {
                    String cadena = Port.ReadLine();

                    var arreglo = cadena.Split(",");

                    Data data = new Data();

                    data.Id = Guid.NewGuid();
                    data.Fecha = DateTime.Now.ToString();
                    data.Tipo = arreglo[0].Trim();
                    data.Datos = (arreglo[1].Trim());

                    //


                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    using (var client = new HttpClient())
                    {
                        //http://localhost:5090/api/dato
                        var response = client.PostAsync($"{localHost}/api/dato", content).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Se inserto un dato.");
                        }
                        else
                        {
                            Console.WriteLine("Ocurrio un error en la API");
                        }
                    }

                    Console.WriteLine($"{cadena}");

                }
                catch (Exception a)
                {
                    Console.WriteLine($"error");
                    Console.WriteLine($"{a.Message}");
                }
            }
        }

        ~Serial()
        {
            Console.WriteLine($"Destructor de la clase");
        }


    }
}