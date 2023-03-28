#include <Wire.h>
#include <LiquidCrystal_I2C.h>

// Variables que manejara la clase pomodoro
int time_work = 25;       // time of work
int time_break = 5;      // time of break
int time_long_break = 15; // time of long break

// Variables of the Timer
int minutes = 0;
int sec = 0;

bool RUN = true;

LiquidCrystal_I2C lcd(0x27, 16, 2);

void setup() {
  // Configuracion del LCD
  Wire.begin();
  lcd.init();
  lcd.clear();
  lcd.backlight();
  
  print_LCD_firstLine("POMODORO");
  print_LCD_secondLine("WELCOME");
  delay(1000);

}

void loop() {
  // put your main code here, to run repeatedly:
  sec = time_work;
  RUN = true;

  print_LCD_firstLine("Iniciando");
  print_LCD_secondLine("Tiempo de trabajo");
  lcd.clear();
  startTimerWork();
  delay(3000);  
}

void startTimerWork()
{
  //pomodoro.startWork();
  minutes = time_work;
  RUN = true;

  print_LCD_firstLine("Iniciando");
  print_LCD_secondLine("Tiempo de trabajo");
  lcd.clear();
  delay(3000);

  while (RUN)
  {
    /*
    if (!isSitting())
    {
      pomodoro.stopWork();
    } else {
      pomodoro.startWork();
    }*/

    sec = sec - 1;
    delay(1000);
    if (sec == -1)
    {
      sec = 59;
      minutes = minutes - 1;
    }
   
    lcd.setCursor(0, 1);
    lcd.print("****************");
    lcd.setCursor(6, 0);
    
    if (minutes <= 9)
    {
      lcd.print('0');
    }
    lcd.print(minutes);
    lcd.print(':');
    if (sec <= 9)
    {
      lcd.print('0');
    }
    lcd.print(sec);

    if (minutes == 0 && sec == 0)
    {
      lcd.setCursor(4, 0);
      RUN = false;
      //INIT();
    }
  }
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
