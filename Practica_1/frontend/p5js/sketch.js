// target values
let values = ["40","3","189"];
// intermediate values
let lerpValues = [];
// index which will increase at regular intervals
let index = 0;

let datos;

function preload(){
	
	//datos =  loadJSON('http://localhost:5090/api/datosordenados');
	datos = {
		"0": {
			"id": "7c744739-017f-4a01-ad85-4e26189faf5a",
			"fecha": "2/15/2023 3:20:37 PM",
			"calor": 27.8,
			"humedadRelativa": 48,
			"humedadAbsoluta": 0.02,
			"velocidad": 1.42,
			"direccion": "Sur",
			"presion": "853.39"
		},
		"1": {
			"id": "5e12fdcd-f4bc-449f-ab48-72ed6938d7f1",
			"fecha": "2/15/2023 3:20:35 PM",
			"calor": 27.9,
			"humedadRelativa": 48,
			"humedadAbsoluta": 0.02,
			"velocidad": 1.42,
			"direccion": "Sur",
			"presion": "853.41"
		},
		"2": {
			"id": "68304c71-c34a-49de-9567-f49d259877a2",
			"fecha": "2/15/2023 3:20:33 PM",
			"calor": 27.8,
			"humedadRelativa": 49,
			"humedadAbsoluta": 0.02,
			"velocidad": 1.42,
			"direccion": "Sur",
			"presion": "853.41"
		},
		"3": {
			"id": "5989deee-3351-4da8-a35c-265c006c2c88",
			"fecha": "2/15/2023 3:20:31 PM",
			"calor": 27.8,
			"humedadRelativa": 49,
			"humedadAbsoluta": 0.02,
			"velocidad": 1.42,
			"direccion": "Sur",
			"presion": "853.40"
		}
	}
}


