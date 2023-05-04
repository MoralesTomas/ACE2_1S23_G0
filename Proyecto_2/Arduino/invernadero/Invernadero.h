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
    String dataGreenHouse = "{\"humedadExterna\":" + String(humedadExterna)
                            + ",\"humedadInterna\":" + String(humedadInterna)
                            + ",\"temperaturaExterna\":" + String(temperaturaExterna)
                            + ",\"temperaturaInterna\":" + String(temperaturaInterna)
                            + ",\"estadoRiego\":" + String(estadoRiego)
                            + ",\"porcentajeAguaDisponible\":" + String(porcentajeAguaDisponible)
                            + "}";
    Serial.println(dataGreenHouse);
    Serial.println("");
  }

};