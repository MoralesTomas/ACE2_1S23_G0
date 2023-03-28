#include <Tone32.h>
#include "Song.h"
#include <WiFi.h>
#include <HTTPClient.h>
#include <Arduino_JSON.h>

/**
 * @brief Object that represents a phisics pomodoro
 *
 */
class Pomodoro
{
private:
  const char *userName = "";
  double workTime;
  double shortBreakTime;
  double longBreakTime;
  int numberPomodoros = 0;
  int completedPomodoros;
  bool isWorking;
  bool isBreak;
  bool isLongBreak;
  bool isGropuCompleted;
  bool isSitting;
  const char *ssid = "CLARO1_78B977";
  const char *password = "462u2QDobH";
  unsigned long lastTime = 0;

public:
  Pomodoro(double wt, double sbt, double lbt)
  {
    workTime = wt;
    shortBreakTime = sbt;
    longBreakTime = lbt;
    completedPomodoros = 0;
    isWorking = false;
    isLongBreak = false;
  }

  void incrementPomodoros()
  {
    numberPomodoros++;
  }

  bool getIsSitting()
  {
    return isSitting;
  }

  bool setIsSitting(bool stsp)
  {
    isSitting = stsp;
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

  void setAll(double wt, double sbt, double lbt)
  {
    workTime = wt;
    shortBreakTime = sbt;
    longBreakTime = lbt;
  }

  void reset()
  {
    completedPomodoros = 0;
    isWorking = false;
    isLongBreak = false;
  }

  bool grupoCompleto()
  {
    return completedPomodoros == 4;
  }

  void startWork()
  {
    isWorking = true;
    isLongBreak = false;
    isBreak = false;
    httpPOSTStartNewPomodoro();
  }

  void stopWork()
  {
    isWorking = false;
    isLongBreak = false;
    isBreak = false;
    httpPOSTEndPomodoro();
  }

  void startShortBreak()
  {
    startMelody();
    isWorking = false;
    isLongBreak = false;
    isBreak = true;
    completedPomodoros++;
    httpPOSTshortBreak(); 
  }

  void stopShortBreak()
  {
    isWorking = false;
    isLongBreak = false;
    isBreak = false;
  }
  void startLongBreak()
  {
    startMelody();
    isWorking = false;
    isLongBreak = true;
    isBreak = false;
    completedPomodoros++;
  }

  bool isWorkingSession()
  {
    return isWorking;
  }

  bool isBreakSession()
  {
    return isBreak;
  }

  bool isLongBreakSession()
  {
    return isLongBreak;
  }

  String getUserName()
  {
    return userName;
  }

  double getWorkTime()
  {
    return workTime;
  }

  double setWorkTime(double wt)
  {
    workTime = wt;
  }

  void stopShortWork()
  {
    isWorking = false;
    isLongBreak = false;
    isBreak = false;
  }

  void stopLongWork()
  {
    isWorking = false;
    isLongBreak = false;
  }

  double getShortBreakTime()
  {
    return shortBreakTime;
  }

  double setShortBreakTime(double sbt)
  {
    shortBreakTime = sbt;
  }

  void stopBreak()
  {
    isWorking = true;
  }

  double getLongBreakTime()
  {
    return longBreakTime;
  }

  double setLongBreakTime(double lbt)
  {
    longBreakTime = lbt;
  }

  void stopLongBreak()
  {
    isWorking = true;
  }

  double getCompletedPomodoros()
  {
    return completedPomodoros;
  }

  void soundMelody()
  {
    startMelody();
  }

  // Pasar bool a string los booleanso de la clase
  String getIsWorking()
  {
    if (isWorking)
    {
      return "true";
    }
    else
    {
      return "false";
    }
  }

  String getIsBreak()
  {
    if (isBreak)
    {
      return "true";
    }
    else
    {
      return "false";
    }
  }

  String getIsLongBreak()
  {
    if (isLongBreak)
    {
      return "true";
    }
    else
    {
      return "false";
    }
  }

  String getIsSittingString()
  {
    if (isSitting)
    {
      return "true";
    }
    else
    {
      return "false";
    }
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
    lastTime = millis();
  }

  String httpPOSTRequest(String serverName, String payload)
  {
    if (WiFi.status() == WL_CONNECTED)
    { // Check WiFi connection status

      HTTPClient http;
      String datos_a_enviar = ("{\"userName\":\"Amborguesa\",\"numeroPomodoro\":1}");

      http.begin("http://192.168.1.34:5000/agregarRegistro"); // Indicamos el destino
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

    delay(2000);
  }



  /**
   * @brief Get the Data User of the API and save in data global User
   *  Enpoint: http://localhost:5012/datosUser
   */
  void httpgetDataUserAPI()
  {
    String IP_tomas = "http://192.168.1.34:5000";
    String serverNameTomas = "";
    String serverName = IP_tomas + "/datosUserArduino"; // Tomas
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

      userName = myObject["userName"];
      workTime = myObject["valPomodoro"];
      shortBreakTime = myObject["valDescanso"];
      longBreakTime = myObject["valDescansoLargo"];

      Serial.println(myObject["userName"]);
      // Serial.print
      Serial.println("Datos de usuario ingresados");
      /* code */
    }
  }

  void httpgetName()
  {
    String IP_tomas = "http://192.168.1.34:5000";
    String serverNameTomas = "";
    String serverName = IP_tomas + "/datosUserArduino"; // Tomas
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

      userName = myObject["userName"];
      Serial.println(myObject["userName"]);
      // Serial.print
      Serial.println("Datos de usuario ingresados");
      /* code */
    }
  }