/* const tempExterna = ( sketch ) => {

	let tamx = 600;
	let tamy = 500;

	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
	};
  
	sketch.draw = () => {
		//sketch.background(255);
		//sketch.noStroke();
		//sketch.fill(0,200,220);
	};
};

const humedadAbsoluta = ( sketch ) => {

	let tamx = 600;
	let tamy = 500;
  
	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
		datos = Object.values(datos);
		// Create a new plot and set its position on the screen
		points = [];
		let fecha = new Date(datos[1].fecha);
		let seconds = fecha.getSeconds();
		console.log(seconds)
		for (let i = 0; i < datos.length; i++) {
			
			let calor = datos[i].humedadAbsoluta;
			//console.log(fecha);
			points[i] = new GPoint(seconds, calor);
			seconds += 2.00;
		}
		plot = new GPlot(sketch);
		plot.setPos(0, 0);
		plot.setOuterDim(tamx, tamy);

		// Add the points
		plot.setPoints(points);

		// Set the plot title and the axis labels
		plot.setTitleText("A very simple example");
		plot.getXAxis().setAxisLabelText("x axis");
		plot.getYAxis().setAxisLabelText("y axis");

		// Draw it!
		plot.defaultDraw();
	};
  
	sketch.draw = () => {
		//sketch.background(0);
		//sketch.noStroke();
		//sketch.fill(0,200,220);
	};
};

const humedadRelativa = ( sketch ) => {

	let tamx = 600;
	let tamy = 500;
  
	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
		datos = Object.values(datos);
		// Create a new plot and set its position on the screen
		points = [];
		let fecha = new Date(datos[1].fecha);
		let seconds = fecha.getSeconds();
		console.log(seconds)
		for (let i = 0; i < datos.length; i++) {
			
			let calor = datos[i].humedadRelativa;
			//console.log(fecha);
			points[i] = new GPoint(seconds, calor);
			seconds += 2.00;
		}
		plot = new GPlot(sketch);
		plot.setPos(0, 0);
		plot.setOuterDim(tamx, tamy);

		// Add the points
		plot.setPoints(points);

		// Set the plot title and the axis labels
		plot.setTitleText("A very simple example");
		plot.getXAxis().setAxisLabelText("x axis");
		plot.getYAxis().setAxisLabelText("y axis");

		// Draw it!
		plot.defaultDraw();
	};
  
	sketch.draw = () => {
		//sketch.background(0);
		//sketch.noStroke();
		//sketch.fill(0,200,220);
	};
};

const velViento = ( sketch ) => {

	let tamx = 600;
	let tamy = 500;
  
	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
		datos = Object.values(datos);
		// Create a new plot and set its position on the screen
		points = [];
		let fecha = new Date(datos[1].fecha);
		let seconds = fecha.getSeconds();
		console.log(seconds)
		for (let i = 0; i < datos.length; i++) {
			
			let calor = datos[i].velocidad;
			//console.log(fecha);
			points[i] = new GPoint(seconds, calor);
			seconds += 2.00;
		}
		plot = new GPlot(sketch);
		plot.setPos(0, 0);
		plot.setOuterDim(tamx, tamy);

		// Add the points
		plot.setPoints(points);

		// Set the plot title and the axis labels
		plot.setTitleText("A very simple example");
		plot.getXAxis().setAxisLabelText("x axis");
		plot.getYAxis().setAxisLabelText("y axis");

		// Draw it!
		plot.defaultDraw();
	};
  
	sketch.draw = () => {
		// sketch.background(0);
		// sketch.noStroke();
		// sketch.fill(0,200,220);
	};
};

const dirViento = ( sketch ) => {

	let tamx = 600;
	let tamy = 500;
  
	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
		datos = Object.values(datos);
		// Create a new plot and set its position on the screen
		points = [];
		let fecha = new Date(datos[1].fecha);
		let seconds = fecha.getSeconds();
		console.log(seconds)
		for (let i = 0; i < datos.length; i++) {
			
			let calor = datos[i].direccion;
			//console.log(fecha);
			points[i] = new GPoint(seconds, calor);
			seconds += 2.00;
		}
		plot = new GPlot(sketch);
		plot.setPos(0, 0);
		plot.setOuterDim(tamx, tamy);

		// Add the points
		plot.setPoints(points);

		// Set the plot title and the axis labels
		plot.setTitleText("A very simple example");
		plot.getXAxis().setAxisLabelText("x axis");
		plot.getYAxis().setAxisLabelText("y axis");

		// Draw it!
		plot.defaultDraw();
	};
  
	sketch.draw = () => {
		// sketch.background(0);
		// sketch.noStroke();
		// sketch.fill(0,200,220);
	};
};

const presionBar = ( sketch ) => {

	let tamx = 600;
	let tamy = 500;
  
	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
		datos = Object.values(datos);
		// Create a new plot and set its position on the screen
		points = [];
		let fecha = new Date(datos[1].fecha);
		let seconds = fecha.getSeconds();
		console.log(seconds)
		for (let i = 0; i < datos.length; i++) {
			
			let calor = parseFloat(datos[i].presion);
			//console.log(fecha);
			points[i] = new GPoint(seconds, calor);
			seconds += 2.00;
		}
		plot = new GPlot(sketch);
		plot.setPos(0, 0);
		plot.setOuterDim(tamx, tamy);

		// Add the points
		plot.setPoints(points);

		// Set the plot title and the axis labels
		plot.setTitleText("A very simple example");
		plot.getXAxis().setAxisLabelText("x axis");
		plot.getYAxis().setAxisLabelText("y axis");

		// Draw it!
		plot.defaultDraw();
	};
  
	sketch.draw = () => {
		// sketch.background(0);
		// sketch.noStroke();
		// sketch.fill(0,200,220);
	};
}; */

