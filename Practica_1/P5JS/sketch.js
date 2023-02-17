
let datos;

function preload() {
	
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
 
function setup() {
	createCanvas(1400, 1400);
	datos = Object.values(datos);
	// Create a new plot and set its position on the screen
	points = [];
	let fecha = new Date(datos[1].fecha);
	let seconds = fecha.getSeconds();
	console.log(seconds)
	for (let i = 0; i < datos.length; i++) {
		
		let calor = datos[i].calor;
		//console.log(fecha);
		points[i] = new GPoint(seconds, calor);
		seconds += 2.00;
	  }
	plot = new GPlot(this);
	plot.setPos(0, 0);
	plot.setOuterDim(width, height);

	// Add the points
	plot.setPoints(points);

	// Set the plot title and the axis labels
	plot.setTitleText("A very simple example");
	plot.getXAxis().setAxisLabelText("x axis");
	plot.getYAxis().setAxisLabelText("y axis");

	// Draw it!
	plot.defaultDraw();
}
