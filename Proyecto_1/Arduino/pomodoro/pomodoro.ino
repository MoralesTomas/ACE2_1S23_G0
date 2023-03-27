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
#include <EEPROM.h>
#include "Pomodoro.h"

// Define the pins 
const int stsp = 2; // start stop button asiento
const int set = 3;  // set time button

// Variables que manejara la clase pomodoro
int time_work = 25;       // time of work
int time_break = 5;      // time of break
int time_long_break = 15; // time of long break

// Variables of the Timer
int min = 0;
int sec = 0;

unsigned int check_val = 50;
int add_chk = 0;
int add_hrs = 1;
int add_min = 2;
bool RUN = true;
bool min_flag = true;
bool hrs_flag = true;

bool flag = true;

// Variables para el manejo del potenciometro que sirven  para la configuracion de los tiempos
int potPin = A0;
int potValue = 0;

LiquidCrystal_I2C lcd(0x27, 16, 2);
Pomodoro pomodoro(time_work, time_break, time_long_break);

void setup()
{
  // Configuracion del LCD
  Wire.begin();
  lcd.init();
  lcd.clear();
  lcd.backlight();
  
  print_LCD_firstLine("POMODORO");
  print_LCD_secondLine("WELCOME");
  delay(1000);
  pinMode(stsp, INPUT_PULLUP);
  pinMode(set, INPUT_PULLUP);
  pinMode(potPin, INPUT);

  eepromSetup();
}

void loop()
{
  configuracionGrupoPomodoro();
  pomodoro.setAll(time_work, time_break, time_long_break);

  // Esperar a que se siente
  while (!isSitting())
  {
    lcd.clear();
    print_LCD_firstLine("Sientese para iniciar");
    delay(500);
  }

  if (isSitting())
  {
    // Detener hasta que se cumplan los 4 pomodoros
    while (!pomodoro.grupoCompleto())
    {
      startTimerWork();
      // Inica el timer de trabajo

      if (!pomodoro.grupoCompleto())
      {
        startTimerShortBreak();
        delay(500);
      }
      lcd.clear();
    }

    startTimerLongBrake();
    delay(500);
    lcd.clear();
  }
  
  delay(500);
}

void eepromSetup()
{
  if (EEPROM.read(add_chk) != check_val)
  {
    EEPROM.write(add_chk, check_val);
    EEPROM.write(add_hrs, 0);
    EEPROM.write(add_min, 1);
  }
  else
  {
    min = EEPROM.read(add_min);
  }
  delay(1500);
  INIT();
}


void INIT()
{
  min = EEPROM.read(add_min);
  sec = 0;
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Start");
  lcd.setCursor(4, 1);
  
  if (min <= 9)
  {
    lcd.print('0');
  }
  lcd.print(min);
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

/**
 * @brief Funcion que inicializa los tiempos de lo 4 pomodoros
 *  estos son llamados un grupo de pomodoro.
 *
 */
void configuracionGrupoPomodoro()
{
  // Edicion de tiempo de trabajo
  if (flag)
  {
    time_work = configuracionTiempo("Work Time: ");
    delay(50);
    lcd.clear();
    print_LCD_firstLine("Work Time Saved:");
    lcd.setCursor(0, 1);
    lcd.print(time_work);
    delay(3000);
    lcd.clear();
    flag = true;
  }

  // Edicion timepo de descanso corto
  if (flag)
  {
    time_break = configuracionTiempo("Break Time: ");
    delay(50);
    print_LCD_firstLine("Break Time Saved:");
    lcd.setCursor(0, 1);
    lcd.print(time_break);
    delay(2000);
    lcd.clear();
    flag = true;
  }

  // Edicion timepo de descanso largo
  if (flag)
  {
    time_long_break = configuracionTiempo("Long Break Time: ");
    delay(50);
    print_LCD_firstLine("Long Break Time Saved:");
    lcd.setCursor(0, 1);
    lcd.print(time_long_break);
    delay(2000);
    lcd.clear();    
    flag = true;
  }
}


int configuracionTiempo(String tiempoModificar)
{
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
    lcd.setCursor(0, 1);
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

bool isSitting()
{
  return digitalRead(stsp) == LOW;
}

void startTimerWork()
{
  pomodoro.startWork();
  min = pomodoro.getWorkTime();
  RUN = true;

  print_LCD_firstLine("Iniciando");
  print_LCD_secondLine("Tiempo de trabajo");
  lcd.clear();
  delay(3000);

  while (RUN)
  {
    if (!isSitting())
    {
      pomodoro.stopWork();
    } else {
      pomodoro.startWork();
    }

    sec = sec - 1;
    delay(1000);
    if (sec == -1)
    {
      sec = 59;
      min = min - 1;
    }
   
    lcd.setCursor(0, 1);
    lcd.print("****************");
    lcd.setCursor(6, 0);
    
    if (min <= 9)
    {
      lcd.print('0');
    }
    lcd.print(min);
    lcd.print(':');
    if (sec <= 9)
    {
      lcd.print('0');
    }
    lcd.print(sec);

    if (min == 0 && sec == 0)
    {
      lcd.setCursor(4, 0);
      RUN = false;
      //INIT();
    }
  }
  lcd.clear();
}


void startTimerShortBreak()
{
  pomodoro.startShortBreak();
  min = pomodoro.getShortBreakTime();
  RUN = true;

  print_LCD_firstLine("Iniciando");
  print_LCD_secondLine("Tiempo de trabajo");
  lcd.clear();
  delay(3000);

  while (RUN)
  {
    if (isSitting())
    {
      pomodoro.stopBreak();
    } else {
      // Avisar que ya dejo de sentarse
    }

    sec = sec - 1;
    delay(1000);
    if (sec == -1)
    {
      sec = 59;
      min = min - 1;
    }
   
    lcd.setCursor(0, 1);
    lcd.print("** Short Break **");
    lcd.setCursor(6, 0);
    
    if (min <= 9)
    {
      lcd.print('0');
    }
    lcd.print(min);
    lcd.print(':');
    if (sec <= 9)
    {
      lcd.print('0');
    }
    lcd.print(sec);

    if (min == 0 && sec == 0)
    {
      lcd.setCursor(4, 0);
      RUN = false;
      //INIT();
    }
  }
  pomodoro.soundMelody();
  lcd.clear();
}


void startTimerLongBrake()
{
  pomodoro.startLongBreak();
  min = pomodoro.getLongBreakTime();
  RUN = true;

  print_LCD_firstLine("Iniciando");
  print_LCD_secondLine("Tiempo de trabajo");
  lcd.clear();
  delay(3000);

  while (RUN)
  {
    if (!isSitting())
    {
      pomodoro.stopLongWork();
    } else {
      // Avisar que ya dejo de sentarse
    }

    sec = sec - 1;
    delay(1000);
    if (sec == -1)
    {
      sec = 59;
      min = min - 1;
    }
   
    lcd.setCursor(0, 1);
    lcd.print("** Long Break **");
    lcd.setCursor(6, 0);
 
    if (min <= 9)
    {
      lcd.print('0');
    }
    lcd.print(min);
    lcd.print(':');
    if (sec <= 9)
    {
      lcd.print('0');
    }
    lcd.print(sec);

    if (min == 0 && sec == 0)
    {
      lcd.setCursor(4, 0);
      RUN = false;
      //INIT();
    }
  }
  pomodoro.soundMelody();
  lcd.clear();
}


void print_LCD_firstLine(String text)
{
  lcd.setCursor(0, 0);
  lcd.print(text);
}

void print_LCD_secondLine(String text)
{
  lcd.setCursor(0, 1);
  lcd.print(text);
}