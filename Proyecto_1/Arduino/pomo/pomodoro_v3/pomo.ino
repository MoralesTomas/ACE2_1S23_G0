#include <LiquidCrystal.h>
#include<EEPROM.h>
const int rs = 12, en = 11, d4 = 5, d5 = 4, d6 = 3, d7 = 6;
LiquidCrystal lcd(rs, en, d4, d5, d6, d7);
const int stsp = A0;
const int inc = A1;
const int dec = A2;
const int set = A3;
const int buzz = 9;
const int relay = 8;
int hrs = 0;
int Min = 0;
int sec = 0;
unsigned int check_val = 50;
int add_chk = 0;
int add_hrs = 1;
int add_min = 2;
bool RUN = true;
bool min_flag = true;
bool hrs_flag = true;
void setup()
{
  lcd.begin(16, 2);
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Mr. Screw Driver");
  lcd.setCursor(0, 1);
  lcd.print("COUNTDOWN TIMER");
  pinMode(stsp, INPUT_PULLUP);
  pinMode(inc, INPUT_PULLUP);
  pinMode(dec, INPUT_PULLUP);
  pinMode(set, INPUT_PULLUP);
  pinMode(buzz, OUTPUT);
  pinMode(relay, OUTPUT);
  digitalWrite(relay, LOW); 
  digitalWrite(buzz, LOW);
  if (EEPROM.read(add_chk) != check_val)
  {
    EEPROM.write(add_chk, check_val);
    EEPROM.write(add_hrs, 0);
    EEPROM.write(add_min, 1);
  }
  else
  {
    hrs = EEPROM.read(add_hrs);
    Min = EEPROM.read(add_min);
  }
  delay(1500);
  INIT();
}

void loop()
{
  if (digitalRead(stsp) == LOW)
  {
    lcd.clear();
    delay(250);
    RUN = true;
    while (RUN)
    {
      if (digitalRead(stsp) == LOW)
      {
        delay(1000);
        if (digitalRead(stsp) == LOW)
        {
          digitalWrite(relay, LOW); 
          lcd.clear();
          lcd.setCursor(0, 0);
          lcd.print("  TIMER STOPPED");
          lcd.setCursor(0, 1);
          lcd.print("----------------");
          delay(2000);
          RUN = false;
          INIT();
          break;
        }
      }
      digitalWrite(relay, HIGH); 
      sec = sec - 1;
      delay(1000);
      if (sec == -1)
      {
        sec = 59;
        Min = Min - 1;
      }
      if (Min == -1)
      {
        Min = 59;
        hrs = hrs - 1;
      }
      if (hrs == -1) hrs = 0;
      lcd.setCursor(0, 1);
      lcd.print("****************");
      lcd.setCursor(4, 0);
      if (hrs <= 9)
      {
        lcd.print('0');
      }
      lcd.print(hrs);
      lcd.print(':');
      if (Min <= 9)
      {
        lcd.print('0');
      }
      lcd.print(Min);
      lcd.print(':');
      if (sec <= 9)
      {
        lcd.print('0');
      }
      lcd.print(sec);
      if (hrs == 0 && Min == 0 && sec == 0)
      {
        digitalWrite(relay, LOW); 
        lcd.setCursor(4, 0);
        RUN = false;
        for (int i = 0; i < 20; i++)
        {
          digitalWrite(buzz, HIGH);
          delay(100);
          digitalWrite(buzz, LOW);
          delay(100);
        }
        INIT();
      }
    }
  }
  if (digitalRead(set) == LOW)
  {
    delay(500);
    while (min_flag)
    {
      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print("SET MINUTE: ");
      lcd.print(Min);
      delay(100);
      if (digitalRead(inc) == LOW)
      {
        Min = Min + 1;
        if (Min >= 60) Min = 0;
        delay(100);
      }
      if (digitalRead(dec) == LOW)
      {
        Min = Min - 1;
        if (Min <= -1) Min = 0;
        delay(100);
      }
      if (digitalRead(set) == LOW)
      {
        min_flag = false;
        delay(250);
      }
    }
    while (hrs_flag)
    {
      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print("SET HOUR: ");
      lcd.print(hrs);
      delay(100);
      if (digitalRead(inc) == LOW)
      {
        hrs = hrs + 1;
        if (hrs > 23) hrs = 0;
        delay(100);
      }
      if (digitalRead(dec) == LOW)
      {
        hrs = hrs - 1;
        if (hrs <= -1) hrs = 0;
        delay(100);
      }
      if (digitalRead(set) == LOW)
      {
        hrs_flag = false;
        delay(250);
      }
    }
    if (hrs == 0 && Min == 0)
    {
      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print("  INVAID TIME");
      delay(2000);
    }
    else
    {
      EEPROM.write(add_hrs, hrs);
      EEPROM.write(add_min, Min);
    }
    INIT();
  }
}

void INIT()
{
  hrs = EEPROM.read(add_hrs);
  Min = EEPROM.read(add_min);
  sec = 0;
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Start / Set Time");
  lcd.setCursor(4, 1);
  if (hrs <= 9)
  {
    lcd.print('0');
  }
  lcd.print(hrs);
  lcd.print(':');
  if (Min <= 9)
  {
    lcd.print('0');
  }
  lcd.print(Min);
  lcd.print(':');
  if (sec <= 9)
  {
    lcd.print('0');
  }
  lcd.print(sec);
  min_flag = true;
  hrs_flag = true;
  delay(500);
}