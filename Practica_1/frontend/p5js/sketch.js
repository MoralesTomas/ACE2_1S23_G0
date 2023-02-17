// target values
let values = ["40","3","189"];
// intermediate values
let lerpValues = [];
// index which will increase at regular intervals
let index = 0;


const tempExterna = ( sketch ) => {

	let x = 50;
	let y = 50;
  
	sketch.setup = () => {
		sketch.createCanvas(500, 500);
	};
  
	sketch.draw = () => {
		sketch.background(255);
		sketch.noStroke();
		sketch.fill(0,200,220);
	};
};

const humedadRelativa = ( sketch ) => {

	let x = 50;
	let y = 50;
  
	sketch.setup = () => {
		sketch.createCanvas(500, 500);
	};
  
	sketch.draw = () => {
		sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220);
	};
};

const velViento = ( sketch ) => {

	let x = 50;
	let y = 50;
  
	sketch.setup = () => {
		sketch.createCanvas(500, 500);
	};
  
	sketch.draw = () => {
		sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220);
	};
};

const dirViento = ( sketch ) => {

	let x = 50;
	let y = 50;
  
	sketch.setup = () => {
		sketch.createCanvas(500, 500);
	};
  
	sketch.draw = () => {
		sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220);
	};
};

const presionBar = ( sketch ) => {

	let x = 50;
	let y = 50;
  
	sketch.setup = () => {
		sketch.createCanvas(500, 500);
	};
  
	sketch.draw = () => {
		sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220);
	};
};

const htempExterna = ( sketch ) => {

	let x = 50;
	let y = 50;
  
	sketch.setup = () => {
		sketch.createCanvas(500, 500);
	};
  
	sketch.draw = () => {
		sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220);
	};
};

const hhumedadRelativa = ( sketch ) => {

	let x = 50;
	let y = 50;
  
	sketch.setup = () => {
		sketch.createCanvas(500, 500);
	};
  
	sketch.draw = () => {
		sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220);
	};
};

const hvelViento = ( sketch ) => {

	let x = 50;
	let y = 50;
  
	sketch.setup = () => {
		sketch.createCanvas(500, 500);
	};
  
	sketch.draw = () => {
		sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220);
	};
};

const hdirViento = ( sketch ) => {

	let x = 50;
	let y = 50;
  
	sketch.setup = () => {
		sketch.createCanvas(500, 500);
	};
  
	sketch.draw = () => {
		sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220);
	};
};

const hpresionBar = ( sketch ) => {

	let x = 50;
	let y = 50;
  
	sketch.setup = () => {
		sketch.createCanvas(500, 500);
	};
  
	sketch.draw = () => {
		sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220);
	};
};
  
  
let graphTempExterna = new p5(tempExterna, "dash1");
let graphHumedad = new p5(humedadRelativa, "dash2");
let graphVelViento = new p5(velViento, "dash3");
let graphdirViento = new p5(dirViento, "dash4");
let graphPresionBar = new p5(presionBar, "dash5");

let hgraphTempExterna = new p5(htempExterna, "exp1");
let hgraphHumedad = new p5(hhumedadRelativa, "exp2");
let hgraphVelViento = new p5(hvelViento, "exp3");
let hgraphdirViento = new p5(hdirViento, "exp4");
let hgraphPresionBar = new p5(hpresionBar, "exp5");