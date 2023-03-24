// Definir el pin del potenciómetro
int potPin = A0;

void setup() {
  // Inicializar el puerto serial
  Serial.begin(9600);
  // Configurar el pin del potenciómetro como entrada
  pinMode(potPin, INPUT);
}

void loop() {
  // Leer el valor del potenciómetro
  int potValue = analogRead(potPin);

  // Imprimir el valor del potenciómetro en el puerto serial
  Serial.print("Valor del potenciómetro: ");
  Serial.println(potValue);

  // Esperar 100ms antes de leer el potenciómetro de nuevo
  delay(100);
}
