#define LED    5
#define BUTTON 4

uint8_t stateLED = 1;

  void setup() {
      pinMode(LED, OUTPUT);
      pinMode(BUTTON,INPUT_PULLUP);
  }

  void loop() {

     if(digitalRead(BUTTON) == LOW){
      digitalWrite(LED,HIGH);
    } else {
      digitalWrite(LED, LOW);
    }
  }