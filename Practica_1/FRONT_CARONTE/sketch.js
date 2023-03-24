var datos = 123;

function fetchData() {
	return new Promise((resolve, reject) =>{
		try {
			const response = fetch('http://localhost:5097/');
			console.log(response)
			console.log("hola");
			datos = data;
			// Aquí se pueden ejecutar otras acciones que dependan de los datos recibidos
		} catch (error) {
			console.error("FALLAMOS PANA");
			console.log(error)
		}
	})
}

const logica_padre = async () => {
	const tmp = await fetchData()
	.then()
	.catch(function (error) {
		console.log(error);
		});
}

function setup() {


	logica_padre();
	console.log("ultimo");

	createCanvas(400, 400);
			// Create a new plot and set its position on the screen
			points = [];
		  seed = 100 * random();

		for (i = 0; i < 100; i++) {
			points[i] = new GPoint(i, 10 * noise(0.1 * i + seed));
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

function draw() {
//  background(220);
}