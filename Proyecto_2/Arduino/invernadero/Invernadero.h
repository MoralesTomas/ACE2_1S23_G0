#include "HardwareSerial.h"
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
    Serial.println("ME MUERO---------------------------------");
    humedadExterna = hE;
    humedadInterna = hI;
    temperaturaExterna = tE;
    temperaturaInterna = tI;
    porcentajeAguaDisponible = pAD;
    estadoRiego = eR;
    tiempoRiego = tR;
  }

  bool getEstadoRiego()
  {
    if (estadoRiego)
    {
      Serial.println("SOY VERDADERO");
    }
    return estadoRiego;
  }

  int getTiempoRiego()
  {
    return tiempoRiego;
  }

  void setEstadoRiego(bool eR)
  {
    estadoRiego = eR;
  }

  int setTiempoRiego(int tR)
  {
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

 void httpPostDataConEstado() {
    String estadoRiegoString = "false";
    
    if (estadoRiego)
    {
      estadoRiegoString = "true";
    }
    String urlRegistro = url + "/agregarRegistroConEstado";
    String dataGreenHouse = "{\"valorHumedadExterna\":" + String(humedadExterna)
                            + ",\"valorHumedadInterna\":" + String(humedadInterna)
                            + ",\"valorTemperaturaExterna\":" + String(temperaturaExterna)
                            + ",\"valorTemperaturaInterna\":" + String(temperaturaInterna)
                            + ",\"estadoRiego\":" + String(estadoRiegoString)
                            + ",\"porcentajeAguaDisponible\":" + String(porcentajeAguaDisponible)
                            + ",\"capacidadTanque\": 1"
                            + ",\"tiempoRiego\":" + String(tiempoRiego)
                            + "}";

    Serial.println(dataGreenHouse);
    Serial.println(urlRegistro);
    // urlRegistro

    httpPOSTRequest(urlRegistro, dataGreenHouse);
  }

 void httpPostData() {
    String estadoRiegoString = "false";
    
    if (estadoRiego)
    {
      estadoRiegoString = "true";
    }
    String urlRegistro = url + "/agregarRegistroConEstado";
    String dataGreenHouse = "{\"valorHumedadExterna\":" + String(humedadExterna)
                            + ",\"valorHumedadInterna\":" + String(humedadInterna)
                            + ",\"valorTemperaturaExterna\":" + String(temperaturaExterna)
                            + ",\"valorTemperaturaInterna\":" + String(temperaturaInterna)
                            + ",\"estadoRiego\":" + String(estadoRiegoString)
                            + ",\"porcentajeAguaDisponible\":" + String(porcentajeAguaDisponible)
                            + ",\"capacidadTanque\": 1"
                            + ",\"tiempoRiego\":" + String(tiempoRiego)
                            + "}";

                            /*{
  "valorHumedadExterna": 0,
  "valorHumedadInterna": 0,
  "valorTemperaturaExterna": 0,
  "valorTemperaturaInterna": 0,
  "porcentajeAguaDisponible": 0,
  "estadoRiego": true,
  "capacidadTanque": 0,
  "tiempoRiego": 0,
  "fecha": "string"
}*/

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

    delay(10);
  }

  String httpGETRequest(String serverName)
  {
    String payload = "";
    if (WiFi.status() == WL_CONNECTED)
    {
      HTTPClient http;

      String serverPath = serverName;

      http.begin(serverPath.c_str());

      int httpResponseCode = http.GET();
      Serial.println(httpResponseCode);
      Serial.println("httpResponseCode");

      if (httpResponseCode > 0)
      {
        Serial.println("HTTP Response code: ");
        Serial.println(httpResponseCode);

        payload = http.getString();
        delay(500);
        Serial.println(payload);
        delay(100);
        return payload;
      }
      else
      {
        Serial.println("Error on HTTP request");
        Serial.println(httpResponseCode);
        payload = "Error";
      }
      http.end();
    }
    else
    {
      Serial.println("Error in WiFi connection");
      payload = "Error";
    }
   
  }


  void httpGetEstadoBottonApp()
  {
    Serial.println("PIDIENDO EL ESTADO DEL BOTON DE LA APP -----------");
    String serverName = url + "/verEstadoArduino"; // Tomas
    // JSON de la api de datos de usuario
    String payload = httpGETRequest(serverName);

    if (payload != "Error")
    {
      JSONVar myObject = JSON.parse(payload);
      if (JSON.typeof(myObject) == "undefined")
      {
        Serial.println("Parsing input failed!");
        return;
      }

      Serial.println("Estado del boton de la app: de myObject");
      Serial.println(myObject["estadoRiego"]);

      Serial.println("Estado del boton de la app: de estadoRiego");
      Serial.println(estadoRiego);

      const char *estadoBottonApptemporal = myObject["estadoRiego"];
      String estadoB = estadoBottonApptemporal;
      // Coverting a JSON object to a String
      
      
      
      const char *tiempoRiegoTemporal = myObject["tiempoRiego"];
      
      Serial.println("ESTADO BOTTON APP");
      Serial.println(estadoB);
 
      
      if (estadoB == "True") 
      {
        estadoRiego = true;
        Serial.println("Voy a REGAR --------");
      } else {
        estadoRiego = false;
        tiempoRiego = (String(tiempoRiegoTemporal)).toInt();
      }
      
      
      
      /*
      {
  "estadoRiego": false,
  "tiempoRiego": 5
}
      */
      

    }
  }

};