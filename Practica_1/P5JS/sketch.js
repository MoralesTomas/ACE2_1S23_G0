function setup() {
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