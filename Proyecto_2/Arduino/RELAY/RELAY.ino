int relayPin= 2;

void setup() {
  // put your setup code here, to run once:
  pinMode(relayPin, OUTPUT);
  digitalWrite(relayPin, LOW);
  delay(4000);
}

void loop() {
  // put your main code here, to run repeatedly:
  digitalWrite(relayPin, HIGH);
  delay(4000); 
}
