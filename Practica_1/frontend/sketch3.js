let px, py;
let tx = 500;
let ty = 500;
function setup(){
    let prueba = createCanvas(tx, ty);
    background(255);
    prueba.parent("dash1");
}
  
function draw(){
    px = tx/2;
    py = ty/2;
    ellipse(px, py, 400, 400);
    let startAngle = 0;
    let divisions = 36;
    let delta = 360.0 / divisions;
  
    //let centerX = 150;
    //let centerY = 150;
    let radius = Math.min(150, 150);
    let angle = startAngle;
    for (let i = 0; i < divisions; i++) {
        let x=(radius * Math.cos(degrees_to_radians(angle)));
        let y =(radius * Math.sin(degrees_to_radians(angle)));
        console.log(x, y);
        line( x+150, y+150,150,150);
        angle += delta;
    }
    
}
const degrees_to_radians = deg => (deg * Math.PI) / 180.0;