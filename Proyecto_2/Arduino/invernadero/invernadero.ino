#include <OneWire.h>
#include <DallasTemperature.h>
#include "DHTesp.h"
#include <LiquidCrystal_I2C.h>
#include "Invernadero.h"

// Pines
#define DHTpin 15 // D15 of ESP32 DevKit
// GPIO where the DS18B20 is connected to
const int oneWireBus = 4;

const int Analog_channel_pin = A0;

const int trigPin = 5;
const int echoPin = 18;
int relayPin = 2;

// Variable for ultrasonic sensor
// define sound speed in cm/uS
#define SOUND_SPEED 0.034
#define ALTURA_TANQUE 12 // cm
bool estadoRiego = false;
int tiempoRiego = 0;
long duration;
float distanceCm;
float distanceInch;

int ADC_VALUE = 0;
int humedadInterna = 0;
int porcentajeAguaDisponible = 0;
DHTesp dht;
//  Class Definition
Invernadero invernadero(50, 60, 25.5, 26.5, 80, true, 10);
// Setup a oneWire instance to communicate with any OneWire devices
OneWire oneWire(oneWireBus);

// Pass our oneWire reference to Dallas Temperature sensor
DallasTemperature sensors(&oneWire);

LiquidCrystal_I2C lcd(0x27, 16, 2);

void setup()
{
  // put your setup code here, to run once:
  Serial.begin(115200);
  invernadero.setupwifi();
  
  pinMode(relayPin, OUTPUT);
  digitalWrite(relayPin, LOW);
  Wire.begin();
  lcd.init();
  lcd.clear();
  lcd.backlight();
  print_LCD_firstLine("IniciandoASFDA...");
  delay(10000);

  // Start the DS18B20 sensor
  sensors.begin();

  // Autodetect is not working reliable, don't use the following line
  // dht.setup(17);

  // use this instead:
  dht.setup(DHTpin, DHTesp::DHT11); // for DHT11 Connect DHT sensor to GPIO 17
  // dht.setup(DHTpin, DHTesp::DHT22); //for DHT22 Connect DHT sensor to GPIO 17
  pinMode(trigPin, OUTPUT); // Sets the trigPin as an Output
  pinMode(echoPin, INPUT);  // Sets the echoPin as an Input
  lcd.clear();
}

void loop()
{
  print_LCD_firstLine("Iniciando...");
  //digitalWrite(relayPin, HIGH);
  delay(4000);
  //digitalWrite(relayPin, LOW);

  // put your main code here, to run repeatedly:
  ADC_VALUE = analogRead(Analog_channel_pin);
  humedadInterna = convertToPercent(ADC_VALUE);

  sensors.requestTemperatures();
  int temperaturaInterna = sensors.getTempCByIndex(0);

  delay(dht.getMinimumSamplingPeriod());
  int humedadExterna = dht.getHumidity();
  int temperaturaExterna = dht.getTemperature();

  calculoPorcentajeAguaDisponible();
  invernadero.setAll(humedadExterna, humedadInterna, temperaturaExterna, temperaturaInterna, porcentajeAguaDisponible, estadoRiego, tiempoRiego);

  invernadero.httpPostData();
  delay(1000);
}

void calculoPorcentajeAguaDisponible()
{
  // Clears the trigPin
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  // Sets the trigPin on HIGH state for 10 micro seconds
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);

  // Reads the echoPin, returns the sound wave travel time in microseconds
  duration = pulseIn(echoPin, HIGH);

  // Calculate the distance
  distanceCm = duration * SOUND_SPEED / 2;
  porcentajeAguaDisponible = (distanceCm * 100) / ALTURA_TANQUE;
}

int convertToPercent(int value)
{
  return 100 - ((value * 100) / 4095);
}

void print_LCD_firstLine(String text)
{
  lcd.setCursor(0, 0);
  lcd.print(text);
  delay(2000);
  lcd.clear();
}