
let datos;

function preload() {
	
	datos =  loadJSON('http://localhost:5090/api/datosordenados');
	

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
