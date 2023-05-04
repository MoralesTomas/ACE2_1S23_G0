class Invernadero {
private:
  int humedadExterna;
  int humedadInterna;
  int temperaturaExterna;
  int temperaturaInterna;
  int porcentajeAguaDisponible;
  bool estadoRiego;
  int tiempoRiego;

public:
  String url = "http://192.168.1.34:5000/api/greenhouse";
  Invernadero(int  hE) {
    humedadExterna = hE;
    humedadInterna = 0;
    temperaturaExterna = 0;
    temperaturaInterna = 0;
    porcentajeAguaDisponible = 0;
    estadoRiego = false;
    tiempoRiego = 0;
  }

  void setHumedadExterna(int hE) {
    humedadExterna = hE;
  }
  
};
