#include <Wire.h>
#include <LiquidCrystal_I2C.h>

#define BUTTON_PIN 2
#define WORK_TIME 25*60*1000
#define BREAK_TIME 5*60*1000

LiquidCrystal_I2C lcd(0x27, 16, 2);

unsigned long startTime;
unsigned long timeLeft;
bool isWorking = false;

void setup() {
  Wire.begin();
  pinMode(BUTTON_PIN, INPUT);
  lcd.init();
  lcd.backlight();
  lcd.setCursor(0, 0);
  lcd.print("Pomodoro Timer");
}

void loop() {
  if (digitalRead(BUTTON_PIN) == LOW) {
    lcd.clear();
      lcd.setCursor(0, 0);
  lcd.print("off");


    if (!isWorking) {
      timeLeft = WORK_TIME;
      isWorking = true;
      lcd.setCursor(0, 1);
      lcd.print("Workinga         ");
    }
    startTime = millis();
  } else {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("on");
  }

  unsigned long elapsedTime = millis() - startTime;
  timeLeft = max(0, (long)(timeLeft - elapsedTime));

  int minutes = timeLeft / 60000;
  int seconds = (timeLeft / 1000) % 60;

  lcd.setCursor(0, 1);
  lcd.print(isWorking ? "Working " : "Break   ");
  lcd.print(minutes < 10 ? "0" : "");
  lcd.print(minutes);
  lcd.print(":");
  lcd.print(seconds < 10 ? "0" : "");
  lcd.print(seconds);

  if (timeLeft == 0) {
    isWorking = !isWorking;
    timeLeft = isWorking ? WORK_TIME : BREAK_TIME;
    lcd.setCursor(0, 1);
    lcd.print(isWorking ? "Working         " : "Break           ");
    tone(3, 440, 500);
    delay(500);
  }

  delay(100);
}
