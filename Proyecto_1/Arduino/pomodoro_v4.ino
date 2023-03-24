#include <Wire.h>
#include <LiquidCrystal_I2C.h>

#define BUTTON_PIN 2
#define SEAT_PIN A0
#define WORK_TIME 25*60*1000
#define BREAK_TIME 5*60*1000
#define LONG_BREAK_TIME 15*60*1000
#define POMODORO_COUNT 4

LiquidCrystal_I2C lcd(0x27, 16, 2); // Dirección de la pantalla y tamaño

unsigned long startTime;
unsigned long timeLeft;
bool isWorking = false;
bool isSitting = false;
int pomodoroCount = 0;
bool takeLongBreak = false;

void setup() {
  Wire.begin();
  pinMode(BUTTON_PIN, INPUT_PULLUP);
  pinMode(SEAT_PIN, INPUT);
  lcd.init();
  lcd.backlight();
  lcd.setCursor(0, 0);
  lcd.print("Pomodoro Timer");
}

void loop() {
  if (digitalRead(BUTTON_PIN) == LOW && isSitting) {
    if (!isWorking) {
      timeLeft = WORK_TIME;
      isWorking = true;
      lcd.setCursor(0, 1);
      lcd.print("Working         ");
    }
    startTime = millis();
  }

  isSitting = analogRead(SEAT_PIN) > 500;

  if (!isSitting) {
    startTime += millis() - startTime;
  }

  unsigned long elapsedTime = millis() - startTime;
  timeLeft = max(0, (long)(timeLeft - elapsedTime));

  int minutes = timeLeft / 60000;
  int seconds = (timeLeft / 1000) % 60;

  lcd.setCursor(0, 1);
  if (isWorking) {
    lcd.print("Working ");
    lcd.print(pomodoroCount + 1);
    lcd.print("/");
    lcd.print(POMODORO_COUNT);
  } else {
    lcd.print("Break   ");
  }
  lcd.print(minutes < 10 ? "0" : "");
  lcd.print(minutes);
  lcd.print(":");
  lcd.print(seconds < 10 ? "0" : "");
  lcd.print(seconds);

  if (timeLeft == 0) {
    isWorking = !isWorking;
    timeLeft = isWorking ? WORK_TIME : (takeLongBreak ? LONG_BREAK_TIME : BREAK_TIME);
    lcd.setCursor(0, 1);
    if (isWorking) {
      pomodoroCount++;
      lcd.print("Working ");
      lcd.print(pomodoroCount + 1);
      lcd.print("/");
      lcd.print(POMODORO_COUNT);
    } else {
      lcd.print("Break           ");
      if (pomodoroCount == POMODORO_COUNT - 1) {
        takeLongBreak = true;
      }
      if (pomodoroCount == POMODORO_COUNT) {
        pomodoroCount = 0;
        takeLongBreak = false;
      }
    }
    tone(3, 440, 500);
    delay(500);
  }

  delay(100);
}
