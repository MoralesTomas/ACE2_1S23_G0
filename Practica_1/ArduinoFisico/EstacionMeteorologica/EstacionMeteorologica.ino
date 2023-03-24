#include <DHT.h>
#define DHTPin 2
#define DHTType DHT11
DHT dht11(DHTPin, DHTType);

#include <Wire.h>		// incluye libreria de bus I2C
#include <Adafruit_Sensor.h>	// incluye librerias para sensor BMP280
#include <Adafruit_BMP280.h>

Adafruit_BMP280 bmp;		// crea objeto con nombre bmp

double TEMPERATURA;		// variable para almacenar valor de temperatura
double PRESION;			// variable para almacenar valor de presion atmosferica

#define HallPinSur12 12
#define HallPinSurEste11 11
#define HallPinEste10 10
#define HallPinNorEste9 9
#define HallPinNorte8 8
#define HallPinNorOeste6 6
#define HallPinOeste5 5
#define HallPinSurOeste4 4
#define HallPinVelocidad3 3

unsigned long previuosMillis = 0;
long interval = 2000;

const int HALLPin = 3;
const int LEDPin = 13;

float t, t1, t2;
float windSpeed;

bool state = true;
bool count = true;

int S1 = 0;
int S2 = 0;
int S3 = 0;
int S4 = 0;
int S5 = 0;
int S6 = 0;
int S7 = 0;
int S8 = 0;
int S9 = 0;

String dataVelocidad = "";
String dataDHT = "";
String direccion = "";

void setup() {
  Serial.begin(9600);
  dht11.begin();
  pinMode(LEDPin, OUTPUT);
  pinMode(HALLPin, INPUT);

   if ( !bmp.begin() ) {				// si falla la comunicacion con el sensor mostrar
    digitalWrite(13, HIGH);
   // Serial.println("No jalo");
    while (1);					// mediante bucle infinito
  } else {
    //Serial.println("Si jalo");
  }
  delay(2000);
}

void loop() {
  
   if (state == 0) {
    if (count == 0) {
      readSensor();
      direccion = windDirection();

      if (S9 == 1) {
        t2 = millis();
        t = t2 - t1;
        t = t / 1000;
        windSpeed = (2 * 3.1416 * 0.00007 * 3600) / t;

        dataVelocidad =  windSpeed; 
        //Serial.println();
        //Serial.print("Wind: ");
        //Serial.print(windSpeed);
        //Serial.print(" kmh");
        //Serial.println();
        count = true;
      }
    }
    else {
      readSensor();
      direccion = windDirection();

      if (S9 == 0) {
        t1 = millis();
        count = false;
      }
    }
  } else {
    readSensor();
    direccion = windDirection();

    //   Serialprint();
    if (S9 == 1) {
      state = 0;
    }

    dataVelocidad = "0";
    //Serial.print("Wind: ");
    //Serial.print("0 ");
    //Serial.print("kmh");
    //Serial.print("Direction:");
    //Serial.println();
  }

  unsigned long currentMillis = millis();

  if((currentMillis - previuosMillis) >= interval) {
    TEMPERATURA = bmp.readTemperature();		// almacena en variable el valor de temperatura
    PRESION = bmp.readPressure() / 100;		// almacena en variable el valor de presion divido
						                              // por 100 para covertirlo a hectopascales
    previuosMillis = currentMillis;

    double tempC =dht11.readTemperature();
    double humedadRelativa = dht11.readHumidity();
    double milibaresPresion = PRESION;

    double humedadAbsoluta = humedad_absoluta(humedadRelativa, tempC, PRESION);
    dataDHT = String (tempC) + "," + humedadRelativa + "," + humedadAbsoluta + "," + dataVelocidad + "," + direccion + "," + milibaresPresion;    
    Serial.println(dataDHT);



  }

}

void readSensor() {
  S1 = digitalRead(HallPinNorte8);
  S2 = digitalRead(HallPinNorEste9);
  S3 = digitalRead(HallPinEste10);
  S4 = digitalRead(HallPinSurEste11);
  S5 = digitalRead(HallPinSur12);
  S6 = digitalRead(HallPinSurOeste4);
  S7 = digitalRead(HallPinOeste5);
  S8 = digitalRead(HallPinNorOeste6);
  S9 = !digitalRead(HallPinVelocidad3);

}

String windDirection() {
  if (S1 == 0) {
    return "Norte";
    //Serial.print("0");
    //Serial.print("N   ");
  } else if (S2 == 0) {
    return "NorEste";
    //Serial.print("45");
    //Serial.print("NE ");
  } else if (S3 == 0) {
    return "Este";
    //Serial.print("90");
    //Serial.print("E  ");
  } else if (S4 == 0) {
    return "SurEste";
    //Serial.print("135");
    //Serial.print("SE");
  } else if (S5 == 0) {
    return "Sur";
    //Serial.print("180");
    //Serial.print("S ");
  } else if (S6 == 0) {
    return "SurOeste";
    //Serial.print("225");
    //Serial.print("SW");
  } else if (S7 == 0) {
    return "Oeste";
    //Serial.print("270");
    //Serial.print("W ");
  } else if (S8 == 0) {
    return "NorOeste";
    //Serial.print("315");
    //Serial.print("NW");
  }
}


double humedad_absoluta(double HR, double T, double P_hPa) {
  double e_s = 6.112 * exp((17.67 * T) / (T + 243.5));
  double e = HR / 100.0 * e_s;
  return e / 1000.0;
}
