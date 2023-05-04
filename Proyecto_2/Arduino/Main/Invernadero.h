#include <WiFi.h>
#include <HTTPClient.h>
#include <Arduino_JSON.h>

class Invernadero 
{
  private:
    int humedadExterna;
    int humedadInterna;
    int temperaturaExterna;
    int temperaturaInterna;
    int porcentajeAguaDisponible;
    bool estadoRiego;
    int tiempoRiego;

  public:
    Invernadero(int he)
    {
      humedadExterna = he;
      humedadInterna = 0;
      temperaturaExterna = 0;
      temperaturaInterna = 0;
      porcentajeAguaDisponible = 0;
      estadoRiego = false;
      tiempoRiego = 0;
    }

    void setHumedadExterna(int hE)
    {
      humedadExterna = hE;
    }
};
