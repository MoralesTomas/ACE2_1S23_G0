/**
 * @file pomodoro.ino
 * @author Ormandy Rony (ormandyrony@ieee.org)
 * @brief 
 * @version 0.1
 * @date 2023-03-27
 * 
 * @copyright Copyright (c) 2023
 * 
 */
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include<EEPROM.h>

const int stsp = 2; // start stopa
const int inc = 3; // increment
const int dec = 4; // decrement
const int set = 5; // set
const int buzz = 9;
const int relay = 8;
int hrs = 0;
int Min = 0; // time of work
int time_break = 0; // time of break
int time_long_break = 0; // time of long break
int sec = 0;
unsigned int check_val = 50;
int add_chk = 0;
int add_hrs = 1;
int add_min = 2;
bool RUN = true;
bool min_flag = true;
bool hrs_flag = true;

bool flag = true;

//
int potPin = A0;
int potValue = 0;

LiquidCrystal_I2C lcd(0x27, 16, 2);

void setup()
{
  Wire.begin();
  lcd.init();
  
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Mr. Rony");
  lcd.setCursor(0, 1);
  lcd.print("COUNTDOWN TIMER");
  pinMode(stsp, INPUT_PULLUP);
  pinMode(inc, INPUT);
  pinMode(dec, INPUT);
  pinMode(set, INPUT_PULLUP);
  pinMode(buzz, OUTPUT);
  pinMode(relay, OUTPUT);
  pinMode(potPin, INPUT);
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
// Edicion de tiempo de trabajo
  if (flag) {
    Min = configuracionTiempo("Work Time: ");
    delay(50);
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("tiempo seteado");
    lcd.setCursor(0, 1);
    lcd.print(Min);
    delay(500);
    flag = true;
  }
 
// Edicion timepo de descanso corto
  if (flag) {
    time_break = configuracionTiempo("Break Time: ");
    delay(50);
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("tiempo break");
    delay(500);
    flag = true;
  }


// Edicion timepo de descanso largo
  if(flag) {
    time_long_break = configuracionTiempo("Long Break Time: ");
    delay(50);
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("tiempo seteado");
    delay(200);
    flag = false;
    
  }


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
 
}


int configuracionTiempo(String tiempoModificar) {
  int potValue = analogRead(potPin);
  int tiempo = 0;
  while (min_flag)
  {
    potValue = analogRead(potPin);

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print(tiempoModificar);
    
    tiempo = map(potValue, 0, 1023, 1, 46);
    delay(50);
    lcd.print(tiempo);
    delay(300);

    if (digitalRead(set) == LOW)
    {
      tiempo = map(potValue, 0, 1023, 1, 46);
      delay(50);
      min_flag = false;
      delay(500);
      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print(tiempoModificar);
      lcd.print(tiempo);
      delay(300);
      lcd.setCursor(0, 0);
      lcd.print(tiempoModificar);
      lcd.print(tiempo);
      delay(300);
      lcd.setCursor(0, 1);
      lcd.print("Saved");
      lcd.clear();
      
    }
    
  }
  min_flag = true;
  return tiempo;   
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