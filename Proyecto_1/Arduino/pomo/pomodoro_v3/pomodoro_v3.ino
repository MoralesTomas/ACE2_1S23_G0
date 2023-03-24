#include <Wire.h>
#include <LiquidCrystal_I2C.h>

#define BUTTON_PIN 2
#define WORK_TIME 25*60*1000
#define BREAK_TIME 5*60*1000
#define LONG_BREAK_TIME 15*60*1000

LiquidCrystal_I2C lcd(0x27, 16, 2);

unsigned long startTime = 0;
unsigned long timeLeft;
bool isWorking = false;
bool estatusBoton = false;

void setup() {
Serial.begin(9600);
  Wire.begin();
  pinMode(BUTTON_PIN, INPUT);
  lcd.init();
  lcd.backlight();
  lcd.setCursor(0, 0);
  lcd.print("Pomodoro Timer");
}

void loop() {
    if (digitalRead(BUTTON_PIN) == HIGH) {
         if (!isWorking) {
            lcd.setCursor(0, 1);
            lcd.print("Working iniciado        ");
            delay(8000);
            timeLeft = WORK_TIME; 
            isWorking = true;
            
        }
        startTime = millis();
        lcd.clear();
        lcd.setCursor(0, 0);
        lcd.print("Tiempo iniciado ");
        delay(4000);
    }

    unsigned long elapsedTime = millis() - startTime;
    Serial.print("Elapsed ");
    Serial.println(elapsedTime); 
    timeLeft = (timeLeft - elapsedTime);

    Serial.print("Time ");
    Serial.println(timeLeft);
    

    long minutes = convertirMillisMinutos(timeLeft);
    long seconds = convertirMillisSegundos(timeLeft);

    Serial.print("Minutos ");
    Serial.println(minutes);

    Serial.print("Segundos ");
    Serial.println(seconds);

    if (isWorking)
    {
        lcd.setCursor(0, 1);
        lcd.print("Workinga ");
        lcd.print(minutes );
        lcd.print(":");
        lcd.print(seconds);
    }
    else
    {
        lcd.setCursor(0, 1);
        lcd.print("Break ");
        lcd.print(minutes);
        lcd.print(":");
        lcd.print(seconds);
    }
    
    /*
    lcd.setCursor(0, 1);
    lcd.print(isWorking ? "Working " : "Break   ");
    lcd.print(minutes < 10 ? "0" : "");
    lcd.print(minutes);
    lcd.print(":");
    lcd.print(seconds < 10 ? "0" : "");
    lcd.print(seconds);
    */
    delay(200);
}

unsigned long convertirMillisMinutos(unsigned long millis) {
    unsigned long minutos = millis / 1000 / 60;
    return minutos;
}

unsigned long convertirMillisSegundos(unsigned long millis) {
    unsigned long minutos = convertirMillisMinutos(millis);
    millis -= minutos * 60 * 1000;
    unsigned long segundos = millis / 1000;
    return segundos;
}