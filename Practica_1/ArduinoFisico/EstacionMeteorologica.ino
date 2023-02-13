#include <DHT.h>
#define DHTPin 2
#define DHTType DHT11
DHT dht11(DHTPin, DHTType);

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

void setup() {
  Serial.begin(9600);
  dht11.begin();
  pinMode(LEDPin, OUTPUT);
  pinMode(HALLPin, INPUT);
  Serial.println("Si soy");
}

void loop() {
  
   if (state == 0) {
    if (count == 0) {
      readSensor();
      windDirection();

      if (S9 == 1) {
        t2 = millis();
        t = t2 - t1;
        t = t / 1000;
        windSpeed = (2 * 3.1416 * 0.00007 * 3600) / t;
        Serial.println();
        Serial.print("Wind: ");
        Serial.print(windSpeed);
        Serial.print(" kmh");
        Serial.println();
        count = true;
      }
    }
    else {
      readSensor();
      windDirection();

      if (S9 == 0) {
        t1 = millis();
        count = false;
      }
    }
  } else {
    readSensor();
    windDirection();

    //   Serialprint();
    if (S9 == 1) {
      state = 0;
    }
    Serial.print("Wind: ");
    Serial.print("0 ");
    Serial.print("kmh");
    Serial.print("Direction:");
    Serial.println();
  }

  unsigned long currentMillis = millis();

  if((currentMillis - previuosMillis) >= interval) {
    
    previuosMillis = currentMillis;

    int tempC =dht11.readTemperature();
    int hum = dht11.readHumidity();

    Serial.println("Temperatura: ");
    Serial.print(tempC);
    Serial.print("C /");
    Serial.print(" Humedad: ");
    Serial.print(hum);

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

void windDirection() {
  if (S1 == 0) {
    Serial.print("0");
    Serial.print("N   ");
  } else if (S2 == 0) {
    Serial.print("45");
    Serial.print("NE ");
  } else if (S3 == 0) {
    Serial.print("90");
    Serial.print("E  ");
  } else if (S4 == 0) {
    Serial.print("135");
    Serial.print("SE");
  } else if (S5 == 0) {
    Serial.print("180");
    Serial.print("S ");
  } else if (S6 == 0) {
    Serial.print("225");
    Serial.print("SW");
  } else if (S7 == 0) {
    Serial.print("270");
    Serial.print("W ");
  } else if (S8 == 0) {
    Serial.print("315");
    Serial.print("NW");
  }
}


