
double data; 

void setup() {
  Serial.begin(115200);
  pinMode(A0, INPUT);
}

void loop() {
  data = analogRead(A0);
  Serial.println(data);
  delay(400);
}

int convertToPercent(int value) {
  return (value * 100) / 4095;
}