  void debugJson()
  {
    String payload = "[{\"userName\":\"Amborguesa\",\"numeroPomodoro\":1}]";

    JSONVar myArray = JSON.parse(payload);
    if (JSON.typeof(myArray) == "undefined")
    {
      Serial.println("Parsing input failed!");
      return;
    }

    Serial.print("JSON.typeof(myArray) = ");
    Serial.println(JSON.typeof(myArray)); // prints: array

    Serial.print("myArray.length() = ");
    Serial.println(myArray.length());
    Serial.println();

    Serial.print("JSON.typeof(myArray[0]) = ");
    Serial.println(JSON.typeof(myArray[0]));

    Serial.print("myArray[0] = ");
    Serial.println(myArray[0]);
    Serial.println();
    /*
    Serial.println(myObject["userName"]);
    Serial.println(myObject["numeroPomodoro"]);
    */
    Serial.println("Datos impresos");
    delay(1000);
  }

  /**
 * @brief Update the Data User AP  object
 * Ejemplo de consumo:
 *
 *      http://localhost:5012/agregarRegistro
 *
 *   Ejemplo de datos a enviar al endpoint :
 *
 *       {
            "descansoLargo":false
            "descansoNormal":false,
            "inicio":true,
            "fin":false,
            "numeroPomodoro":1,
            "numeroDescanso": -1,
            "userName":"usuario1",
            "sentado":true
        }
 *
 */
  void httppostRegisterDataPomodoro()
  {
    // Inicio

    // Actualizar numero de pomodoro
    // si se para mandar sentado = false

    // Fin de grupo
  }


  void httpPOSTStartNewPomodoro()
  {
    String dataRegister = "{\"descansoLargo\":" + getIsLongBreak() +",\"descansoNormal\":" + getIsBreak() +",\"inicio\":true,\"fin\":false,\"numeroPomodoro\":" + String(numberPomodoros) + ",\"numeroDescanso\": -1,\"userName\":" + String(userName) + ",\"sentado\": "+ String(getIsSittingString()) +"}";
    Serial.println(dataRegister);
    httpPOSTRequest("", dataRegister);
  }

  void httpPOSTEndPomodoro()
  {
    String dataRegister = "{\"descansoLargo\":" + getIsLongBreak() +",\"descansoNormal\":" + getIsBreak() +",\"inicio\":false,\"fin\":true,\"numeroPomodoro\":" + String(numberPomodoros) + ",\"numeroDescanso\": -1,\"userName\":" + String(userName) + ",\"sentado\": "+ String(getIsSittingString()) +"}";
    Serial.println(dataRegister);
    httpPOSTRequest("", dataRegister);
  }



  void httpPOSTupdate()
  {
    String dataRegister = "{\"descansoLargo\":" + getIsLongBreak() +",\"descansoNormal\":" + getIsBreak() +",\"inicio\":false,\"fin\":false,\"numeroPomodoro\":" + String(numberPomodoros) + ",\"numeroDescanso\": -1,\"userName\":" + String(userName) + ",\"sentado\": "+ String(getIsSittingString()) +"}";
    Serial.println(dataRegister);
    httpPOSTRequest("", dataRegister);
  }

  void httpPOSTfin()
  {
    String dataRegister = "{\"descansoLargo\":" + getIsLongBreak() +",\"descansoNormal\":" + getIsBreak() +",\"inicio\":false,\"fin\":true,\"numeroPomodoro\":" + String(numberPomodoros) + ",\"numeroDescanso\": -1,\"userName\":" + String(userName) + ",\"sentado\": "+ String(getIsSittingString()) +"}";
    Serial.println(dataRegister);
    httpPOSTRequest("", dataRegister);
  }

  void httpPOSTshortBreak()
  {
    String dataRegister = "{\"descansoLargo\":" + getIsLongBreak() +",\"descansoNormal\":" + getIsBreak() +",\"inicio\":false,\"fin\":false,\"numeroPomodoro\":" + String(numberPomodoros) + ",\"numeroDescanso\": -1,\"userName\":" + String(userName) + ",\"sentado\": "+ String(getIsSittingString()) +"}";
    Serial.println(dataRegister);
    httpPOSTRequest("", dataRegister);
  }
};
