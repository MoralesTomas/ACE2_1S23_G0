using System.IO.Ports;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using API.Models;

namespace EJEMPLO_API.Utilidades
{
    public class Serial
    {
        public SerialPort Port;
        public bool isClosed = false;

        public string localHost { get; set; }

        public string puerto { get; set; }
        
        

        public Serial()
        {
            Port = new SerialPort();

            //mandando el puerto.

            Port.PortName = "COM5";
            Port.BaudRate = 9600;
            //Port.ReadTimeout = 1500;
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

                    /* data.Id = Guid.NewGuid();
                    data.Fecha = DateTime.Now.ToString();
                    data.Calor = Double.Parse(arreglo[0].Trim());
                    data.HumedadRelativa = Double.Parse(arreglo[1].Trim());
                    data.HumedadAbsoluta = Double.Parse(arreglo[2].Trim());
                    data.Velocidad = Double.Parse(arreglo[3].Trim());
                    data.Direccion = arreglo[4].Trim();
                    data.Presion = arreglo[5].Trim(); */

                    data.numeroPomodoro = Convert.ToInt32(arreglo[0]);
                    data.numeroDescanso = Convert.ToInt32(arreglo[1]);
                    if (arreglo[2] == "1") data.sentado = true;
                    else data.sentado = false;
                    // data.sentado = Convert.ToInt32(arreglo[2]);
                    
                    //


                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    using (var client = new HttpClient())
                    {
                        //http://localhost:5090/api/dato
                        var response = client.PostAsync($"{localHost}/agregarRegistro", content).Result;

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