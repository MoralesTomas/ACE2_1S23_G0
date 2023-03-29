$(document).ready(function() {
    //let myChart = document.getElementById('myChart');
    //let lim_sup = document.getElementById('limit-sup');
    //let lim_inf = document.getElementById('limit-inf');
    /*----------------- FUNCTIONS -----------------*/

    //Funcion para establecer la hora y fecha actual como limite del input
    // function setCurrentDateInput(){
    //     let now = new Date();
    //     now.setUTCHours(now.getHours());
    //     lim_sup.max = now.toISOString().slice(0, 16);
    //     lim_sup.value = now.toISOString().slice(0, 16);
    //     lim_inf.max = now.toISOString().slice(0, 16);
    // }
    // setCurrentDateInput();

    /* ---------------------------------------------------------------------- */
    /* $.ajax({
        url: "https://mocki.io/v1/16d17515-8cd8-409e-9b5f-923c75c3399d",
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        method: "GET",
        success: function(data) {
            var nombre = [];
            var stock = [];
            console.log(data);
 
            for (var i in data) {
                nombre.push(data[i].nombre);
                stock.push(data[i].valor);
            }
 
            var chartdata = {
                labels: nombre,
                datasets: [{
                    label: "Prueba XD",
                    borderWidth: 1,
                    data: stock
                }]
            };
 
            var grafico = new Chart(myChart, {
                type: 'bar',
                data: chartdata,
                options: {
                    responsive: true,
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero: true
                            }
                        }]
                    }
                }
            });
        },
        error: function(data) {
            console.log(data);
        }
    }); */
    /* ---------------------------------------------------------------------- */
    const drawLine = (ctx, startX, startY, endX, endY,color)=>{
        ctx.save();
        ctx.strokeStyle = color;
        ctx.beginPath();
        ctx.moveTo(startX,startY);
        ctx.lineTo(endX,endY);
        ctx.stroke();
        ctx.restore();
    }

    const drawBar = (ctx, upperLeftCornerX, upperLeftCornerY, width, height,color)=>{
        ctx.save();
        ctx.fillStyle=color;
        ctx.fillRect(upperLeftCornerX,upperLeftCornerY,width,height);
        ctx.restore();
    }
    
    class Barchart {
        constructor(options) {
            this.options = options;
            this.canvas = options.canvas;
            this.ctx = this.canvas.getContext("2d");
            this.colors = options.colors;
            this.canvas.height = 400
            this.canvas.width = this.canvas.parentElement.clientWidth
    
            this.draw = function () {
                let maxValue = 100;
                let canvasActualHeight = this.canvas.height - this.options.padding * 2;
                let canvasActualWidth = this.canvas.width - this.options.padding * 2;
                let gridValue = 0;
                while (gridValue <= maxValue) {
                    let gridY = canvasActualHeight * (1 - gridValue / maxValue) + this.options.padding;
                    drawLine(this.ctx,0,gridY,this.canvas.width,gridY,this.options.gridColor);
    
                    this.ctx.save();
                    this.ctx.fillStyle = this.options.gridColor;
                    this.ctx.textBaseline = "bottom";
                    this.ctx.font = "bold 10px Arial";
                    this.ctx.fillText(gridValue, 10, gridY - 2);
                    this.ctx.restore();
                    gridValue += this.options.gridScale;
                }
    
                //drawing the bars 
                let barIndex = 0;
                var numberOfBars = Object.keys(this.options.data).length;
                var barSize = (canvasActualWidth) / numberOfBars;
                for (let categ in this.options.data) {
                    var val = this.options.data[categ];
                    var barHeight = Math.round(canvasActualHeight * val / maxValue);
                    drawBar(
                        this.ctx,
                        this.options.padding + barIndex * barSize,
                        this.canvas.height - barHeight - this.options.padding,
                        barSize,
                        barHeight,
                        this.colors[barIndex % this.colors.length]
                    );
                    barIndex++;
                }
                //drawing series name 
                this.ctx.save();
                this.ctx.textBaseline = "bottom";
                this.ctx.textAlign = "center";
                this.ctx.fillStyle = "#000000";
                this.ctx.font = "bold 14px Arial";
                this.ctx.fillText(this.options.seriesName, this.canvas.width / 2, this.canvas.height);
                this.ctx.restore();
    
                //draw legend 
                barIndex = 0;
                let legend = document.querySelector(`legend[for='${this.canvas.id}']`);
                legend.innerHTML=""
                let ul = document.createElement("ul");
                legend.append(ul);
                for (let categ in this.options.data) {
                    let li = document.createElement("li");
                    li.style.listStyle = "none";
                    li.style.borderLeft = "20px solid " + this.colors[barIndex % this.colors.length];
                    li.style.padding = "5px";
                    li.textContent = categ;
                    ul.append(li);
                    barIndex++;
                }
            };
        }
    }
    
    let livegraph = document.getElementById("live");
    let usuario1 = document.getElementById("usuario");
    let fechaInicio1 = document.getElementById("fechainicio");
    let fechafin1 = document.getElementById("fechafin");
    let btnbuscar1 = document.getElementById("btnbuscar");
    let selecthisotrica = document.getElementById("opcionhistorica");
    let historicagraph = document.getElementById("historica");

    // eliminar y colocar fech en ejecutarlive
    let live = {				
        "FECHA":"2/13/2023 17:39:21",			
        "DIA":13,			
        "MES":2,			
        "tiempo standar":25,
        "descanso standar":5,
        "P1": {"tiempo":12,"HoraInicio":"12:00","HoraFinal":"12:25"},
        "D1":{"tiempo":5,"HoraInicio":"12:25","HoraFinal":"12:30" },			
        "P2":{"tiempo":25,"HoraInicio":"12:30","HoraFinal":"12:55" },			
        "D2":{"tiempo":5,"HoraInicio":"12:55","HoraFinal":"13:00" },			
        "P3":{"tiempo":25,"HoraInicio":"13:00","HoraFinal":"13:25" },			
        "D3":{"tiempo":5,"HoraInicio":"13:25","HoraFinal":"13:30" },			
        "P4":{"tiempo":25,"HoraInicio":"13:30","HoraFinal":"13:55" },			
        "D4":{"tiempo":15,"HoraInicio":"13:55","HoraFinal":"14:10" }			
    }
    
    
    let prueba = 0;
    //Grafica en vivo
    const ejecutarlive = async ()=>{
        let data = null;
        await fetch('http://localhost:3000/data')
        .then(response => response.json())
        .then(d => data = d[1]);
        let stringdata = "{\n"
        let values = Object.keys(data)
        for(let i = 5;i<Object.keys(data).length;i++){
            if(values[i].includes("P")){
                stringdata+=`"${i-4+". "+values[i]+" "+data[values[i]].HoraInicio+"-"+data[values[i]].HoraFinal+" "+100*data[values[i]].tiempo/data["tiempo standar"]}":${100*data[values[i]].tiempo/data["tiempo standar"]},\n`
            }else{
                stringdata+=`"${i-4+". "+values[i]+" "+data[values[i]].HoraInicio+"-"+data[values[i]].HoraFinal+" "+100*data[values[i]].tiempo/data["descanso standar"]}":${100*data[values[i]].tiempo/data["descanso standar"]},\n`
            }
        }
        stringdata=stringdata.substring(0, stringdata.length - 2);
        stringdata+=`,\n"prueba":${prueba++}`
        stringdata+="}"
        let datalive = JSON.parse(stringdata)
        let myBarchart = new Barchart(
            {
                canvas:livegraph,
                seriesName:"Tabla En VIVO",
                padding:25,
                gridScale:25,
                gridColor:"#a7a5a5",
                data:datalive,
                colors:["#a55ca5","#67b6c7"]
            }
        );
        myBarchart.draw()
        
    }
    //Actualiza la grafica live cada segundo
    setInterval(ejecutarlive, 1000);

    //Grafica historica
    const ejecutarhisotrica = (datainput)=>{
        let data = datainput;
        let stringdata = "{\n"
        let values = Object.keys(data)
        for(let i = 5;i<Object.keys(data).length;i++){
            if(values[i].includes("P")){
                stringdata+=`"${i-4+". "+data["FECHA"]+" "+values[i]+" "+data[values[i]].HoraInicio+"-"+data[values[i]].HoraFinal+" "+100*data[values[i]].tiempo/data["tiempo standar"]}":${100*data[values[i]].tiempo/data["tiempo standar"]},\n`
            }else{
                stringdata+=`"${i-4+". "+data["FECHA"]+" "+values[i]+" "+data[values[i]].HoraInicio+"-"+data[values[i]].HoraFinal+" "+100*data[values[i]].tiempo/data["descanso standar"]}":${100*data[values[i]].tiempo/data["descanso standar"]},\n`
            }
        }
        stringdata=stringdata.substring(0, stringdata.length - 2);
        stringdata+="}"
        let datalive = JSON.parse(stringdata)
        let myBarchart = new Barchart(
            {
                canvas:historicagraph,
                seriesName:"Tabla Historica",
                padding:25,
                gridScale:25,
                gridColor:"#a7a5a5",
                data:datalive,
                colors:["#9FE2BF","#6495ED", "#9FE2BF","#6495ED", "#9FE2BF","#6495ED", "#9FE2BF", "#67b6c7"]
            }
        );
        myBarchart.draw()
        
    }

    let hisotorticodata = []
    btnbuscar1.addEventListener("click", async function(e){
        e.preventDefault()
        let fechainiciof = fechaInicio1.value
        let fechafinf = fechafin1.value
        let usuariof = usuario1.value
        
        await fetch('http://localhost:3000/data')
        .then(response => response.json())
        .then(d => hisotorticodata = d);
        let data = hisotorticodata
        let etiquetas=[];
        for(let i in data){
            etiquetas.push(data[i].FECHA)
        }
        opcionhistorica.innerHTML="";
        for (let categ of etiquetas) {
            let li = document.createElement("option");
            li.value=categ
            li.textContent = categ;
            opcionhistorica.append(li);
        }
    }, false);

    opcionhistorica.on("change", function(e){
        e.preventDefault()
        opcionhistorica.options[opcionhistorica.selectedIndex].text
        let dato=null
        for(let categ of hisotorticodata){
            if(opcionhistorica.options[opcionhistorica.selectedIndex].text===categ.FECHA){
                dato=categ
            }
        }
        if(dato!=null){
            ejecutarhisotrica(dato)
        }
    });
    
    const ejecutarpenalizaciones = ()=>{
        //en esta variable queda el fetch
        let data = live;
        let stringdata = "{\n"
        let values = Object.keys(data)
        for(let i = 5;i<Object.keys(data).length;i++){
            if(values[i].includes("P")){
                stringdata+=`"${i-4+". "+data["FECHA"]+" "+values[i]+" "+data[values[i]].HoraInicio+"-"+data[values[i]].HoraFinal+" "+100*data[values[i]].tiempo/data["tiempo standar"]}":${100*data[values[i]].tiempo/data["tiempo standar"]},\n`
            }else{
                stringdata+=`"${i-4+". "+data["FECHA"]+" "+values[i]+" "+data[values[i]].HoraInicio+"-"+data[values[i]].HoraFinal+" "+100*data[values[i]].tiempo/data["descanso standar"]}":${100*data[values[i]].tiempo/data["descanso standar"]},\n`
            }
        }
        stringdata=stringdata.substring(0, stringdata.length - 2);
        stringdata+="}"
        let datalive = JSON.parse(stringdata)
        let myBarchart = new Barchart(
            {
                canvas:livegraph,
                seriesName:"Tabla Hisotrica",
                padding:25,
                gridScale:25,
                gridColor:"#a7a5a5",
                data:datalive,
                colors:["#a55ca5","#67b6c7"]
            }
        );
        myBarchart.draw()
        
    }
});