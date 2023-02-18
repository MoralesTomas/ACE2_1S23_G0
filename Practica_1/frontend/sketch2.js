// target values
let values = ["40","3","189"];
// intermediate values
let lerpValues = [];
// index which will increase at regular intervals
let index = 0;



function setup() {
	let prueba = createCanvas(600,400);
	prueba.parent("dash2");
	// Initialization of the table of intermediate values (all to zero)
	for(let i = 0;i<values.length;i++){
		lerpValues.push(0);
	}
    background(255);
	noStroke();
	fill(0,200,220);
	for(i=0;i<index;i++){
		let posx = map(i,0,values.length,40,width);
		lerpValues[i] = lerp(lerpValues[i],values[i],0.2);
		rect(posx, height-20, 40, -lerpValues[i]);
		textAlign(CENTER);
		text(round(lerpValues[i]),posx+20,height-lerpValues[i]-30);
	}
}

function draw() {
	
}

setInterval(function(){
	if(index<values.length){
	  index+=1;
	}
},100);

function windowResized() {
  resizeCanvas(400, 400);
}
