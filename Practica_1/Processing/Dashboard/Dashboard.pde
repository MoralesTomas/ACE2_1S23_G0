PImage fondo;
PImage t1;
PImage t2;
PImage t3;

PImage d1;
PImage d2;
PImage d3;
PImage d4;
PImage d5;

PImage h1;
PImage h2;
PImage h3;
PImage h4;

PImage c1;
PImage c2;
PImage c3;
PImage c4;

void setup(){
  size(1634, 980);
  fondo = loadImage("fondo3.jpg");
  t1 = loadImage("T1.png");
  t2 = loadImage("T2.png");
  t3 = loadImage("T3.png");
  
  d1 = loadImage("D1.png");
  d2 = loadImage("D2.png");
  d3 = loadImage("D3.png");
  d4 = loadImage("D4.png");
  d5 = loadImage("D5.png");
  
  h1 = loadImage("H1.png");
  h2 = loadImage("H2.png");
  h3 = loadImage("H3.png");
  h4 = loadImage("H4.png");
  
  c1 = loadImage("C1.png");
  c2 = loadImage("C2.png");
  c3 = loadImage("C3.png");
  c4 = loadImage("C4.png");
  
 background(fondo);
 
 
}

void draw(){
  JSONObject data = obtenerdata(); 
  background(fondo);
  stroke (0,0,0); //color borde
  strokeWeight (5); //grosor
  fill (150, 152, 154); // relleno
  
  rect (50,100,500,440, 28);//rectángulo T_Interior
  rect (570,100,500,440, 28);//rectángulo T_Exterior
  rect (1090,100,500,440, 28);//rectángulo Lum
  rect (50,630,500,300, 28);//rectángulo Humedad
  rect (570,630,500,300, 28);//rectángulo CO2
  rect (1090,630,500,300, 28);//rectángulo CO2
  
  textSize(50);
  fill(255, 255, 255);
  text("Velocidad", 70, 80);
  text("Temperatura", 590, 80);
  text("Direccion", 1250, 80);
  text("Humedad Absoluta", 72, 610);
  text("Humedad Relativa", 590, 610);
  text("Presion", 1250, 610);
  
  textSize(100);
  fill (255,255,255);
  
  //Temperatura
  text(data.getFloat("calor")+" C", 620, 350);
  if (data.getFloat("calor") < 20){
    image(t1, 870, 150,150,350);
  }else if (data.getFloat("calor") >= 20 && data.getFloat("calor") <= 30){
    image(t2, 870, 150,150,350);
  }else if (data.getFloat("calor") > 30){
    image(t3, 870, 150,150,350);
  }
  
  //Velocidad
  text(data.getFloat("velocidad"), 95, 300);
  text("Km/h", 100, 400);
  if (data.getFloat("velocidad") < 4){
    image(c1, 350, 150,150,350);
  }else if (data.getFloat("velocidad") >= 4 && data.getFloat("velocidad") <= 7){
    image(c2, 350, 150,150,350);
  }else if (data.getFloat("velocidad") > 7){
    image(c3, 350, 150,150,350);
  }
  
  //HUMEDAD
  text(data.getFloat("humedadRelativa")+" %", 100, 830);
  if (data.getFloat("humedadRelativa") < 36){
    image(h4, 350, 700,200,200);
  }else if (data.getFloat("humedadRelativa") >= 36 && data.getFloat("humedadRelativa") < 60){
    image(h2, 350, 700,200,200);
  }else if (data.getFloat("humedadRelativa") >= 60){
    image(h3, 350, 700,200,200);
  }else if (data.getFloat("humedadRelativa") >= 97){
    image(h1, 350, 700,200,200);
  }
  
  //HUMEDAD
  text(data.getFloat("humedadAbsoluta"), 600, 830);
  if (data.getFloat("humedadRelativa") < 36){
    image(h4, 870, 700,200,200);
  }else if (data.getFloat("humedadRelativa") >= 36 && data.getFloat("humedadRelativa") < 60){
    image(h2, 870, 700,200,200);
  }else if (data.getFloat("humedadRelativa") >= 60){
    image(h3, 870, 700,200,200);
  }else if (data.getFloat("humedadRelativa") >= 97){
    image(h1, 870, 700,200,200);
  }
  
  //Presion
  text(data.getFloat("presion"), 1100, 750);
  text("mmHg", 1100, 850);
  if (data.getFloat("presion") < 300){
    image(c1, 1400, 700,200,200);
  }else if (data.getFloat("presion") >= 300 && data.getFloat("presion") <= 550){
    image(c2, 1400, 700,200,200);
  }else if (data.getFloat("presion") > 550 && data.getFloat("presion") <= 800){
    image(c2, 1400, 700,200,200);
  }else if (data.getFloat("presion") > 800){
    image(c4, 1400, 700,200,200);
  }
  
  //Direccion
  text(data.getString("direccion"), 1120, 350);
  if (data.getString("direccion").equals("Sur")){
    image(d2, 1400, 150,150,300);
  }else if (data.getString("direccion").equals("Norte")){
    image(d1, 1400, 150,150,300);
  }else if (data.getString("direccion").equals("Este")){
    image(d3, 1400, 150,150,300);
  }else if (data.getString("direccion").equals("Oeste")){
    image(d4, 1400, 150,150,300);
  }else{
    image(d5, 1400, 150,150,300);
  }

  delay(3000);
}

JSONObject obtenerdata(){
  String[]jsonval = loadStrings("http://localhost:5090/api/datoultimo");
  saveStrings("data/valores.json", jsonval);
  JSONObject  jobj1 = loadJSONObject("valores.json");  
  return jobj1;
}
