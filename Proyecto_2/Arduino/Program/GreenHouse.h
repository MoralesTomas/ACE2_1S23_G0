#include <WiFi.h>
#include <HTTPClient.h>
#include <Arduino_JSON.h>

class GreenHouse
{
    private:
        int humedadExterna;
        int humedadInterna;
        int temperaturaExterna;
        int temperaturaInterna;
        int porcentajeAguaDisponible;
        bool estadoRiego;

    public:
        String url = "http://192.168.1.34:5000/api/greenhouse";
        GreenHouse();

        void setHumedadExterna(int humedadExterna)
        {
            this->humedadExterna = humedadExterna;
        }
        void setHumedadInterna(int humedadInterna)
        {
            this->humedadInterna = humedadInterna;
        }
        void setTemperaturaExterna(int temperaturaExterna)
        {
            this->temperaturaExterna = temperaturaExterna;
        }

        void setTemperaturaInterna(int temperaturaInterna)
        {
            this->temperaturaInterna = temperaturaInterna;
        }

        void setPorcentajeAguaDisponible(int porcentajeAguaDisponible)
        {
            this->porcentajeAguaDisponible = porcentajeAguaDisponible;
        }

        void setEstadoRiego(bool estadoRiego)
        {
            this->estadoRiego = estadoRiego;
        }

        int getHumedadExterna()
        {
            return this->humedadExterna;
        }

        int getHumedadInterna()
        {
            return this->humedadInterna;
        }

        int getTemperaturaExterna()
        {
            return this->temperaturaExterna;
        }
        int getTemperaturaInterna()
        {
            return this->temperaturaInterna;
        }

        int getPorcentajeAguaDisponible()
        {
            return this->porcentajeAguaDisponible;
        }

        /*
        *   Funcion que envia los datos al servidor con la estructura de JSON
        *   {
                "humedadExterna": 15, // medida en porcentaje
                "humedadInterna" : 12.4, // esto representa la humedad del suelo, medida en porcentaje
                "temperaturaExterna" : 29, // esto representa la termperatura del ambiente 
                "temperaturaInterna" : 24, // medida en grados centigrados
                "estadoRiego": true, // esto indica si se esta regando el sistema ahora.
                "porcentajeAguaDisponible" : 50 // esto representa el porcentaje de agua disponible
            }
        */
        void httpPostData()
        {
            String dataGreenHouse = "{\"humedadExterna\":" + String(this->humedadExterna) 
                                        + ",\"humedadInterna\":" + String(this->humedadInterna) 
                                        + ",\"temperaturaExterna\":" + String(this->temperaturaExterna) 
                                        + ",\"temperaturaInterna\":" + String(this->temperaturaInterna) 
                                        + ",\"estadoRiego\":" + String(this->estadoRiego) 
                                        + ",\"porcentajeAguaDisponible\":" + String(this->porcentajeAguaDisponible) 
                                        + "}";
            Serial.println(dataGreenHouse);
            httpPOSTRequest(this->url, dataGreenHouse);

        }

        String httpPOSTRequest(String serverName, String payload)
        {
            if (WiFi.status() == WL_CONNECTED)
            { // Check WiFi connection status

                HTTPClient http;

                http.begin(serverName); // Indicamos el destino
                http.addHeader("Content-Type", "application/json");          // Preparamos el header text/plain si solo vamos a enviar texto plano sin un paradigma llave:valor.
                Serial.print("VOY A ENVIAR DATOS");
                int codigo_respuesta = http.POST(payload); // Enviamos el post pasándole, los datos que queremos enviar. (esta función nos devuelve un código que guardamos en un int)

            if (codigo_respuesta > 0)
            {
                Serial.println("Código HTTP ► " + String(codigo_respuesta)); // Print return code

                if (codigo_respuesta == 200)
                {
                String cuerpo_respuesta = http.getString();
                Serial.println("El servidor respondió ▼ ");
                Serial.println(cuerpo_respuesta);
                }
            }
            else
            {

                Serial.print("Error enviando POST, código: ");
                Serial.println(codigo_respuesta);
            }

            http.end(); // libero recursos
            }
            else
            {

            Serial.println("Error en la conexión WIFI");
            }

            delay(1000);
        }
}
