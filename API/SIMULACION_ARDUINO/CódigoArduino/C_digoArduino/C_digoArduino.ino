String tipos[4] = {"Luz","Humedad","Calor","Viento"};

void setup() {
  randomSeed(analogRead(0));
  Serial.begin(9600);
}

void loop() {
  int pos = random(4);

  int randomNum1 = random(101);
  Serial.println(tipos[pos]+","+String(randomNum1));
  delay(1000);
}