const htempExterna = ( sketch ) => {

	let tamx = 600;
	let tamy = 500;
  
	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
		datos = Object.values(datos);
		// Create a new plot and set its position on the screen
		points = [];
		let fecha = new Date(datos[1].fecha);
		let seconds = fecha.getSeconds();
		// console.log(seconds)
		for (let i = 0; i < datos.length; i++) {
			
			let calor = datos[i].calor;
			//console.log(fecha);
			points[i] = new GPoint(seconds, calor);
			seconds += 2.00;
		}
		plot = new GPlot(sketch);
		plot.setPos(0, 0);
		plot.setOuterDim(tamx, tamy);

		// Add the points
		plot.setPoints(points);

		// Set the plot title and the axis labels
		plot.setTitleText("A very simple example");
		plot.getXAxis().setAxisLabelText("x axis");
		plot.getYAxis().setAxisLabelText("y axis");

		// Draw it!
		plot.defaultDraw();
	};
  
	sketch.draw = () => {
		/* sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220); */
	};
};

const hhumedadRelativa = ( sketch ) => {

	let tamx = 600;
	let tamy = 500;
  
	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
		datos = Object.values(datos);
		// Create a new plot and set its position on the screen
		points = [];
		let fecha = new Date(datos[1].fecha);
		let seconds = fecha.getSeconds();
		// console.log(seconds)
		for (let i = 0; i < datos.length; i++) {
			
			let calor = datos[i].humedadRelativa;
			//console.log(fecha);
			points[i] = new GPoint(seconds, calor);
			seconds += 2.00;
		}
		plot = new GPlot(sketch);
		plot.setPos(0, 0);
		plot.setOuterDim(tamx, tamy);

		// Add the points
		plot.setPoints(points);

		// Set the plot title and the axis labels
		plot.setTitleText("A very simple example");
		plot.getXAxis().setAxisLabelText("x axis");
		plot.getYAxis().setAxisLabelText("y axis");

		// Draw it!
		plot.defaultDraw();
	};
  
	sketch.draw = () => {
		/* sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220); */
	};
};

const hhumedadAbsoluta = ( sketch ) => {

	let tamx = 600;
	let tamy = 500;
  
	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
		datos = Object.values(datos);
		// Create a new plot and set its position on the screen
		points = [];
		let fecha = new Date(datos[1].fecha);
		let seconds = fecha.getSeconds();
		// console.log(seconds)
		for (let i = 0; i < datos.length; i++) {
			
			let calor = datos[i].humedadAbsoluta;
			//console.log(fecha);
			points[i] = new GPoint(seconds, calor);
			seconds += 2.00;
		}
		plot = new GPlot(sketch);
		plot.setPos(0, 0);
		plot.setOuterDim(tamx, tamy);

		// Add the points
		plot.setPoints(points);

		// Set the plot title and the axis labels
		plot.setTitleText("A very simple example");
		plot.getXAxis().setAxisLabelText("x axis");
		plot.getYAxis().setAxisLabelText("y axis");

		// Draw it!
		plot.defaultDraw();
	};
  
	sketch.draw = () => {
		/* sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220); */
	};
};

const hvelViento = ( sketch ) => {

	let tamx = 600;
	let tamy = 500;
  
	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
		datos = Object.values(datos);
		// Create a new plot and set its position on the screen
		points = [];
		let fecha = new Date(datos[1].fecha);
		let seconds = fecha.getSeconds();
		// console.log(seconds)
		for (let i = 0; i < datos.length; i++) {
			
			let calor = datos[i].velocidad;
			//console.log(fecha);
			points[i] = new GPoint(seconds, calor);
			seconds += 2.00;
		}
		plot = new GPlot(sketch);
		plot.setPos(0, 0);
		plot.setOuterDim(tamx, tamy);

		// Add the points
		plot.setPoints(points);

		// Set the plot title and the axis labels
		plot.setTitleText("A very simple example");
		plot.getXAxis().setAxisLabelText("x axis");
		plot.getYAxis().setAxisLabelText("y axis");

		// Draw it!
		plot.defaultDraw();
	};
  
	sketch.draw = () => {
		/* sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220); */
	};
};

