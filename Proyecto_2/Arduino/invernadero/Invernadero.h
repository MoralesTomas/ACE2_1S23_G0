#include <WiFi.h>
#include <HTTPClient.h>
#include <Arduino_JSON.h>


class Invernadero
{
private:
  int humedadExterna;
  int humedadInterna;
  float temperaturaExterna;
  float temperaturaInterna;
  int porcentajeAguaDisponible;
  bool estadoRiego;
  int tiempoRiego;
  const char *ssid = "CLARO1_78B977";
  const char *password = "462u2QDobH";
  String url = "http://192.168.1.15:5000";

public:
  Invernadero(int hE, int hI, float tE, float tI, int pAD, bool eR, int tR)
  {
    humedadExterna = hE;
    humedadInterna = hI;
    temperaturaExterna = tE;
    temperaturaInterna = tI;
    porcentajeAguaDisponible = pAD;
    estadoRiego = eR;
    tiempoRiego = tR;
  }

  void setupwifi()
  {
    WiFi.mode(WIFI_STA);
    WiFi.begin(ssid, password);

    Serial.println("Connecting to WiFi");

    while (WiFi.status() != WL_CONNECTED)
    {
      Serial.print(".");
      delay(100);
    }

    Serial.println("Connected to the WiFi network");
    Serial.println("Local ESP8266 IP: ");
    Serial.println(WiFi.localIP());
  }  

  void setAll(int hE, int hI, float tE, float tI, int pAD, bool eR, int tR)
  {
    humedadExterna = hE;
    humedadInterna = hI;
    temperaturaExterna = tE;
    temperaturaInterna = tI;
    porcentajeAguaDisponible = pAD;
    estadoRiego = eR;
    tiempoRiego = tR;
  }

 void imprimir()
 {
    Serial.println("Humedad Externa: " + String(humedadExterna));
    Serial.println("Humedad Interna: " + String(humedadInterna));
    Serial.println("Temperatura Externa: " + String(temperaturaExterna));
    Serial.println("Temperatura Interna: " + String(temperaturaInterna));
    Serial.println("Porcentaje Agua Disponible: " + String(porcentajeAguaDisponible));
    Serial.println("Estado Riego: " + String(estadoRiego));
    Serial.println("Tiempo Riego: " + String(tiempoRiego));
    
 }

 void httpPostData() {
    String estadoRiegoString = "false";
    
    if (estadoRiego)
    {
      estadoRiegoString = "true";
    }
    String urlRegistro = url + "/agregarRegistro";
    String dataGreenHouse = "{\"valorHumedadExterna\":" + String(humedadExterna)
                            + ",\"valorHumedadInterna\":" + String(humedadInterna)
                            + ",\"valorTemperaturaExterna\":" + String(temperaturaExterna)
                            + ",\"valorTemperaturaInterna\":" + String(temperaturaInterna)
                            + ",\"estadoRiego\":" + String(estadoRiegoString)
                            + ",\"porcentajeAguaDisponible\":" + String(porcentajeAguaDisponible)
                            + ",\"capacidadTanque\": 0"
                            + ",\"tiempoRiego\":" + String(tiempoRiego)
                            + "}";
    Serial.println(dataGreenHouse);
    Serial.println(urlRegistro);
    // urlRegistro

    httpPOSTRequest(urlRegistro, dataGreenHouse);
  }

    String httpPOSTRequest(String serverName, String payload) {
    if (WiFi.status() == WL_CONNECTED) {  // Check WiFi connection status

      HTTPClient http;

      http.begin(serverName);                              // Indicamos el destino
      http.addHeader("Content-Type", "application/json");  // Preparamos el header text/plain si solo vamos a enviar texto plano sin un paradigma llave:valor.
      Serial.print("VOY A ENVIAR DATOS");
      int codigo_respuesta = http.POST(payload);  // Enviamos el post pasándole, los datos que queremos enviar. (esta función nos devuelve un código que guardamos en un int)

      if (codigo_respuesta > 0) {
        Serial.println("Código HTTP ► " + String(codigo_respuesta));  // Print return code

        if (codigo_respuesta == 200) {
          String cuerpo_respuesta = http.getString();
          Serial.println("El servidor respondió ▼ ");
          Serial.println(cuerpo_respuesta);
        }
      } else {

        Serial.print("Error enviando POST, código: ");
        Serial.println(codigo_respuesta);
      }

      http.end();  // libero recursos
    } else {

      Serial.println("Error en la conexión WIFI");
    }

    delay(1000);
  }

};