const hdirViento = ( sketch ) => {

	let tamx = 600;
	let tamy = 500;
  
	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
		datos = Object.values(datos);
		// Create a new plot and set its position on the screen
		points = [];
		let fecha = new Date(datos[1].fecha);
		let seconds = fecha.getSeconds();
		// console.log(seconds)
		for (let i = 0; i < datos.length; i++) {
			
			let calor = datos[i].direccion;
			//console.log(fecha);
			points[i] = new GPoint(seconds, calor);
			seconds += 2.00;
		}
		plot = new GPlot(sketch);
		plot.setPos(0, 0);
		plot.setOuterDim(tamx, tamy);

		// Add the points
		plot.setPoints(points);

		// Set the plot title and the axis labels
		plot.setTitleText("A very simple example");
		plot.getXAxis().setAxisLabelText("x axis");
		plot.getYAxis().setAxisLabelText("y axis");

		// Draw it!
		plot.defaultDraw();
	};
  
	sketch.draw = () => {
		/* sketch.background(0);
		sketch.noStroke();
		sketch.fill(0,200,220); */
	};
};

const hpresionBar = ( sketch ) => {

	let tamx = 600;
	let tamy = 500;
  
	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
		datos = Object.values(datos);
		// Create a new plot and set its position on the screen
		points = [];
		let fecha = new Date(datos[1].fecha);
		let seconds = fecha.getSeconds();
		// console.log(seconds)
		for (let i = 0; i < datos.length; i++) {
			
			let calor = parseFloat(datos[i].presion);
			//console.log(fecha);
			points[i] = new GPoint(seconds, calor);
			seconds += 2.00;
		}
		plot = new GPlot(sketch);
		plot.setPos(0, 0);
		plot.setOuterDim(tamx, tamy);

		// Add the points
		plot.setPoints(points);

		// Set the plot title and the axis labels
		plot.setTitleText("A very simple example");
		plot.getXAxis().setAxisLabelText("x axis");
		plot.getYAxis().setAxisLabelText("y axis");

		// Draw it!
		plot.defaultDraw();
	};
  
	sketch.draw = () => {
		// sketch.background(0);
		// sketch.noStroke();
		// sketch.fill(0,200,220);
	};
};
  
const gRadial = ( sketch ) => {

	let tamx = 500;
	let tamy = 500;
  
	sketch.setup = () => {
		sketch.createCanvas(tamx, tamy);
		sketch.background(255);
	};
  
	sketch.draw = () => {
		sketch.ellipse(500/2, 500/2, 400, 400);
		let startAngle = 0;
		let divisions = 36;
		let delta = 360.0 / divisions;
	
		//let centerX = 150;
		//let centerY = 150;
		let radius = Math.min(250, 250);
		let angle = startAngle;
		for (let i = 0; i < divisions; i++) {
			let x=(radius * Math.cos(degrees_to_radians(angle)));
			let y =(radius * Math.sin(degrees_to_radians(angle)));
			// console.log(x, y);
			sketch.line(x+250, y+250 , 250, 250);
			angle += delta;
		}
	};
};
const degrees_to_radians = deg => (deg * Math.PI) / 180.0;

// let graphTempExterna = new p5(tempExterna, "dash1");
// let graphHumedadA = new p5(humedadAbsoluta, "dash2");
// let graphHumedadR = new p5(humedadRelativa, "dash3");
// let graphVelViento = new p5(velViento, "dash4");
// let graphdirViento = new p5(dirViento, "dash5");
// let graphPresionBar = new p5(presionBar, "dash6");

let hgraphTempExterna = new p5(htempExterna, "exp1");
let hgraphHumedadA = new p5(hhumedadAbsoluta, "exp2");
let hgraphHumedadR = new p5(hhumedadRelativa, "exp3");
let hgraphVelViento = new p5(hvelViento, "exp4");
let hgraphPresionBar = new p5(hpresionBar, "exp5");
// let hgraphdirViento = new p5(hdirViento, "exp6");
let graphRadial = new p5(gRadial, "exp6");